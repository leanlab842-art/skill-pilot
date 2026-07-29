import { useState } from "react";
import { Link } from "react-router-dom";
import { Plus, SearchX } from "lucide-react";
import { useGetApiV1Analyses } from "@/api/generated/job-analysis/job-analysis";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/common/EmptyState";
import { ErrorState } from "@/components/common/ErrorState";
import { SimplePagination } from "@/components/common/SimplePagination";
import { AnalysisListItem } from "@/features/dashboard/components/AnalysisListItem";
import { AnalysisListItemSkeleton } from "@/features/dashboard/components/AnalysisListItemSkeleton";

const PAGE_SIZE = 10;

export function DashboardPage() {
  const [page, setPage] = useState(1);

  const { data, isPending, isError, refetch, isFetching } = useGetApiV1Analyses({
    page,
    pageSize: PAGE_SIZE,
  });

  const items = data?.items ?? [];
  const totalCount = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  return (
    <div className="mx-auto flex max-w-4xl flex-col gap-6">
      <header className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-xl font-semibold text-foreground">求人分析履歴</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            これまでにAI分析した求人の一覧です。
          </p>
        </div>
        <Button nativeButton={false} render={<Link to="/analyses/new" />}>
          <Plus />
          新しく分析する
        </Button>
      </header>

      {isPending ? (
        <div className="space-y-2" aria-busy="true" aria-label="読み込み中">
          {Array.from({ length: 5 }).map((_, i) => (
            <AnalysisListItemSkeleton key={i} />
          ))}
        </div>
      ) : isError ? (
        <ErrorState
          title="分析履歴の取得に失敗しました"
          description="時間をおいて再度お試しください。"
          onRetry={() => void refetch()}
        />
      ) : items.length === 0 ? (
        <EmptyState
          icon={SearchX}
          title="まだ分析履歴がありません"
          description="求人を登録してAI分析を始めましょう。"
          action={
            <Button nativeButton={false} render={<Link to="/analyses/new" />} size="sm">
              <Plus />
              求人を分析する
            </Button>
          }
        />
      ) : (
        <div className="space-y-4">
          <div className="space-y-2" aria-busy={isFetching}>
            {items.map((analysis) => (
              <AnalysisListItem key={analysis.id} analysis={analysis} />
            ))}
          </div>
          <SimplePagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      )}
    </div>
  );
}
