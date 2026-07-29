namespace SkillPilot.Application.Abstractions.Auth;

/// <summary>ログイン成功時にJWTアクセストークンを発行する。</summary>
public interface IJwtTokenGenerator
{
    /// <summary>指定したユーザーIdを主体とするアクセストークンを生成する。</summary>
    /// <param name="userId">トークンの主体となるユーザーId。</param>
    string Generate(Guid userId);
}
