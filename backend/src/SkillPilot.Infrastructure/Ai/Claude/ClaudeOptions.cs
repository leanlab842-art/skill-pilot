namespace SkillPilot.Infrastructure.Ai.Claude;

/// <summary><c>appsettings.json</c>の<c>Ai:Claude</c>セクションにバインドする設定。</summary>
public sealed class ClaudeOptions
{
    public string BaseUrl { get; set; } = "https://api.anthropic.com";

    public string Model { get; set; } = string.Empty;

    /// <summary>APIキー。本番環境ではUser Secrets/環境変数から注入し、平文で保存しない。</summary>
    public string ApiKey { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;
}
