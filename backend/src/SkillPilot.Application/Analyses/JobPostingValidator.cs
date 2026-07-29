using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses;

/// <summary>
/// 求人登録・編集で共通の入力検証。<see cref="CreateJobAnalysis.CreateJobAnalysisUseCase"/>と
/// <see cref="UpdateJobAnalysis.UpdateJobAnalysisUseCase"/>の両方から利用される検証ロジックのため、
/// 重複回避のため専用クラスとして切り出している(<see cref="JobAnalysisRunner"/>と同様の方針)。
/// 上限文字数はDBカラム定義(CompanyName/JobTitle: varchar(200), JobUrl: varchar(2048)。
/// <c>SkillPilotDbContext</c>のEntityTypeConfiguration参照)と一致させ、DB例外による
/// 予期しない500エラーを防ぐ。
/// </summary>
internal static class JobPostingValidator
{
    private const int NameMaxLength = 200;
    private const int JobUrlMaxLength = 2048;

    /// <summary>入力が不正な場合はエラーを返す。問題なければnull。</summary>
    public static Error? Validate(string companyName, string jobTitle, string? jobUrl, string jobDescription)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Error.Validation("COMPANY_NAME_REQUIRED", "会社名は必須です。");
        if (companyName.Length > NameMaxLength)
            return Error.Validation("COMPANY_NAME_TOO_LONG", $"会社名は{NameMaxLength}文字以内で入力してください。");

        if (string.IsNullOrWhiteSpace(jobTitle))
            return Error.Validation("JOB_TITLE_REQUIRED", "求人タイトルは必須です。");
        if (jobTitle.Length > NameMaxLength)
            return Error.Validation("JOB_TITLE_TOO_LONG", $"求人タイトルは{NameMaxLength}文字以内で入力してください。");

        if (jobUrl is { Length: > JobUrlMaxLength })
            return Error.Validation("JOB_URL_TOO_LONG", $"求人URLは{JobUrlMaxLength}文字以内で入力してください。");

        if (string.IsNullOrWhiteSpace(jobDescription))
            return Error.Validation("JOB_DESCRIPTION_REQUIRED", "求人本文は必須です。");

        return null;
    }
}
