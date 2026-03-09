using UnityEditor;
using UnityEngine;

namespace FigmaToUnity.Converter
{
    public class FigmaImporterWindow : EditorWindow
    {
        private string _figmaFileKey = string.Empty;
        private string _nodeIdsCsv = string.Empty;
        private Vector2 _scroll;
        private FigmaImportReport _lastReport;

        [MenuItem("Tools/Figma/Importer")]
        public static void Open()
        {
            var window = GetWindow<FigmaImporterWindow>("Figma Importer");
            window.minSize = new Vector2(600, 420);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Figma -> Unity Converter", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            _figmaFileKey = EditorGUILayout.TextField("File Key", _figmaFileKey);
            _nodeIdsCsv = EditorGUILayout.TextField("Node IDs (csv)", _nodeIdsCsv);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview"))
            {
                _lastReport = FigmaImporterService.RunPreview(_figmaFileKey, _nodeIdsCsv);
            }

            if (GUILayout.Button("Import"))
            {
                _lastReport = FigmaImporterService.RunImport(_figmaFileKey, _nodeIdsCsv, false);
            }

            if (GUILayout.Button("Reimport"))
            {
                _lastReport = FigmaImporterService.RunImport(_figmaFileKey, _nodeIdsCsv, true);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            ImportPreviewPanel.Draw(_lastReport);
            ImportReportView.Draw(_lastReport);
            EditorGUILayout.EndScrollView();
        }
    }
}
