import type { ReactNode } from "react";
import { Compass } from "lucide-react";
import { cn } from "@/lib/utils";

function Logo({ size = "default" }: { size?: "default" | "sm" }) {
  return (
    <div className="flex items-center gap-2">
      <span
        className={cn(
          "flex items-center justify-center rounded-xl bg-primary text-primary-foreground",
          size === "default" ? "size-10" : "size-8",
        )}
      >
        <Compass className={size === "default" ? "size-5" : "size-4"} />
      </span>
      <span
        translate="no"
        className={cn("font-bold text-slate-900", size === "default" ? "text-2xl" : "text-xl")}
      >
        SkillPilot
      </span>
    </div>
  );
}

/**
 * ログイン・アカウント作成画面で共有するレイアウト。
 * デスクトップ(lg以上)は左にブランディングパネル、右にフォームカードを配置する2カラム構成。
 * タブレット・スマホでは左パネルを省略し、フォームの上に簡易ロゴだけを表示する。
 */
export function AuthLayout({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-screen">
      <div className="relative hidden w-1/2 items-center overflow-hidden bg-slate-50 px-16 lg:flex">
        <div
          aria-hidden="true"
          className="absolute -top-24 -left-24 size-96 rounded-full bg-blue-100/60 blur-3xl"
        />
        <div
          aria-hidden="true"
          className="absolute -bottom-32 left-1/3 size-96 rounded-full bg-blue-50 blur-3xl"
        />
        <div className="relative z-10 max-w-md">
          <Logo />
          <p className="mt-1 text-sm font-medium text-slate-500">求人分析AI</p>
          <p className="mt-6 leading-relaxed text-slate-600">
            AIがあなたのスキルを分析し、理想のキャリアを実現するための学習ロードマップを提供します。
          </p>
        </div>
      </div>

      <div className="flex flex-1 flex-col items-center justify-center bg-white px-4 py-12 sm:px-6 lg:px-8">
        <div className="mb-8 lg:hidden">
          <Logo size="sm" />
        </div>
        <div className="w-full max-w-sm">{children}</div>
      </div>
    </div>
  );
}
