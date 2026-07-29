import { Skeleton } from "@/components/ui/skeleton";

/** AnalysisListItemと同じレイアウトのローディングスケルトン。 */
export function AnalysisListItemSkeleton() {
  return (
    <div className="flex items-center gap-4 rounded-xl bg-card px-4 py-3.5 ring-1 ring-foreground/10">
      <div className="min-w-0 flex-1 space-y-2">
        <Skeleton className="h-4 w-40" />
        <Skeleton className="h-3.5 w-56" />
      </div>
      <Skeleton className="hidden h-4 w-10 shrink-0 sm:block" />
      <Skeleton className="hidden h-5 w-14 shrink-0 rounded-4xl sm:block" />
      <Skeleton className="hidden h-4 w-28 shrink-0 md:block" />
      <Skeleton className="size-4 shrink-0" />
    </div>
  );
}
