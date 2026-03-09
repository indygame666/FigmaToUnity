using UnityEditor;
using UnityEngine;

namespace FigmaToUnity.Converter
{
    public static class ImportPreviewPanel
    {
        public static void Draw(FigmaImportReport report)
        {
            EditorGUILayout.LabelField("Preview / Diff", EditorStyles.boldLabel);
            if (report == null)
            {
                EditorGUILayout.HelpBox("Run Preview or Import to generate a diff report.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Mode", report.Mode);
            EditorGUILayout.LabelField("File", string.IsNullOrEmpty(report.FileKey) ? "(empty)" : report.FileKey);
            EditorGUILayout.LabelField("Operations", report.Operations.Count.ToString());

            foreach (var operation in report.Operations)
            {
                var label = $"{operation.Action.ToUpperInvariant()} {operation.NodeId} -> {operation.TargetPath}";
                EditorGUILayout.LabelField(label);
            }

            EditorGUILayout.Space(8);
        }
    }
}
