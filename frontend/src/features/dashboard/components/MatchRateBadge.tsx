import { getMatchRateTextClass } from "@/lib/matchRate";
import { cn } from "@/lib/utils";

/** マッチ率を色分けして表示する。未分析(null)の場合はプレースホルダーを表示する。 */
export function MatchRateBadge({ matchRate }: { matchRate: number | null | undefined }) {
  if (matchRate === null || matchRate === undefined) {
    return <span className="text-sm text-muted-foreground">—</span>;
  }

  return (
    <span className={cn("text-sm font-semibold tabular-nums", getMatchRateTextClass(matchRate))}>
      {matchRate}%
    </span>
  );
}
