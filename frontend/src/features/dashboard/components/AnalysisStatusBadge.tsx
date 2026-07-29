import { Badge } from "@/components/ui/badge";
import { AnalysisStatus } from "@/api/generated/models";
import { cn } from "@/lib/utils";

const STATUS_LABEL: Record<AnalysisStatus, string> = {
  [AnalysisStatus.Pending]: "分析待ち",
  [AnalysisStatus.Completed]: "完了",
  [AnalysisStatus.Failed]: "失敗",
};

const STATUS_CLASS: Record<AnalysisStatus, string> = {
  [AnalysisStatus.Pending]: "bg-amber-100 text-amber-700 dark:bg-amber-500/15 dark:text-amber-400",
  [AnalysisStatus.Completed]: "bg-emerald-100 text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-400",
  [AnalysisStatus.Failed]: "bg-red-100 text-red-700 dark:bg-red-500/15 dark:text-red-400",
};

export function AnalysisStatusBadge({ status }: { status: AnalysisStatus }) {
  return <Badge className={cn(STATUS_CLASS[status])}>{STATUS_LABEL[status]}</Badge>;
}
