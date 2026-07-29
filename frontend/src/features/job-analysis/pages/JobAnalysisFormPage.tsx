import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Sparkles } from "lucide-react";
import { PageHeader } from "@/components/common/PageHeader";
import { FormField } from "@/components/common/FormField";
import { FormErrorAlert } from "@/components/common/FormErrorAlert";
import { SubmitButton } from "@/components/common/SubmitButton";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { postApiV1Analyses } from "@/api/generated/job-analysis/job-analysis";
import { ApiError } from "@/lib/apiClient";
import {
  jobAnalysisFormSchema,
  type JobAnalysisFormValues,
} from "@/features/job-analysis/schemas/jobAnalysisSchemas";

const JOB_DESCRIPTION_MAX_LENGTH = 20000;

export function JobAnalysisFormPage() {
  const navigate = useNavigate();
  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<JobAnalysisFormValues>({
    resolver: zodResolver(jobAnalysisFormSchema),
    defaultValues: { companyName: "", jobTitle: "", jobUrl: "", jobDescription: "" },
  });

  const jobDescriptionLength = watch("jobDescription")?.length ?? 0;

  const onSubmit = async (values: JobAnalysisFormValues) => {
    setFormError(null);
    try {
      const analysis = await postApiV1Analyses({
        companyName: values.companyName,
        jobTitle: values.jobTitle,
        jobUrl: values.jobUrl === "" ? null : values.jobUrl,
        jobDescription: values.jobDescription,
      });
      navigate(`/analyses/${analysis.id}`, { replace: true });
    } catch (error) {
      setFormError(
        error instanceof ApiError ? error.message : "求人の登録に失敗しました。時間をおいて再度お試しください。",
      );
    }
  };

  return (
    <div className="mx-auto flex max-w-2xl flex-col gap-6">
      <PageHeader title="求人を分析する" description="求人情報を入力すると、AIが必要スキルとの一致率を分析します。" />

      <Card>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5" noValidate>
            {formError && <FormErrorAlert message={formError} />}

            <fieldset disabled={isSubmitting} className="space-y-5">
              <FormField id="companyName" label="会社名" error={errors.companyName}>
                <Input autoComplete="organization" placeholder="株式会社サンプル" {...register("companyName")} />
              </FormField>

              <FormField id="jobTitle" label="求人タイトル" error={errors.jobTitle}>
                <Input placeholder="バックエンドエンジニア" {...register("jobTitle")} />
              </FormField>

              <FormField id="jobUrl" label="求人URL(任意)" error={errors.jobUrl}>
                <Input
                  type="url"
                  inputMode="url"
                  placeholder="https://example.com/jobs/123"
                  {...register("jobUrl")}
                />
              </FormField>

              <FormField id="jobDescription" label="求人本文" error={errors.jobDescription}>
                <Textarea
                  className="field-sizing-fixed min-h-56 resize-y"
                  placeholder="求人ページに掲載されている本文をそのまま貼り付けてください。"
                  maxLength={JOB_DESCRIPTION_MAX_LENGTH}
                  {...register("jobDescription")}
                />
              </FormField>
              <p className="-mt-3 text-right text-xs text-muted-foreground tabular-nums" aria-hidden="true">
                {jobDescriptionLength} / {JOB_DESCRIPTION_MAX_LENGTH}
              </p>
            </fieldset>

            {isSubmitting && (
              <p role="status" className="text-sm text-muted-foreground">
                AIが求人内容を分析しています。この処理には数十秒かかる場合があります。しばらくお待ちください。
              </p>
            )}

            <div className="flex justify-end gap-3">
              <Button type="button" variant="outline" disabled={isSubmitting} onClick={() => navigate("/")}>
                キャンセル
              </Button>
              <SubmitButton isSubmitting={isSubmitting}>
                <Sparkles />
                AI分析を開始
              </SubmitButton>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
