import { CanonicalNode, CanonicalSceneGraph } from "../core/canonical-model.js";
import { LayoutWarning, makeFallbackWarning } from "./fallbacks.js";
import { ResponsiveRule, buildResponsiveRule } from "./responsive-rules.js";

export interface ConstraintEngineResult {
  rules: ResponsiveRule[];
  warnings: LayoutWarning[];
}

function normalizeInvalidConstraints(node: CanonicalNode): LayoutWarning[] {
  const warnings: LayoutWarning[] = [];
  const c = node.constraints;

  if (c.minWidth !== undefined && c.maxWidth !== undefined && c.minWidth > c.maxWidth) {
    warnings.push(
      makeFallbackWarning(
        node,
        "minWidth is greater than maxWidth",
        "swap minWidth and maxWidth",
      ),
    );
    [c.minWidth, c.maxWidth] = [c.maxWidth, c.minWidth];
  }

  if (c.minHeight !== undefined && c.maxHeight !== undefined && c.minHeight > c.maxHeight) {
    warnings.push(
      makeFallbackWarning(
        node,
        "minHeight is greater than maxHeight",
        "swap minHeight and maxHeight",
      ),
    );
    [c.minHeight, c.maxHeight] = [c.maxHeight, c.minHeight];
  }

  if (node.layout.layoutMode === "NONE" && node.type === "FRAME") {
    warnings.push(
      makeFallbackWarning(
        node,
        "Frame without explicit layout mode",
        "use fixed positioning fallback",
      ),
    );
  }

  return warnings;
}

export function runConstraintEngine(graph: CanonicalSceneGraph): ConstraintEngineResult {
  const warnings: LayoutWarning[] = [];
  const rules: ResponsiveRule[] = [];

  for (const node of graph.nodes) {
    warnings.push(...normalizeInvalidConstraints(node));
    rules.push(buildResponsiveRule(node));
  }

  return { rules, warnings };
}
