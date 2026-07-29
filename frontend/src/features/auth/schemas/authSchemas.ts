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

// アカウント作成時は、新規パスワードとして最低文字数を課す(バックエンドはv1では
// パスワード強度を検証しないため、これはあくまでクライアント側のUX上の目安)。
export const registerSchema = z
  .object({
    name: z
      .string()
      .min(1, "お名前を入力してください")
      .max(100, "お名前は100文字以内で入力してください"),
    email: emailSchema,
    password: z.string().min(8, "パスワードは8文字以上で入力してください"),
    confirmPassword: z.string().min(1, "確認用のパスワードを入力してください"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "パスワードが一致しません",
    path: ["confirmPassword"],
  });

export type RegisterFormValues = z.infer<typeof registerSchema>;
