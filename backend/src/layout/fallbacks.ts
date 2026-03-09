import { CanonicalNode } from "../core/canonical-model.js";

export type WarningLevel = "info" | "warning" | "error";

export interface LayoutWarning {
  level: WarningLevel;
  nodeId: string;
  message: string;
  fallbackApplied?: string;
}

export function makeFallbackWarning(
  node: CanonicalNode,
  message: string,
  fallbackApplied: string,
): LayoutWarning {
  return {
    level: "warning",
    nodeId: node.id,
    message,
    fallbackApplied,
  };
}
