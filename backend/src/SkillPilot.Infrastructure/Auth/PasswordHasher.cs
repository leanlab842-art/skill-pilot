using System.Security.Cryptography;
using SkillPilot.Application.Abstractions.Auth;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Auth;

/// <summary>
/// PBKDF2(HMACSHA256)によるパスワードハッシュ化。
/// </summary>
/// <remarks>
/// ASP.NET Core Identityの<c>PasswordHasher&lt;TUser&gt;</c>と同じアルゴリズムを採用しているが、
/// 本プロジェクトはIdentityのUserManager基盤を使わないカスタム認証のため、
/// Identityパッケージを追加せず<see cref="Rfc2898DeriveBytes"/>で直接実装している。
/// </remarks>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSizeBytes = 16; // 128bit
    private const int SubkeySizeBytes = 32; // 256bit
    private const int Iterations = 100_000;

    /// <inheritdoc />
    public HashedPassword Hash(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, Iterations, HashAlgorithmName.SHA256, SubkeySizeBytes);

        // "反復回数.salt.hash" の形式でエンコードする。反復回数を保存しておくことで、
        // 将来Iterationsの値を引き上げても、過去に発行済みのハッシュを検証し続けられる。
        var encoded = $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";

        return HashedPassword.Create(encoded);
    }

    /// <inheritdoc />
    public bool Verify(string plainPassword, HashedPassword hashedPassword)
    {
        var parts = hashedPassword.Value.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        byte[] salt;
        byte[] expectedSubkey;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expectedSubkey = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(plainPassword, salt, iterations, HashAlgorithmName.SHA256, expectedSubkey.Length);

        // タイミング攻撃(比較にかかる時間差から正解を推測される攻撃)を防ぐため、
        // 通常の配列比較(SequenceEqual等)ではなく定数時間比較を使う。
        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }
}
