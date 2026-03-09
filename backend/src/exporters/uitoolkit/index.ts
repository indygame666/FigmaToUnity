import { CanonicalSceneGraph } from "../../core/canonical-model.js";
import { runConstraintEngine } from "../../layout/constraint-engine.js";
import { buildAssetManifest } from "../assets/manifest.js";

export interface UiToolkitExportResult {
  uxmlText: string;
  ussText: string;
  report: {
    warnings: string[];
    nodesExported: number;
  };
  assetManifestJson: string;
}

function toVisualElementTag(type: string): string {
  switch (type) {
    case "TEXT":
      return "Label";
    case "IMAGE":
      return "VisualElement";
    default:
      return "VisualElement";
  }
}

export function exportToUiToolkit(graph: CanonicalSceneGraph): UiToolkitExportResult {
  const layoutResult = runConstraintEngine(graph);
  const warnings = layoutResult.warnings.map(
    (w) => `${w.level.toUpperCase()} [${w.nodeId}] ${w.message}`,
  );

  const uxmlLines: string[] = [];
  uxmlLines.push('<ui:UXML xmlns:ui="UnityEngine.UIElements">');
  uxmlLines.push('  <ui:VisualElement name="ImportedFigmaRoot">');
  for (const node of graph.nodes) {
    const tag = toVisualElementTag(node.type);
    uxmlLines.push(`    <ui:${tag} name="${node.name}" />`);
  }
  uxmlLines.push("  </ui:VisualElement>");
  uxmlLines.push("</ui:UXML>");

  const ussLines: string[] = [];
  ussLines.push(".ImportedFigmaRoot {");
  ussLines.push("  flex-direction: column;");
  ussLines.push("}");

  return {
    uxmlText: uxmlLines.join("\n"),
    ussText: ussLines.join("\n"),
    report: {
      warnings,
      nodesExported: graph.nodes.length,
    },
    assetManifestJson: JSON.stringify(buildAssetManifest(graph), null, 2),
  };
}
