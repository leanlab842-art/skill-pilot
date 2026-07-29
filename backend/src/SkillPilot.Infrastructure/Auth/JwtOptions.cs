namespace SkillPilot.Infrastructure.Auth;

/// <summary><c>appsettings.json</c>の<c>Jwt</c>セクションにバインドする設定。</summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>署名鍵。本番環境ではUser Secrets/環境変数から注入し、平文で保存しない。</summary>
    public string SigningKey { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
