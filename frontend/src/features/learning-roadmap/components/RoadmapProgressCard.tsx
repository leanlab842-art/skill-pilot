import { PartyPopper } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";

interface RoadmapProgressCardProps {
  completedCount: number;
  totalCount: number;
}

/** 学習ロードマップ全体の進捗を進捗バーで表示する。全項目完了時はお祝いメッセージを添える。 */
export function RoadmapProgressCard({ completedCount, totalCount }: RoadmapProgressCardProps) {
  const percentage = totalCount === 0 ? 0 : Math.round((completedCount / totalCount) * 100);
  const isComplete = totalCount > 0 && completedCount === totalCount;

  return (
    <Card>
      <CardContent className="space-y-4">
        <div className="space-y-2">
          <div className="flex items-baseline justify-between">
            <span className="text-sm font-medium text-foreground">学習の進捗</span>
            <span className="text-sm text-muted-foreground tabular-nums">
              {completedCount} / {totalCount} 完了({percentage}%)
            </span>
          </div>
          <Progress value={percentage} aria-label="学習ロードマップの進捗" />
        </div>
        {isComplete && (
          <div className="flex items-center gap-2 rounded-lg bg-emerald-50 px-3 py-2 text-sm font-medium text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-400">
            <PartyPopper className="size-4 shrink-0" aria-hidden="true" />
            すべての学習項目が完了しました!お疲れさまでした。
          </div>
        )}
      </CardContent>
    </Card>
  );
}
