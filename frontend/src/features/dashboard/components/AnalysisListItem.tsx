import { Link } from "react-router-dom";
import { ChevronRight } from "lucide-react";
import { AnalysisStatusBadge } from "@/features/dashboard/components/AnalysisStatusBadge";
import { MatchRateBadge } from "@/features/dashboard/components/MatchRateBadge";
import { formatDateTime } from "@/lib/format";
import type { JobAnalysisSummaryDto } from "@/api/generated/models";

/** 分析履歴一覧の1行。カード全体をクリック領域として求人詳細へ遷移する。 */
export function AnalysisListItem({ analysis }: { analysis: JobAnalysisSummaryDto }) {
  return (
    <Link
      to={`/analyses/${analysis.id}`}
      className="flex items-center gap-4 rounded-xl bg-card px-4 py-3.5 ring-1 ring-foreground/10 transition-colors hover:bg-muted/50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
    >
      <div className="min-w-0 flex-1 space-y-0.5">
        <p className="truncate text-sm font-medium text-foreground">{analysis.companyName || "(会社名未設定)"}</p>
        <p className="truncate text-sm text-muted-foreground">{analysis.jobTitle || "(求人タイトル未設定)"}</p>
      </div>
      <div className="hidden w-20 shrink-0 text-right sm:block">
        <MatchRateBadge matchRate={analysis.matchRate} />
      </div>
      <div className="hidden shrink-0 sm:block">
        {analysis.status && <AnalysisStatusBadge status={analysis.status} />}
      </div>
      <div className="hidden w-36 shrink-0 text-right text-sm text-muted-foreground md:block">
        {formatDateTime(analysis.updatedAt)}
      </div>
      <ChevronRight className="size-4 shrink-0 text-muted-foreground" aria-hidden="true" />
    </Link>
  );
}
