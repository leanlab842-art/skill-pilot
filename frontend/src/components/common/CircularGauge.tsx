import type { ReactNode } from "react";
import { cn } from "@/lib/utils";

interface CircularGaugeProps {
  /** 0〜100の値。 */
  value: number;
  /** アクセシビリティ用のラベル(例: "必須スキルの一致率")。 */
  label: string;
  size?: number;
  strokeWidth?: number;
  /** 進捗の線色(Tailwindのstroke-*クラス)。 */
  colorClassName?: string;
  /** 中央に表示する内容。省略時は"{value}%"を表示する。 */
  children?: ReactNode;
  className?: string;
}

/** 円形のゲージ表示。値と色は呼び出し側が自由に指定できる汎用コンポーネント。 */
export function CircularGauge({
  value,
  label,
  size = 160,
  strokeWidth = 12,
  colorClassName = "stroke-primary",
  children,
  className,
}: CircularGaugeProps) {
  const clamped = Math.min(100, Math.max(0, value));
  const radius = (size - strokeWidth) / 2;
  const circumference = 2 * Math.PI * radius;
  const offset = circumference * (1 - clamped / 100);
  const center = size / 2;

  return (
    <div
      className={cn("relative inline-flex items-center justify-center", className)}
      style={{ width: size, height: size }}
      role="img"
      aria-label={`${label} ${clamped}%`}
    >
      <svg width={size} height={size} className="-rotate-90" aria-hidden="true">
        <circle cx={center} cy={center} r={radius} strokeWidth={strokeWidth} fill="none" className="stroke-muted" />
        <circle
          cx={center}
          cy={center}
          r={radius}
          strokeWidth={strokeWidth}
          fill="none"
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={offset}
          className={cn(colorClassName, "transition-[stroke-dashoffset] duration-700 ease-out")}
        />
      </svg>
      <div className="absolute inset-0 flex items-center justify-center" aria-hidden="true">
        {children ?? <span className="text-3xl font-bold tabular-nums">{clamped}%</span>}
      </div>
    </div>
  );
}
