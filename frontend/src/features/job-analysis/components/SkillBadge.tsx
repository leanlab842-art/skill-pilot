import { Check, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { SKILL_LEVEL_LABEL } from "@/lib/skillLevel";
import type { SkillResultDto } from "@/api/generated/models";

/** 必要スキル1件を、保有(緑)/不足(赤)で色分けしたバッジとして表示する。 */
export function SkillBadge({ skill }: { skill: SkillResultDto }) {
  const owned = !skill.isMissing;

  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full border px-3 py-1.5 text-sm font-medium",
        owned
          ? "border-emerald-200 bg-emerald-50 text-emerald-700 dark:border-emerald-500/30 dark:bg-emerald-500/10 dark:text-emerald-400"
          : "border-red-200 bg-red-50 text-red-700 dark:border-red-500/30 dark:bg-red-500/10 dark:text-red-400",
      )}
    >
      {owned ? <Check className="size-3.5" aria-hidden="true" /> : <X className="size-3.5" aria-hidden="true" />}
      <span>{skill.skillName}</span>
      {skill.level && <span className="opacity-70">({SKILL_LEVEL_LABEL[skill.level]})</span>}
      <span className="sr-only">・{owned ? "保有済み" : "不足"}</span>
    </span>
  );
}
