import { AnchorMode, AxisBehavior, CanonicalNode } from "../core/canonical-model.js";

export interface ResponsiveRule {
  nodeId: string;
  widthBehavior: AxisBehavior;
  heightBehavior: AxisBehavior;
  horizontalAnchor: AnchorMode;
  verticalAnchor: AnchorMode;
  minWidth?: number;
  maxWidth?: number;
  minHeight?: number;
  maxHeight?: number;
  layoutMode?: "NONE" | "HORIZONTAL" | "VERTICAL";
}

export function buildResponsiveRule(node: CanonicalNode): ResponsiveRule {
  return {
    nodeId: node.id,
    widthBehavior: node.constraints.widthBehavior,
    heightBehavior: node.constraints.heightBehavior,
    horizontalAnchor: node.constraints.horizontal,
    verticalAnchor: node.constraints.vertical,
    minWidth: node.constraints.minWidth,
    maxWidth: node.constraints.maxWidth,
    minHeight: node.constraints.minHeight,
    maxHeight: node.constraints.maxHeight,
    layoutMode: node.layout.layoutMode ?? "NONE",
  };
}
