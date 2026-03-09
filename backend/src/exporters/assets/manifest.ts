import { CanonicalSceneGraph } from "../../core/canonical-model.js";

export interface AssetManifestEntry {
  assetId: string;
  sourceUri: string;
  targetPath: string;
}

export interface AssetManifest {
  generatedAtIso: string;
  documentId: string;
  entries: AssetManifestEntry[];
}

export function buildAssetManifest(graph: CanonicalSceneGraph): AssetManifest {
  return {
    generatedAtIso: new Date().toISOString(),
    documentId: graph.documentId,
    entries: graph.assets.map((asset) => ({
      assetId: asset.assetId,
      sourceUri: asset.sourceUri,
      targetPath: asset.targetPath,
    })),
  };
}
