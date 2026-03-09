export type CanonicalNodeType = "FRAME" | "TEXT" | "IMAGE" | "COMPONENT" | "INSTANCE";

export type AxisBehavior = "fixed" | "fill" | "hug";
export type AnchorMode = "min" | "max" | "stretch" | "center";

export interface CanonicalLayout {
  layoutMode?: "NONE" | "HORIZONTAL" | "VERTICAL";
  itemSpacing?: number;
  paddingLeft?: number;
  paddingRight?: number;
  paddingTop?: number;
  paddingBottom?: number;
}

export interface CanonicalConstraints {
  horizontal: AnchorMode;
  vertical: AnchorMode;
  widthBehavior: AxisBehavior;
  heightBehavior: AxisBehavior;
  minWidth?: number;
  maxWidth?: number;
  minHeight?: number;
  maxHeight?: number;
}

export interface CanonicalNode {
  id: string;
  name: string;
  type: CanonicalNodeType;
  parentId: string | null;
  children: string[];
  layout: CanonicalLayout;
  constraints: CanonicalConstraints;
  styleRefs: string[];
}

export interface CanonicalStyle {
  id: string;
  kind: "text" | "paint" | "effect";
  name: string;
  payload: Record<string, unknown>;
}

export interface CanonicalAsset {
  assetId: string;
  kind: "image" | "font";
  sourceUri: string;
  targetPath: string;
}

export interface CanonicalSceneGraph {
  schemaVersion: string;
  documentId: string;
  rootNodeId: string;
  nodes: CanonicalNode[];
  styles: CanonicalStyle[];
  assets: CanonicalAsset[];
}

export interface ValidationIssue {
  level: "error" | "warning" | "info";
  message: string;
  nodeId?: string;
}

export interface ValidationResult {
  ok: boolean;
  issues: ValidationIssue[];
}
