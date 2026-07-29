import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AuthLayout } from "@/features/auth/components/AuthLayout";
import { registerSchema, type RegisterFormValues } from "@/features/auth/schemas/authSchemas";
import { postApiV1AuthRegister } from "@/api/generated/auth/auth";
import { useAuth } from "@/contexts/AuthContext";
import { ApiError } from "@/lib/apiClient";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { PasswordInput } from "@/components/common/PasswordInput";
import { FormField } from "@/components/common/FormField";
import { FormErrorAlert } from "@/components/common/FormErrorAlert";
import { SubmitButton } from "@/components/common/SubmitButton";

export function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: { name: "", email: "", password: "", confirmPassword: "" },
  });

  const onSubmit = async (values: RegisterFormValues) => {
    setFormError(null);
    try {
      await postApiV1AuthRegister({
        name: values.name,
        email: values.email,
        password: values.password,
      });
      // 登録APIはCookieを発行しないため、そのままログインしてダッシュボードへ進める
      // (登録・ログインを2回に分けず、一度の操作で完了させるため)。
      await login(values.email, values.password);
      navigate("/", { replace: true });
    } catch (error) {
      setFormError(
        error instanceof ApiError ? error.message : "登録に失敗しました。時間をおいて再度お試しください。",
      );
    }
  };

  return (
    <AuthLayout>
      <Card>
        <CardHeader>
          <CardTitle role="heading" aria-level={1} className="text-center text-2xl">
            アカウント作成
          </CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5" noValidate>
            {formError && <FormErrorAlert message={formError} />}

            <FormField id="name" label="お名前" error={errors.name}>
              <Input autoComplete="name" placeholder="山田 太郎" {...register("name")} />
            </FormField>

            <FormField id="email" label="メールアドレス" error={errors.email}>
              <Input type="email" autoComplete="email" placeholder="example@mail.com" {...register("email")} />
            </FormField>

            <FormField id="password" label="パスワード" error={errors.password}>
              <PasswordInput
                autoComplete="new-password"
                placeholder="8文字以上のパスワード"
                {...register("password")}
              />
            </FormField>

            <FormField id="confirmPassword" label="パスワード(確認)" error={errors.confirmPassword}>
              <PasswordInput
                autoComplete="new-password"
                placeholder="パスワードを再入力"
                {...register("confirmPassword")}
              />
            </FormField>

            <SubmitButton isSubmitting={isSubmitting} size="lg" className="w-full">
              アカウント作成
            </SubmitButton>
          </form>
        </CardContent>
        <CardFooter className="justify-center border-t-0 bg-transparent p-0 pt-6 text-sm text-muted-foreground">
          アカウントをお持ちの方は{" "}
          <Link to="/login" className="ml-1 font-medium text-primary hover:underline">
            ログイン
          </Link>
        </CardFooter>
      </Card>
    </AuthLayout>
  );
}
