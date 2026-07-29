import type { ReactNode } from "react";
import { AlertCircle, RotateCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface ErrorStateProps {
  title?: string;
  description?: string;
  onRetry?: () => void;
  /** onRetryの代わりに表示するアクション(例: 404で「一覧に戻る」リンクを出す場合)。 */
  action?: ReactNode;
  className?: string;
}

/** データ取得に失敗したときに表示する共通のエラー状態。一覧・詳細系の画面で使い回す。 */
export function ErrorState({
  title = "データの取得に失敗しました",
  description = "時間をおいて再度お試しください。",
  onRetry,
  action,
  className,
}: ErrorStateProps) {
  return (
    <div
      role="alert"
      className={cn(
        "flex flex-col items-center justify-center gap-3 rounded-xl border border-destructive/30 bg-destructive/5 px-6 py-16 text-center",
        className,
      )}
    >
      <div className="flex size-12 items-center justify-center rounded-full bg-destructive/10">
        <AlertCircle className="size-6 text-destructive" />
      </div>
      <div className="space-y-1">
        <p className="text-sm font-medium text-foreground">{title}</p>
        <p className="text-sm text-muted-foreground">{description}</p>
      </div>
      {action ??
        (onRetry && (
          <Button variant="outline" size="sm" onClick={onRetry}>
            <RotateCw />
            再読み込み
          </Button>
        ))}
    </div>
  );
}
