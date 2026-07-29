import { SkillLevel } from "@/api/generated/models";

/** スキル習熟レベルの日本語表示。求人分析結果・プロフィールのスキル管理で共通利用する。 */
export const SKILL_LEVEL_LABEL: Record<SkillLevel, string> = {
  [SkillLevel.Beginner]: "初級",
  [SkillLevel.Intermediate]: "中級",
  [SkillLevel.Advanced]: "上級",
};
