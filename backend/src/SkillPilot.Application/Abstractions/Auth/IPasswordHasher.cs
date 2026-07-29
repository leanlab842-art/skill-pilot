using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Abstractions.Auth;

/// <summary>パスワードのハッシュ化・検証を行う。実装(アルゴリズム選定)はInfrastructure層が担う。</summary>
public interface IPasswordHasher
{
    /// <summary>生のパスワードをハッシュ化する。</summary>
    HashedPassword Hash(string plainPassword);

    /// <summary>生のパスワードがハッシュ値と一致するか検証する。</summary>
    bool Verify(string plainPassword, HashedPassword hashedPassword);
}
