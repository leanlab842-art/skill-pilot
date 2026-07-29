export type MatchRateTier = "good" | "warn" | "bad";

/** マッチ率の色分け閾値。ダッシュボードの一覧・分析結果画面のゲージで共通利用する。 */
export function getMatchRateTier(rate: number): MatchRateTier {
  if (rate >= 80) return "good";
  if (rate >= 60) return "warn";
  return "bad";
}

const TEXT_CLASS: Record<MatchRateTier, string> = {
  good: "text-emerald-600 dark:text-emerald-400",
  warn: "text-amber-600 dark:text-amber-400",
  bad: "text-red-600 dark:text-red-400",
};

const STROKE_CLASS: Record<MatchRateTier, string> = {
  good: "stroke-emerald-500",
  warn: "stroke-amber-500",
  bad: "stroke-red-500",
};

export function getMatchRateTextClass(rate: number): string {
  return TEXT_CLASS[getMatchRateTier(rate)];
}

export function getMatchRateStrokeClass(rate: number): string {
  return STROKE_CLASS[getMatchRateTier(rate)];
}
