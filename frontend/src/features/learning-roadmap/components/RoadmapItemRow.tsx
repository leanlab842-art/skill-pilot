import { useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import { PriorityBadge } from "@/features/learning-roadmap/components/PriorityBadge";
import { usePatchApiV1AnalysesAnalysisIdRoadmapRoadmapItemId } from "@/api/generated/learning-roadmap/learning-roadmap";
import { getGetApiV1AnalysesIdQueryKey } from "@/api/generated/job-analysis/job-analysis";
import type { LearningRoadmapDto } from "@/api/generated/models";
import type { Priority } from "@/features/learning-roadmap/lib/priority";
import { cn } from "@/lib/utils";

interface RoadmapItemRowProps {
  analysisId: string;
  item: LearningRoadmapDto;
  priority: Priority;
  relatedSkillName?: string | null;
}

/** 学習ロードマップの1項目。チェックボックスで完了操作を行う(完了の取り消しはAPI未対応のため不可)。 */
export function RoadmapItemRow({ analysisId, item, priority, relatedSkillName }: RoadmapItemRowProps) {
  const queryClient = useQueryClient();

  const { mutate, isPending } = usePatchApiV1AnalysesAnalysisIdRoadmapRoadmapItemId({
    mutation: {
      onSuccess: () => {
        void queryClient.invalidateQueries({ queryKey: getGetApiV1AnalysesIdQueryKey(analysisId) });
      },
      onError: () => {
        toast.error("完了状態の更新に失敗しました。時間をおいて再度お試しください。");
      },
    },
  });

  const handleCheckedChange = (checked: boolean) => {
    if (!checked || item.completed || !item.id) return;
    mutate({ analysisId, roadmapItemId: item.id });
  };

  return (
    <div className="flex items-start gap-3">
      <Checkbox
        className="mt-0.5"
        checked={item.completed}
        disabled={item.completed || isPending}
        onCheckedChange={handleCheckedChange}
        aria-label={`${item.title || "学習項目"}を完了にする`}
      />
      <div className="min-w-0 flex-1 space-y-1">
        <div className="flex flex-wrap items-center gap-2">
          <p
            className={cn(
              "text-sm font-medium text-foreground",
              item.completed && "text-muted-foreground line-through",
            )}
          >
            {item.title}
          </p>
          <PriorityBadge priority={priority} />
          {relatedSkillName && (
            <Badge variant="outline" className="font-normal text-muted-foreground">
              {relatedSkillName}
            </Badge>
          )}
        </div>
        {item.description && (
          <p className={cn("text-sm text-muted-foreground", item.completed && "line-through")}>
            {item.description}
          </p>
        )}
      </div>
    </div>
  );
}
