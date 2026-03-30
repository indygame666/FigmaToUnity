using System;
using UnityEditor;
using UnityEngine;

namespace FigmaToUnity.Converter
{
    /// <summary>
    /// Main editor window for configuring and running Figma import operations.
    /// </summary>
    public class FigmaImporterWindow : EditorWindow
    {
        private string _figmaUrl = string.Empty;
        private string _figmaFileKey = string.Empty;
        private string _nodeIdsCsv = string.Empty;
        private string _figmaToken = string.Empty;
        private string _outputFolder = "Assets/UI/Generated";
        private string _fileNamePattern = "{nodeName}_{nodeId}";
        private Vector2 _scroll;
        private FigmaImportReport _lastReport;

        [MenuItem("Tools/Figma/Importer")]
        public static void Open()
        {
            var window = GetWindow<FigmaImporterWindow>("Figma Importer");
            window.minSize = new Vector2(600, 420);
        }

        private void OnEnable()
        {
            _figmaToken = FigmaImporterService.LoadSavedToken();
            _outputFolder = FigmaImporterService.LoadSavedOutputFolder();
            _fileNamePattern = FigmaImporterService.LoadSavedFileNamePattern();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Figma -> Unity Converter", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            _figmaUrl = EditorGUILayout.TextField("Figma URL", _figmaUrl);
            if (GUILayout.Button("Parse URL"))
            {
                if (TryParseFigmaUrl(_figmaUrl, out var fileKey, out var nodeIdsCsv, out var parseError))
                {
                    _figmaFileKey = fileKey;
                    _nodeIdsCsv = nodeIdsCsv;
                }
                else
                {
                    _lastReport = new FigmaImportReport
                    {
                        Mode = "Parse URL",
                        FileKey = _figmaFileKey,
                        PerformedAtIso = DateTime.UtcNow.ToString("O"),
                        Warnings = { parseError },
                        TechnicalDetails = "URL parsing failed."
                    };
                }
            }

            _figmaToken = EditorGUILayout.PasswordField("Figma Token", _figmaToken);
            _figmaFileKey = EditorGUILayout.TextField("File Key", _figmaFileKey);
            _nodeIdsCsv = EditorGUILayout.TextField("Node IDs (csv)", _nodeIdsCsv);
            _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);
            _fileNamePattern = EditorGUILayout.TextField("File Name Pattern", _fileNamePattern);
            EditorGUILayout.HelpBox("Node ID format: 0:1,12:34. For Figma URL node-id=0-1, use 0:1.", MessageType.None);
            EditorGUILayout.HelpBox("File name placeholders: {nodeName}, {nodeId}. Example: UI_{nodeName}_{nodeId}", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Token"))
            {
                FigmaImporterService.SaveToken(_figmaToken);
            }
            if (GUILayout.Button("Save Output Settings"))
            {
                FigmaImporterService.SaveOutputSettings(_outputFolder, _fileNamePattern);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview"))
            {
                _lastReport = FigmaImporterService.RunPreview(_figmaFileKey, _nodeIdsCsv, _figmaToken, _outputFolder, _fileNamePattern);
            }

            if (GUILayout.Button("Import"))
            {
                _lastReport = FigmaImporterService.RunImport(_figmaFileKey, _nodeIdsCsv, _figmaToken, _outputFolder, _fileNamePattern, false);
            }

            if (GUILayout.Button("Reimport"))
            {
                _lastReport = FigmaImporterService.RunImport(_figmaFileKey, _nodeIdsCsv, _figmaToken, _outputFolder, _fileNamePattern, true);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            ImportPreviewPanel.Draw(_lastReport);
            ImportReportView.Draw(_lastReport);
            EditorGUILayout.EndScrollView();
        }

        private static bool TryParseFigmaUrl(string input, out string fileKey, out string nodeIdsCsv, out string error)
        {
            fileKey = string.Empty;
            nodeIdsCsv = string.Empty;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Figma URL is empty.";
                return false;
            }

            if (!Uri.TryCreate(input.Trim(), UriKind.Absolute, out var uri))
            {
                error = "Invalid Figma URL format.";
                return false;
            }

            var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
            {
                error = "Cannot extract file key from URL path.";
                return false;
            }

            if (!string.Equals(segments[0], "design", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(segments[0], "file", StringComparison.OrdinalIgnoreCase))
            {
                error = "URL must contain /design/{fileKey}/... or /file/{fileKey}/...";
                return false;
            }

            fileKey = segments[1];
            if (string.IsNullOrWhiteSpace(fileKey))
            {
                error = "File key is empty in URL.";
                return false;
            }

            var query = uri.Query;
            if (query.StartsWith("?", StringComparison.Ordinal))
            {
                query = query.Substring(1);
            }

            foreach (var pair in query.Split('&'))
            {
                if (string.IsNullOrWhiteSpace(pair))
                {
                    continue;
                }

                var kv = pair.Split(new[] { '=' }, 2);
                var key = Uri.UnescapeDataString(kv[0]);
                if (!string.Equals(key, "node-id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var rawValue = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
                nodeIdsCsv = rawValue.Replace('-', ':');
                break;
            }

            if (string.IsNullOrWhiteSpace(nodeIdsCsv))
            {
                error = "URL does not contain node-id query parameter.";
                return false;
            }

            return true;
        }
    }
}
