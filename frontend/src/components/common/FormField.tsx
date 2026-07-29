import { cloneElement, isValidElement, type ReactElement, type ReactNode } from "react";
import type { FieldError } from "react-hook-form";
import { Label } from "@/components/ui/label";

interface FormFieldProps {
  id: string;
  label: string;
  error?: FieldError;
  children: ReactNode;
}

/**
 * ラベル・入力欄・バリデーションエラーメッセージを1組にまとめた共通フォームフィールド。
 * `children` に渡した入力コンポーネント(Input/PasswordInput等)へ、id・aria-invalid・
 * aria-describedbyを自動で注入するため、フォーム画面ごとにこれらを書く重複を避けられる。
 */
export function FormField({ id, label, error, children }: FormFieldProps) {
  const errorId = error ? `${id}-error` : undefined;

  const input = isValidElement(children)
    ? cloneElement(children as ReactElement<Record<string, unknown>>, {
        id,
        "aria-invalid": !!error,
        "aria-describedby": errorId,
      })
    : children;

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      {input}
      {error && (
        <p id={errorId} role="alert" className="text-sm text-destructive">
          {error.message}
        </p>
      )}
    </div>
  );
}
