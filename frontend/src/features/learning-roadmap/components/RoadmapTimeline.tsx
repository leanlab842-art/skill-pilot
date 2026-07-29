import { RoadmapItemRow } from "@/features/learning-roadmap/components/RoadmapItemRow";
import { derivePriority } from "@/features/learning-roadmap/lib/priority";
import type { LearningRoadmapDto, SkillResultDto } from "@/api/generated/models";

interface RoadmapTimelineProps {
  analysisId: string;
  roadmap: LearningRoadmapDto[];
  skillResults: SkillResultDto[];
}

/** 学習ロードマップをWeekごとにグループ化し、タイムライン形式で表示する。 */
export function RoadmapTimeline({ analysisId, roadmap, skillResults }: RoadmapTimelineProps) {
  const weeks = new Map<number, LearningRoadmapDto[]>();
  for (const item of roadmap) {
    const week = item.week ?? 1;
    const list = weeks.get(week) ?? [];
    list.push(item);
    weeks.set(week, list);
  }
  const sortedWeeks = Array.from(weeks.entries()).sort(([a], [b]) => a - b);

  return (
    <div className="space-y-8">
      {sortedWeeks.map(([week, items]) => (
        <section key={week} aria-labelledby={`week-${week}-heading`}>
          <div className="mb-4 flex items-center gap-3">
            <span
              className="flex size-8 shrink-0 items-center justify-center rounded-full bg-primary text-xs font-semibold text-primary-foreground"
              aria-hidden="true"
            >
              {week}
            </span>
            <h2 id={`week-${week}-heading`} className="text-sm font-semibold text-foreground">
              Week {week}
            </h2>
          </div>
          <div className="ml-4 space-y-4 border-l border-border py-1 pl-6">
            {items.map((item) => {
              const skill = skillResults.find((s) => s.id === item.skillResultId);
              return (
                <RoadmapItemRow
                  key={item.id}
                  analysisId={analysisId}
                  item={item}
                  priority={derivePriority(item, skillResults)}
                  relatedSkillName={skill?.skillName}
                />
              );
            })}
          </div>
        </section>
      ))}
    </div>
  );
}
