const dateTimeFormatter = new Intl.DateTimeFormat("ja-JP", {
  year: "numeric",
  month: "2-digit",
  day: "2-digit",
  hour: "2-digit",
  minute: "2-digit",
});

/** ISO日時文字列を「YYYY/MM/DD HH:mm」形式に整形する。一覧・詳細系の画面で使い回す。 */
export function formatDateTime(isoString: string | null | undefined): string {
  if (!isoString) return "—";
  return dateTimeFormatter.format(new Date(isoString));
}
