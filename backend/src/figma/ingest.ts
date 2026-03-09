import {
  CanonicalConstraints,
  CanonicalNode,
  CanonicalNodeType,
  CanonicalSceneGraph,
} from "../core/canonical-model.js";
import { validateSceneGraph } from "../core/schema-validator.js";

export interface IngestRequest {
  fileKey: string;
  token: string;
  nodeIds?: string[];
}

interface FigmaNode {
  id: string;
  name: string;
  type: string;
  children?: FigmaNode[];
}

interface FigmaFileResponse {
  document: FigmaNode;
}

const DEFAULT_CONSTRAINTS: CanonicalConstraints = {
  horizontal: "stretch",
  vertical: "stretch",
  widthBehavior: "fill",
  heightBehavior: "fill",
};

function toCanonicalType(figmaType: string): CanonicalNodeType {
  switch (figmaType) {
    case "TEXT":
      return "TEXT";
    case "COMPONENT":
      return "COMPONENT";
    case "INSTANCE":
      return "INSTANCE";
    case "RECTANGLE":
      return "IMAGE";
    default:
      return "FRAME";
  }
}

function flattenFigmaTree(
  node: FigmaNode,
  parentId: string | null,
  out: CanonicalNode[],
): void {
  const childIds = (node.children ?? []).map((child) => child.id);
  out.push({
    id: node.id,
    name: node.name,
    type: toCanonicalType(node.type),
    parentId,
    children: childIds,
    layout: {},
    constraints: DEFAULT_CONSTRAINTS,
    styleRefs: [],
  });

  for (const child of node.children ?? []) {
    flattenFigmaTree(child, node.id, out);
  }
}

export async function fetchFigmaFile({
  fileKey,
  token,
}: IngestRequest): Promise<FigmaFileResponse> {
  const response = await fetch(`https://api.figma.com/v1/files/${fileKey}`, {
    headers: {
      "X-Figma-Token": token,
    },
  });
  if (!response.ok) {
    throw new Error(`Failed to fetch Figma file: ${response.status} ${response.statusText}`);
  }
  const payload = (await response.json()) as FigmaFileResponse;
  return payload;
}

export function normalizeFigmaToSceneGraph(
  figmaFileKey: string,
  figmaRoot: FigmaNode,
): CanonicalSceneGraph {
  const nodes: CanonicalNode[] = [];
  flattenFigmaTree(figmaRoot, null, nodes);

  const graph: CanonicalSceneGraph = {
    schemaVersion: "1.0.0",
    documentId: figmaFileKey,
    rootNodeId: figmaRoot.id,
    nodes,
    styles: [],
    assets: [],
  };

  const validation = validateSceneGraph(graph);
  if (!validation.ok) {
    throw new Error(
      `Scene graph validation failed: ${validation.issues
        .map((issue) => issue.message)
        .join("; ")}`,
    );
  }

  return graph;
}

export async function ingestFigmaToCanonical(req: IngestRequest): Promise<CanonicalSceneGraph> {
  const figmaFile = await fetchFigmaFile(req);
  return normalizeFigmaToSceneGraph(req.fileKey, figmaFile.document);
}
