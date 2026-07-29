import { Link, useParams } from "react-router-dom";
import { Clock, ExternalLink, Inbox } from "lucide-react";
import { useGetApiV1AnalysesId } from "@/api/generated/job-analysis/job-analysis";
import { PageHeader } from "@/components/common/PageHeader";
import { ErrorState } from "@/components/common/ErrorState";
import { EmptyState } from "@/components/common/EmptyState";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { MatchRateHero } from "@/features/job-analysis/components/MatchRateHero";
import { AnalysisSummaryCard } from "@/features/job-analysis/components/AnalysisSummaryCard";
import { SkillSection } from "@/features/job-analysis/components/SkillSection";
import { AnalysisStatus, SkillCategory } from "@/api/generated/models";
import { ApiError } from "@/lib/apiClient";

export function JobAnalysisResultPage() {
  const { id } = useParams<{ id: string }>();

  const { data, isPending, isError, error, refetch } = useGetApiV1AnalysesId(id ?? "");

  if (isPending) {
    return (
      <div className="mx-auto flex max-w-3xl flex-col gap-6" aria-busy="true" aria-label="読み込み中">
        <div className="space-y-2">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-7 w-72" />
        </div>
        <Card>
          <CardContent className="flex flex-col items-center gap-6 py-10">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="size-44 rounded-full" />
            <Skeleton className="h-9 w-48" />
          </CardContent>
        </Card>
        <Skeleton className="h-32 w-full rounded-xl" />
        <Skeleton className="h-24 w-full rounded-xl" />
      </div>
    );
  }

  if (isError) {
    const notFound = error instanceof ApiError && error.status === 404;
    return (
      <div className="mx-auto max-w-3xl">
        <ErrorState
          title={notFound ? "求人分析が見つかりません" : "求人分析の取得に失敗しました"}
          description={
            error instanceof ApiError ? error.message : "時間をおいて再度お試しください。"
          }
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
  const requiredSkills = (analysis.skillResults ?? []).filter((s) => s.category === SkillCategory.Required);
  const preferredSkills = (analysis.skillResults ?? []).filter((s) => s.category === SkillCategory.Preferred);
  const hasSkills = (analysis.skillResults?.length ?? 0) > 0;

  return (
    <div className="mx-auto flex max-w-3xl flex-col gap-6">
      <PageHeader
        title={analysis.jobTitle || "(求人タイトル未設定)"}
        description={
          <span className="flex flex-wrap items-center gap-1.5">
            {analysis.companyName || "(会社名未設定)"}
            {analysis.jobUrl && (
              <>
                <span aria-hidden="true">・</span>
                <a
                  href={analysis.jobUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="inline-flex items-center gap-1 text-primary hover:underline"
                >
                  求人ページを開く
                  <ExternalLink className="size-3.5" aria-hidden="true" />
                </a>
              </>
            )}
          </span>
        }
      />

      {analysis.status === AnalysisStatus.Failed ? (
        <ErrorState
          title="AI分析に失敗しました"
          description="求人本文の分析中にエラーが発生しました。お手数ですが、求人を登録し直してください。"
          action={
            <Button variant="outline" size="sm" nativeButton={false} render={<Link to="/" />}>
              ダッシュボードに戻る
            </Button>
          }
        />
      ) : analysis.status === AnalysisStatus.Pending ? (
        <EmptyState
          icon={Clock}
          title="分析待ちです"
          description="この求人はまだAI分析が完了していません。しばらくしてから再読み込みしてください。"
          action={
            <Button variant="outline" size="sm" onClick={() => void refetch()}>
              再読み込み
            </Button>
          }
        />
      ) : !hasSkills ? (
        <EmptyState
          icon={Inbox}
          title="スキル情報がありません"
          description="AIが必要スキルを抽出できませんでした。求人本文の内容をご確認ください。"
        />
      ) : (
        <>
          <MatchRateHero matchRate={analysis.matchRate ?? 0} analysisId={analysis.id ?? ""} />
          <AnalysisSummaryCard skillResults={analysis.skillResults ?? []} roadmapCount={analysis.roadmap?.length ?? 0} />
          <SkillSection title="必須スキル" skills={requiredSkills} />
          <SkillSection title="歓迎スキル" skills={preferredSkills} />
        </>
      )}
    </div>
  );
}
