import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AuthLayout } from "@/features/auth/components/AuthLayout";
import { loginSchema, type LoginFormValues } from "@/features/auth/schemas/authSchemas";
import { useAuth } from "@/contexts/AuthContext";
import { ApiError } from "@/lib/apiClient";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { PasswordInput } from "@/components/common/PasswordInput";
import { FormField } from "@/components/common/FormField";
import { FormErrorAlert } from "@/components/common/FormErrorAlert";
import { SubmitButton } from "@/components/common/SubmitButton";

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const onSubmit = async (values: LoginFormValues) => {
    setFormError(null);
    try {
      await login(values.email, values.password);
      const redirectTo = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? "/";
      navigate(redirectTo, { replace: true });
    } catch (error) {
      setFormError(
        error instanceof ApiError ? error.message : "ログインに失敗しました。時間をおいて再度お試しください。",
      );
    }
  };

  return (
    <AuthLayout>
      <Card>
        <CardHeader>
          <CardTitle role="heading" aria-level={1} className="text-center text-2xl">
            ログイン
          </CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5" noValidate>
            {formError && <FormErrorAlert message={formError} />}

            <FormField id="email" label="メールアドレス" error={errors.email}>
              <Input type="email" autoComplete="email" placeholder="example@mail.com" {...register("email")} />
            </FormField>

            <FormField id="password" label="パスワード" error={errors.password}>
              <PasswordInput
                autoComplete="current-password"
                placeholder="パスワードを入力"
                {...register("password")}
              />
            </FormField>

            <SubmitButton isSubmitting={isSubmitting} size="lg" className="w-full">
              ログイン
            </SubmitButton>
          </form>
        </CardContent>
        <CardFooter className="justify-center border-t-0 bg-transparent p-0 pt-6 text-sm text-muted-foreground">
          アカウントをお持ちでない方は{" "}
          <Link to="/register" className="ml-1 font-medium text-primary hover:underline">
            新規登録
          </Link>
        </CardFooter>
      </Card>
    </AuthLayout>
  );
}
