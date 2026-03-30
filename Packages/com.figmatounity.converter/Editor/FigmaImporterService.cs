using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace FigmaToUnity.Converter
{
    /// <summary>
    /// Provides editor-side integration with the Figma REST API and builds import reports.
    /// </summary>
    public static class FigmaImporterService
    {
        private const string FigmaApiBase = "https://api.figma.com/v1";
        private const string FigmaTokenEditorPrefKey = "FigmaToUnity.FigmaApiToken";
        private const string OutputFolderEditorPrefKey = "FigmaToUnity.OutputFolder";
        private const string FileNamePatternEditorPrefKey = "FigmaToUnity.FileNamePattern";
        private const string DefaultOutputFolder = "Assets/UI/Generated";
        private const string DefaultFileNamePattern = "{nodeName}_{nodeId}";

        /// <summary>
        /// Executes a preview request and returns a report without applying asset updates.
        /// </summary>
        public static FigmaImportReport RunPreview(string fileKey, string nodeIdsCsv, string figmaToken, string outputFolder, string fileNamePattern)
        {
            return BuildReport("Preview", fileKey, nodeIdsCsv, figmaToken, outputFolder, fileNamePattern, false);
        }

        /// <summary>
        /// Executes an import request and returns an operation report.
        /// </summary>
        public static FigmaImportReport RunImport(string fileKey, string nodeIdsCsv, string figmaToken, string outputFolder, string fileNamePattern, bool isReimport)
        {
            return BuildReport(isReimport ? "Reimport" : "Import", fileKey, nodeIdsCsv, figmaToken, outputFolder, fileNamePattern, isReimport);
        }

        /// <summary>
        /// Loads an editor-local token saved by the importer window.
        /// </summary>
        public static string LoadSavedToken()
        {
            return EditorPrefs.GetString(FigmaTokenEditorPrefKey, string.Empty);
        }

        /// <summary>
        /// Saves an editor-local token used for authenticating Figma API requests.
        /// </summary>
        public static void SaveToken(string token)
        {
            EditorPrefs.SetString(FigmaTokenEditorPrefKey, token ?? string.Empty);
        }

        public static string LoadSavedOutputFolder()
        {
            return EditorPrefs.GetString(OutputFolderEditorPrefKey, DefaultOutputFolder);
        }

        public static string LoadSavedFileNamePattern()
        {
            return EditorPrefs.GetString(FileNamePatternEditorPrefKey, DefaultFileNamePattern);
        }

        public static void SaveOutputSettings(string outputFolder, string fileNamePattern)
        {
            EditorPrefs.SetString(OutputFolderEditorPrefKey, string.IsNullOrWhiteSpace(outputFolder) ? DefaultOutputFolder : outputFolder.Trim());
            EditorPrefs.SetString(FileNamePatternEditorPrefKey, string.IsNullOrWhiteSpace(fileNamePattern) ? DefaultFileNamePattern : fileNamePattern.Trim());
        }

        private static FigmaImportReport BuildReport(string mode, string fileKey, string nodeIdsCsv, string figmaToken, string outputFolder, string fileNamePattern, bool reimport)
        {
            var token = string.IsNullOrWhiteSpace(figmaToken) ? LoadSavedToken() : figmaToken.Trim();
            var resolvedOutputFolder = string.IsNullOrWhiteSpace(outputFolder) ? LoadSavedOutputFolder() : outputFolder.Trim();
            var resolvedFileNamePattern = string.IsNullOrWhiteSpace(fileNamePattern) ? LoadSavedFileNamePattern() : fileNamePattern.Trim();
            var nodeIds = ParseNodeIds(nodeIdsCsv);
            var warnings = new List<string>();

            if (string.IsNullOrWhiteSpace(fileKey))
            {
                warnings.Add("File key is empty.");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                warnings.Add("Figma API token is empty.");
            }
            if (nodeIds.Count == 0)
            {
                warnings.Add("Node IDs are empty.");
            }
            if (!IsValidAssetsRelativePath(resolvedOutputFolder))
            {
                warnings.Add("Output folder must start with 'Assets/' or be exactly 'Assets'.");
            }
            if (string.IsNullOrWhiteSpace(resolvedFileNamePattern))
            {
                warnings.Add("File name pattern is empty.");
            }

            var operations = new List<ImportOperation>();

            if (warnings.Count == 0)
            {
                var requestResult = FetchNodes(fileKey.Trim(), token, nodeIds);
                warnings.AddRange(requestResult.Warnings);

                foreach (var node in requestResult.Nodes)
                {
                    operations.Add(new ImportOperation
                    {
                        NodeId = node.NodeId,
                        NodeName = node.NodeName,
                        Action = reimport ? "update" : "create",
                        TargetPath = BuildTargetPath(resolvedOutputFolder, resolvedFileNamePattern, node.NodeName, node.NodeId)
                    });
                }

                if (!string.Equals(mode, "Preview", StringComparison.OrdinalIgnoreCase))
                {
                    var generationSummary = GeneratePrefabAssets(operations, resolvedOutputFolder, reimport);
                    warnings.AddRange(generationSummary.Warnings);
                }
            }

            return new FigmaImportReport
            {
                Mode = mode,
                FileKey = fileKey,
                PerformedAtIso = DateTime.UtcNow.ToString("O"),
                Operations = operations,
                Warnings = warnings,
                TechnicalDetails = BuildTechnicalDetails(nodeIds, operations)
            };
        }

        private static List<string> ParseNodeIds(string nodeIdsCsv)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(nodeIdsCsv))
            {
                return result;
            }

            foreach (var raw in nodeIdsCsv.Split(','))
            {
                var value = raw.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        private static string BuildTechnicalDetails(List<string> requestedNodeIds, List<ImportOperation> operations)
        {
            return $"Requested nodes: {requestedNodeIds.Count}, resolved nodes: {operations.Count}. API: GET /files/{{key}}/nodes";
        }

        private static PrefabGenerationSummary GeneratePrefabAssets(List<ImportOperation> operations, string outputFolder, bool reimport)
        {
            var summaryWarnings = new List<string>();
            EnsureAssetFolderPathExists(outputFolder);

            foreach (var operation in operations)
            {
                var targetPath = operation.TargetPath;
                var exists = System.IO.File.Exists(targetPath);
                if (exists && !reimport)
                {
                    summaryWarnings.Add($"Skipped existing prefab: {targetPath}. Use Reimport to overwrite.");
                    continue;
                }

                try
                {
                    var root = new GameObject(operation.NodeName);
                    var rectTransform = root.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(100f, 100f);

                    PrefabUtility.SaveAsPrefabAsset(root, targetPath);
                    UnityEngine.Object.DestroyImmediate(root);
                }
                catch (Exception ex)
                {
                    summaryWarnings.Add($"Failed to generate prefab for node '{operation.NodeId}': {ex.Message}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new PrefabGenerationSummary
            {
                Warnings = summaryWarnings
            };
        }

        private static void EnsureAssetFolderPathExists(string fullPath)
        {
            var parts = fullPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = parts[i];
                var combined = $"{current}/{next}";
                if (!AssetDatabase.IsValidFolder(combined))
                {
                    AssetDatabase.CreateFolder(current, next);
                }

                current = combined;
            }
        }

        private static bool IsValidAssetsRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path == "Assets" || path.StartsWith("Assets/", StringComparison.Ordinal);
        }

        private static string BuildTargetPath(string outputFolder, string fileNamePattern, string nodeName, string nodeId)
        {
            var safeNodeName = SanitizeFileName(nodeName);
            var safeNodeId = SanitizeFileName(nodeId);
            var fileName = fileNamePattern
                .Replace("{nodeName}", safeNodeName)
                .Replace("{nodeId}", safeNodeId)
                .Trim();

            fileName = SanitizeFileName(fileName);
            if (!fileName.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".prefab";
            }

            return $"{outputFolder}/{fileName}";
        }

        private static FetchNodesResult FetchNodes(string fileKey, string token, List<string> nodeIds)
        {
            var warnings = new List<string>();
            var encodedIds = string.Join(",", nodeIds.Select(UnityWebRequest.EscapeURL));
            var url = $"{FigmaApiBase}/files/{UnityWebRequest.EscapeURL(fileKey)}/nodes?ids={encodedIds}";

            using (var request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("X-Figma-Token", token);
                var operation = request.SendWebRequest();
                while (!operation.isDone) { }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    warnings.Add($"Figma API request failed: {request.error}");
                    if (!string.IsNullOrWhiteSpace(request.downloadHandler?.text))
                    {
                        warnings.Add($"Figma API body: {request.downloadHandler.text}");
                    }

                    return new FetchNodesResult { Nodes = new List<FigmaNodeInfo>(), Warnings = warnings };
                }

                try
                {
                    var payload = JObject.Parse(request.downloadHandler.text);
                    var nodesToken = payload["nodes"] as JObject;
                    if (nodesToken == null)
                    {
                        warnings.Add("Figma API response does not contain 'nodes'.");
                        return new FetchNodesResult { Nodes = new List<FigmaNodeInfo>(), Warnings = warnings };
                    }

                    var resultNodes = new List<FigmaNodeInfo>();
                    foreach (var requestedId in nodeIds)
                    {
                        var nodeEntry = nodesToken[requestedId] as JObject;
                        var document = nodeEntry?["document"] as JObject;
                        if (document == null)
                        {
                            warnings.Add($"Node '{requestedId}' not found in response.");
                            continue;
                        }

                        var nodeName = document.Value<string>("name");
                        if (string.IsNullOrWhiteSpace(nodeName))
                        {
                            nodeName = "UnnamedNode";
                        }

                        resultNodes.Add(new FigmaNodeInfo
                        {
                            NodeId = requestedId,
                            NodeName = nodeName
                        });
                    }

                    return new FetchNodesResult { Nodes = resultNodes, Warnings = warnings };
                }
                catch (Exception ex)
                {
                    warnings.Add($"Failed to parse Figma API response: {ex.Message}");
                    return new FetchNodesResult { Nodes = new List<FigmaNodeInfo>(), Warnings = warnings };
                }
            }
        }

        private static string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "unnamed";
            }

            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            var chars = input.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray();
            return new string(chars).Replace(' ', '_');
        }
    }

    [Serializable]
    public class ImportOperation
    {
        public string NodeId;
        public string NodeName;
        public string Action;
        public string TargetPath;
    }

    [Serializable]
    public class FigmaImportReport
    {
        public string Mode;
        public string FileKey;
        public string PerformedAtIso;
        public List<ImportOperation> Operations = new List<ImportOperation>();
        public List<string> Warnings = new List<string>();
        public string TechnicalDetails;
    }

    internal class FigmaNodeInfo
    {
        public string NodeId;
        public string NodeName;
    }

    internal class FetchNodesResult
    {
        public List<FigmaNodeInfo> Nodes;
        public List<string> Warnings;
    }

    internal class PrefabGenerationSummary
    {
        public List<string> Warnings;
    }
}
