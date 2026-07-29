import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { SkillCategory } from "@/api/generated/models";
import type { SkillResultDto } from "@/api/generated/models";

interface AnalysisSummaryCardProps {
  skillResults: SkillResultDto[];
  roadmapCount: number;
}

/** AI分析結果の要点(必須/歓迎スキルの充足数・学習項目数)をカードで要約する。 */
export function AnalysisSummaryCard({ skillResults, roadmapCount }: AnalysisSummaryCardProps) {
  const required = skillResults.filter((s) => s.category === SkillCategory.Required);
  const preferred = skillResults.filter((s) => s.category === SkillCategory.Preferred);
  const requiredOwned = required.filter((s) => !s.isMissing).length;
  const preferredOwned = preferred.filter((s) => !s.isMissing).length;

  const stats = [
    { label: "必須スキル", value: `${requiredOwned} / ${required.length}`, unit: "件保有" },
    { label: "歓迎スキル", value: `${preferredOwned} / ${preferred.length}`, unit: "件保有" },
    { label: "学習ロードマップ", value: `${roadmapCount}`, unit: "件の学習項目" },
  ];

  return (
    <Card>
      <CardHeader>
        <CardTitle>分析サマリー</CardTitle>
      </CardHeader>
      <CardContent>
        <dl className="grid grid-cols-1 gap-6 sm:grid-cols-3">
          {stats.map((stat) => (
            <div key={stat.label}>
              <dt className="text-sm text-muted-foreground">{stat.label}</dt>
              <dd className="mt-1">
                <span className="text-2xl font-bold tabular-nums text-foreground">{stat.value}</span>
                <span className="ml-1 text-sm text-muted-foreground">{stat.unit}</span>
              </dd>
            </div>
          ))}
        </dl>
      </CardContent>
    </Card>
  );
}
