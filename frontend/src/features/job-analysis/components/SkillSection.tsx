import { SkillBadge } from "@/features/job-analysis/components/SkillBadge";
import type { SkillResultDto } from "@/api/generated/models";

interface SkillSectionProps {
  title: string;
  skills: SkillResultDto[];
}

/** 必須/歓迎など、カテゴリごとのスキル一覧を見出し付きで表示する。 */
export function SkillSection({ title, skills }: SkillSectionProps) {
  return (
    <div className="space-y-3">
      <h2 className="text-sm font-semibold text-foreground">{title}</h2>
      {skills.length === 0 ? (
        <p className="text-sm text-muted-foreground">該当するスキルはありません。</p>
      ) : (
        <div className="flex flex-wrap gap-2">
          {skills.map((skill) => (
            <SkillBadge key={skill.id} skill={skill} />
          ))}
        </div>
      )}
    </div>
  );
}
