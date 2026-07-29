import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";

interface SimplePaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

/** ページ番号を出さず前へ/次へのみで送るシンプルなページネーション。一覧系の画面で使い回す。 */
export function SimplePagination({ page, totalPages, onPageChange }: SimplePaginationProps) {
  if (totalPages <= 1) return null;

  return (
    <nav aria-label="ページ送り" className="flex items-center justify-center gap-3">
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(page - 1)}
        disabled={page <= 1}
        aria-label="前のページ"
      >
        <ChevronLeft />
        前へ
      </Button>
      <span className="text-sm text-muted-foreground tabular-nums">
        {page} / {totalPages} ページ
      </span>
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(page + 1)}
        disabled={page >= totalPages}
        aria-label="次のページ"
      >
        次へ
        <ChevronRight />
      </Button>
    </nav>
  );
}
