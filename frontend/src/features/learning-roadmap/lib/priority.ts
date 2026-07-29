import { SkillCategory } from "@/api/generated/models";
import type { LearningRoadmapDto, SkillResultDto } from "@/api/generated/models";

export type Priority = "high" | "medium" | "low";

/**
 * 学習項目の優先度は、AIが直接算出したデータを持たない(バックエンドに優先度フィールドが
 * 存在しない)。ダミー値を表示しない方針のため、紐づく必要スキルの区分(必須/歓迎)という
 * 既存の実データから決定的に導出する: 必須スキルに対応する項目=高、歓迎スキルに対応する
 * 項目=中、どちらにも紐づかない項目=低。
 */
export function derivePriority(item: LearningRoadmapDto, skillResults: SkillResultDto[]): Priority {
  const skill = skillResults.find((s) => s.id === item.skillResultId);
  if (!skill) return "low";
  return skill.category === SkillCategory.Required ? "high" : "medium";
}

export const PRIORITY_LABEL: Record<Priority, string> = {
  high: "高",
  medium: "中",
  low: "低",
};

export const PRIORITY_FULL_LABEL: Record<Priority, string> = {
  high: "優先度: 高",
  medium: "優先度: 中",
  low: "優先度: 低",
};
