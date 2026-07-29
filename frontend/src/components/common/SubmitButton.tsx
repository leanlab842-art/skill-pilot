import type { ComponentProps } from "react";
import { Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface SubmitButtonProps extends ComponentProps<typeof Button> {
  isSubmitting: boolean;
}

/**
 * 送信中はスピナーを表示するフォーム送信ボタン。
 *
 * ローディングアイコンを条件付きレンダリング(`{isSubmitting && <Loader2 />}`)すると、
 * Base UIのButton内部のDOM操作とReactの差分検出が競合し `insertBefore` エラーで
 * クラッシュすることがあったため、アイコンは常時マウントしCSSの`hidden`クラスで
 * 表示を切り替える。
 */
export function SubmitButton({ isSubmitting, children, className, ...props }: SubmitButtonProps) {
  return (
    <Button type="submit" disabled={isSubmitting} className={className} {...props}>
      <Loader2 aria-hidden="true" className={cn("size-4 animate-spin", !isSubmitting && "hidden")} />
      <span>{children}</span>
    </Button>
  );
}
