import { cn } from "@/lib/utils";

/** マッチ率を色分けして表示する。未分析(null)の場合はプレースホルダーを表示する。 */
export function MatchRateBadge({ matchRate }: { matchRate: number | null | undefined }) {
  if (matchRate === null || matchRate === undefined) {
    return <span className="text-sm text-muted-foreground">—</span>;
  }

  const colorClass =
    matchRate >= 80
      ? "text-emerald-600 dark:text-emerald-400"
      : matchRate >= 60
        ? "text-amber-600 dark:text-amber-400"
        : "text-red-600 dark:text-red-400";

  return <span className={cn("text-sm font-semibold tabular-nums", colorClass)}>{matchRate}%</span>;
}
