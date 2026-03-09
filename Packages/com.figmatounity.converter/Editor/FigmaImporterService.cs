using System;
using System.Collections.Generic;
using UnityEngine;

namespace FigmaToUnity.Converter
{
    public static class FigmaImporterService
    {
        public static FigmaImportReport RunPreview(string fileKey, string nodeIdsCsv)
        {
            return BuildReport("Preview", fileKey, nodeIdsCsv, false);
        }

        public static FigmaImportReport RunImport(string fileKey, string nodeIdsCsv, bool isReimport)
        {
            return BuildReport(isReimport ? "Reimport" : "Import", fileKey, nodeIdsCsv, isReimport);
        }

        private static FigmaImportReport BuildReport(string mode, string fileKey, string nodeIdsCsv, bool reimport)
        {
            var nodeIds = ParseNodeIds(nodeIdsCsv);
            var warnings = new List<string>();
            if (string.IsNullOrWhiteSpace(fileKey))
            {
                warnings.Add("File key is empty.");
            }

            // Stable IDs are used to ensure idempotent reimport behavior.
            var operations = new List<ImportOperation>();
            foreach (var nodeId in nodeIds)
            {
                operations.Add(new ImportOperation
                {
                    NodeId = nodeId,
                    Action = reimport ? "update" : "create",
                    TargetPath = $"Assets/UI/Generated/{nodeId}.prefab"
                });
            }

            return new FigmaImportReport
            {
                Mode = mode,
                FileKey = fileKey,
                PerformedAtIso = DateTime.UtcNow.ToString("O"),
                Operations = operations,
                Warnings = warnings,
                TechnicalDetails = "Backend pipeline placeholder: ingest -> normalize -> responsive -> export."
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
    }

    [Serializable]
    public class ImportOperation
    {
        public string NodeId;
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
}
