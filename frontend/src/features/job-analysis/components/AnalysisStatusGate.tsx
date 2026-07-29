import type { ReactNode } from "react";
import { Link } from "react-router-dom";
import { Clock } from "lucide-react";
import { ErrorState } from "@/components/common/ErrorState";
import { EmptyState } from "@/components/common/EmptyState";
import { Button } from "@/components/ui/button";
import { AnalysisStatus } from "@/api/generated/models";

interface AnalysisStatusGateProps {
  status: AnalysisStatus | undefined;
  onRetryPending: () => void;
  children: ReactNode;
}

/**
 * 求人分析の状態(Pending/Failed)に応じた共通の案内表示。分析結果画面・学習ロードマップ画面の
 * 両方で必要なため切り出している。Completedの場合のみchildren(各画面固有の内容)を表示する。
 */
export function AnalysisStatusGate({ status, onRetryPending, children }: AnalysisStatusGateProps) {
  if (status === AnalysisStatus.Failed) {
    return (
      <ErrorState
        title="AI分析に失敗しました"
        description="求人本文の分析中にエラーが発生しました。お手数ですが、求人を登録し直してください。"
        action={
          <Button variant="outline" size="sm" nativeButton={false} render={<Link to="/" />}>
            ダッシュボードに戻る
          </Button>
        }
      />
    );
  }

  if (status === AnalysisStatus.Pending) {
    return (
      <EmptyState
        icon={Clock}
        title="分析待ちです"
        description="この求人はまだAI分析が完了していません。しばらくしてから再読み込みしてください。"
        action={
          <Button variant="outline" size="sm" onClick={onRetryPending}>
            再読み込み
          </Button>
        }
      />
    );
  }

  return <>{children}</>;
}
