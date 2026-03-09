import { CanonicalSceneGraph } from "../../core/canonical-model.js";
import { runConstraintEngine } from "../../layout/constraint-engine.js";
import { buildAssetManifest } from "../assets/manifest.js";

export interface UguiExportResult {
  prefabText: string;
  report: {
    warnings: string[];
    nodesExported: number;
  };
  assetManifestJson: string;
}

export function exportToUgui(graph: CanonicalSceneGraph): UguiExportResult {
  const layoutResult = runConstraintEngine(graph);
  const warnings = layoutResult.warnings.map(
    (w) => `${w.level.toUpperCase()} [${w.nodeId}] ${w.message}`,
  );

  const prefabLines: string[] = [];
  prefabLines.push("%YAML 1.1");
  prefabLines.push("%TAG !u! tag:unity3d.com,2011:");
  prefabLines.push("--- !u!1 &1000");
  prefabLines.push("GameObject:");
  prefabLines.push("  m_Name: ImportedFigmaRoot");
  prefabLines.push("  m_Component: []");

  for (const node of graph.nodes) {
    prefabLines.push(`# Node ${node.id} ${node.type} ${node.name}`);
  }

  return {
    prefabText: prefabLines.join("\n"),
    report: {
      warnings,
      nodesExported: graph.nodes.length,
    },
    assetManifestJson: JSON.stringify(buildAssetManifest(graph), null, 2),
  };
}
