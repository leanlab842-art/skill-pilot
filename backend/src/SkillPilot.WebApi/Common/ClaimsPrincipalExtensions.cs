using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SkillPilot.WebApi.Common;

/// <summary>認証済みユーザーのClaimsPrincipalからアプリケーション上のユーザーIdを取り出す。</summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>JWTの<c>sub</c>クレームからユーザーIdを取得する。</summary>
    /// <exception cref="InvalidOperationException">
    /// subクレームが存在しない場合(このメソッドは[Authorize]配下でのみ呼び出す前提のため、
    /// 到達した場合は認証設定側の不具合を疑う)。
    /// </exception>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var subject = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("認証済みユーザーのsubクレームが見つかりません。");

        return Guid.Parse(subject);
    }
}
