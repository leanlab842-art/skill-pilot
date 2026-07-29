import { z } from "zod";

// ログイン・アカウント作成の両方で使うメールアドレス/パスワードのルールを共通化する。
export const emailSchema = z
  .string()
  .min(1, "メールアドレスを入力してください")
  .email("メールアドレスの形式が正しくありません");

export const passwordSchema = z.string().min(1, "パスワードを入力してください");

export const loginSchema = z.object({
  email: emailSchema,
  password: passwordSchema,
});

export type LoginFormValues = z.infer<typeof loginSchema>;
