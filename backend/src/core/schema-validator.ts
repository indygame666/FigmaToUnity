import {
  CanonicalSceneGraph,
  ValidationIssue,
  ValidationResult,
} from "./canonical-model.js";

export function validateSceneGraph(graph: CanonicalSceneGraph): ValidationResult {
  const issues: ValidationIssue[] = [];

  if (!graph.schemaVersion) {
    issues.push({ level: "error", message: "schemaVersion is required" });
  }
  if (!graph.documentId) {
    issues.push({ level: "error", message: "documentId is required" });
  }
  if (!graph.rootNodeId) {
    issues.push({ level: "error", message: "rootNodeId is required" });
  }

  const nodeIds = new Set<string>();
  for (const node of graph.nodes) {
    if (nodeIds.has(node.id)) {
      issues.push({
        level: "error",
        message: `Duplicate node id: ${node.id}`,
        nodeId: node.id,
      });
    }
    nodeIds.add(node.id);
  }

  if (!nodeIds.has(graph.rootNodeId)) {
    issues.push({
      level: "error",
      message: `rootNodeId not found in nodes: ${graph.rootNodeId}`,
      nodeId: graph.rootNodeId,
    });
  }

  for (const node of graph.nodes) {
    if (node.parentId && !nodeIds.has(node.parentId)) {
      issues.push({
        level: "error",
        message: `Parent node not found: ${node.parentId}`,
        nodeId: node.id,
      });
    }

    for (const childId of node.children) {
      if (!nodeIds.has(childId)) {
        issues.push({
          level: "error",
          message: `Child node not found: ${childId}`,
          nodeId: node.id,
        });
      }
    }
  }

  return { ok: issues.every((x) => x.level !== "error"), issues };
}
