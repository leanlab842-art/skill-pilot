import { Link, useParams } from "react-router-dom";
import { ArrowLeft, ListChecks } from "lucide-react";
import { useGetApiV1AnalysesId } from "@/api/generated/job-analysis/job-analysis";
import { PageHeader } from "@/components/common/PageHeader";
import { ErrorState } from "@/components/common/ErrorState";
import { EmptyState } from "@/components/common/EmptyState";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { AnalysisStatusGate } from "@/features/job-analysis/components/AnalysisStatusGate";
import { RoadmapProgressCard } from "@/features/learning-roadmap/components/RoadmapProgressCard";
import { RoadmapTimeline } from "@/features/learning-roadmap/components/RoadmapTimeline";
import { ApiError } from "@/lib/apiClient";

export function LearningRoadmapPage() {
  const { id } = useParams<{ id: string }>();

  const { data, isPending, isError, error, refetch } = useGetApiV1AnalysesId(id ?? "");

  if (isPending) {
    return (
      <div className="mx-auto flex max-w-3xl flex-col gap-6" aria-busy="true" aria-label="読み込み中">
        <div className="space-y-2">
          <Skeleton className="h-4 w-32" />
          <Skeleton className="h-7 w-56" />
        </div>
        <Skeleton className="h-20 w-full rounded-xl" />
        <Card>
          <CardContent className="space-y-6 py-6">
            <div className="space-y-4">
              <Skeleton className="h-4 w-24" />
              <Skeleton className="h-16 w-full" />
              <Skeleton className="h-16 w-full" />
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (isError) {
    const notFound = error instanceof ApiError && error.status === 404;
    return (
      <div className="mx-auto max-w-3xl">
        <ErrorState
          title={notFound ? "求人分析が見つかりません" : "学習ロードマップの取得に失敗しました"}
          description={error instanceof ApiError ? error.message : "時間をおいて再度お試しください。"}
          onRetry={notFound ? undefined : () => void refetch()}
          action={
            notFound ? (
              <Button variant="outline" size="sm" nativeButton={false} render={<Link to="/" />}>
                ダッシュボードに戻る
              </Button>
            ) : undefined
          }
        />
      </div>
    );
  }

  const analysis = data;
  const roadmap = analysis.roadmap ?? [];
  const skillResults = analysis.skillResults ?? [];
  const completedCount = roadmap.filter((item) => item.completed).length;

  return (
    <div className="mx-auto flex max-w-3xl flex-col gap-6">
      <div className="space-y-3">
        <Link
          to={`/analyses/${analysis.id}`}
          className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="size-3.5" aria-hidden="true" />
          分析結果に戻る
        </Link>
        <PageHeader
          title="学習ロードマップ"
          description={`${analysis.companyName || "(会社名未設定)"} ・ ${analysis.jobTitle || "(求人タイトル未設定)"}`}
        />
      </div>

      <AnalysisStatusGate status={analysis.status} onRetryPending={() => void refetch()}>
        {roadmap.length === 0 ? (
          <EmptyState
            icon={ListChecks}
            title="学習ロードマップがありません"
            description="この求人には学習項目が生成されていません。"
          />
        ) : (
          <div className="flex flex-col gap-6">
            <RoadmapProgressCard completedCount={completedCount} totalCount={roadmap.length} />
            <RoadmapTimeline analysisId={analysis.id ?? ""} roadmap={roadmap} skillResults={skillResults} />
          </div>
        )}
      </AnalysisStatusGate>
    </div>
  );
}
