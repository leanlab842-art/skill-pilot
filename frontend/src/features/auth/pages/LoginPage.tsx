import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { AuthLayout } from "@/features/auth/components/AuthLayout";
import { loginSchema, type LoginFormValues } from "@/features/auth/schemas/authSchemas";
import { useAuth } from "@/contexts/AuthContext";
import { ApiError } from "@/lib/apiClient";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PasswordInput } from "@/components/common/PasswordInput";

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
            {formError && (
              <div
                role="alert"
                className="rounded-lg border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive"
              >
                {formError}
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="email">メールアドレス</Label>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                placeholder="example@mail.com"
                aria-invalid={!!errors.email}
                aria-describedby={errors.email ? "email-error" : undefined}
                {...register("email")}
              />
              {errors.email && (
                <p id="email-error" role="alert" className="text-sm text-destructive">
                  {errors.email.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">パスワード</Label>
              <PasswordInput
                id="password"
                autoComplete="current-password"
                placeholder="パスワードを入力"
                aria-invalid={!!errors.password}
                aria-describedby={errors.password ? "password-error" : undefined}
                {...register("password")}
              />
              {errors.password && (
                <p id="password-error" role="alert" className="text-sm text-destructive">
                  {errors.password.message}
                </p>
              )}
            </div>

            <Button type="submit" size="lg" className="w-full" disabled={isSubmitting}>
              {/* Base UIのButtonは子要素の増減とDOM操作が競合しクラッシュすることがあるため、
                  アイコンは常時マウントしCSSで表示を切り替える(条件付きレンダリングを避ける)。 */}
              <Loader2 aria-hidden="true" className={cn("size-4 animate-spin", !isSubmitting && "hidden")} />
              <span>ログイン</span>
            </Button>
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
