export interface ExporterScorecardInput {
  autoLayoutCoverage: number;
  constraintsCoverage: number;
  effectsCoverage: number;
  performanceScore: number;
  editorErgonomicsScore: number;
}

export interface ExporterScorecardResult {
  target: "uGUI" | "UI Toolkit";
  totalScore: number;
  isPrimaryCandidate: boolean;
}

export function scoreExporter(
  target: "uGUI" | "UI Toolkit",
  input: ExporterScorecardInput,
): ExporterScorecardResult {
  const total =
    input.autoLayoutCoverage * 0.3 +
    input.constraintsCoverage * 0.25 +
    input.effectsCoverage * 0.15 +
    input.performanceScore * 0.15 +
    input.editorErgonomicsScore * 0.15;

  return {
    target,
    totalScore: Number(total.toFixed(2)),
    isPrimaryCandidate: total >= 90,
  };
}
