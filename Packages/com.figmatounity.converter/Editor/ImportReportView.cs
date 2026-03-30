using UnityEditor;
using UnityEngine;

namespace FigmaToUnity.Converter
{
    /// <summary>
    /// Renders warnings and technical details produced by the importer pipeline.
    /// </summary>
    public static class ImportReportView
    {
        public static void Draw(FigmaImportReport report)
        {
            EditorGUILayout.LabelField("Import Report", EditorStyles.boldLabel);
            if (report == null)
            {
                EditorGUILayout.HelpBox("No import report yet.", MessageType.None);
                return;
            }

            if (report.Warnings.Count > 0)
            {
                foreach (var warning in report.Warnings)
                {
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No warnings.", MessageType.Info);
            }

            EditorGUILayout.LabelField("Technical details");
            EditorGUILayout.SelectableLabel(report.TechnicalDetails, GUILayout.Height(36));
        }
    }
}
