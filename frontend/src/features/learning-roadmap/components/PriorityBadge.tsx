import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { PRIORITY_FULL_LABEL, PRIORITY_LABEL, type Priority } from "@/features/learning-roadmap/lib/priority";

const PRIORITY_CLASS: Record<Priority, string> = {
  high: "bg-red-100 text-red-700 dark:bg-red-500/15 dark:text-red-400",
  medium: "bg-amber-100 text-amber-700 dark:bg-amber-500/15 dark:text-amber-400",
  low: "bg-slate-100 text-slate-600 dark:bg-slate-500/15 dark:text-slate-400",
};

/** 学習項目の優先度バッジ。視覚的には短い文字(高/中/低)、スクリーンリーダー向けには完全な文言を読み上げる。 */
export function PriorityBadge({ priority }: { priority: Priority }) {
  return (
    <Badge aria-label={PRIORITY_FULL_LABEL[priority]} className={cn(PRIORITY_CLASS[priority])}>
      {PRIORITY_LABEL[priority]}
    </Badge>
  );
}
