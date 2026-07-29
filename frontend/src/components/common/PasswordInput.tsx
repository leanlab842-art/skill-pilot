import { useState, type ComponentProps, type Ref } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";

interface PasswordInputProps extends Omit<ComponentProps<"input">, "type"> {
  ref?: Ref<HTMLInputElement>;
}

/**
 * 表示/非表示を切り替えられるパスワード入力欄。ログイン・アカウント作成画面で共有する。
 */
export function PasswordInput({ className, ref, ...props }: PasswordInputProps) {
  const [visible, setVisible] = useState(false);

  return (
    <div className="relative">
      <Input
        ref={ref}
        type={visible ? "text" : "password"}
        className={cn("pr-9", className)}
        {...props}
      />
      <button
        type="button"
        onClick={() => setVisible((current) => !current)}
        aria-label={visible ? "パスワードを非表示にする" : "パスワードを表示する"}
        aria-pressed={visible}
        className="absolute inset-y-0 right-0 flex items-center rounded-r-lg px-2.5 text-muted-foreground outline-none hover:text-foreground focus-visible:ring-3 focus-visible:ring-ring/50"
      >
        {visible ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
      </button>
    </div>
  );
}
