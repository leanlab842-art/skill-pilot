import { z } from "zod";

// DBのカラム長(CompanyName/JobTitle: varchar(200), JobUrl: varchar(2048))を超えると
// 保存時にエラーになるため、クライアント側でも同じ上限を課しておく。
export const jobAnalysisFormSchema = z.object({
  companyName: z
    .string()
    .min(1, "会社名を入力してください")
    .max(200, "会社名は200文字以内で入力してください"),
  jobTitle: z
    .string()
    .min(1, "求人タイトルを入力してください")
    .max(200, "求人タイトルは200文字以内で入力してください"),
  jobUrl: z.union([
    z.literal(""),
    z.string().max(2048, "URLは2048文字以内で入力してください").url("URLの形式が正しくありません"),
  ]),
  jobDescription: z
    .string()
    .min(30, "AIが分析できるよう、求人本文は30文字以上で入力してください")
    .max(20000, "求人本文は20000文字以内で入力してください"),
});

export type JobAnalysisFormValues = z.infer<typeof jobAnalysisFormSchema>;
