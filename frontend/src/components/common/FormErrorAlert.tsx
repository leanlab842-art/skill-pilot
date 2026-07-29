/** フォーム送信時のサーバーエラー(認証失敗・重複登録等)を表示する共通バナー。 */
export function FormErrorAlert({ message }: { message: string }) {
  return (
    <div
      role="alert"
      className="rounded-lg border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive"
    >
      {message}
    </div>
  );
}
