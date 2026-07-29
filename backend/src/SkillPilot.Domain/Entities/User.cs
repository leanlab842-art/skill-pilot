using SkillPilot.Domain.Common;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Entities;

/// <summary>
/// アプリケーションの利用者。ログイン認証の主体であり、保有スキル(<see cref="UserSkill"/>)と
/// 求人分析(<see cref="JobAnalysis"/>)を所有する。
/// </summary>
public sealed class User : BaseEntity
{
    /// <summary>表示名。</summary>
    public string Name { get; private set; } = null!;

    /// <summary>ログインID(メールアドレス)。</summary>
    public Email Email { get; private set; } = null!;

    /// <summary>ハッシュ化済みパスワード。生のパスワードは保持しない。</summary>
    public HashedPassword PasswordHash { get; private set; } = null!;

    private readonly List<UserSkill> _skills = [];

    /// <summary>保有スキルの一覧。</summary>
    public IReadOnlyCollection<UserSkill> Skills => _skills.AsReadOnly();

    private readonly List<JobAnalysis> _jobAnalyses = [];

    /// <summary>これまでに登録した求人分析の一覧。</summary>
    public IReadOnlyCollection<JobAnalysis> JobAnalyses => _jobAnalyses.AsReadOnly();

    // EF Coreがエンティティを復元する際に使用する。アプリケーションコードから直接呼び出さない。
    private User()
    {
    }

    /// <summary>新規ユーザーを作成する。</summary>
    /// <param name="name">表示名。</param>
    /// <param name="email">ログインID(メールアドレス)。</param>
    /// <param name="passwordHash">ハッシュ化済みパスワード。</param>
    public User(string name, Email email, HashedPassword passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    /// <summary>プロフィール(表示名)を更新する。</summary>
    /// <param name="name">新しい表示名。</param>
    public void UpdateProfile(string name)
    {
        Name = name;
        Touch();
    }
}
