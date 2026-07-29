namespace SkillPilot.Application.Common.Results;

/// <summary>
/// 戻り値を持つUseCaseの実行結果。想定内のビジネス失敗(<see cref="Error"/>)は例外ではなく
/// この型の失敗として表現する。想定外の失敗(バグ、DB接続断など)は例外のまま上位に伝播させる
/// (3層の例外設計。詳細は<c>docs/architecture.md</c>を参照)。
/// </summary>
/// <typeparam name="T">成功時に返す値の型。</typeparam>
public sealed class Result<T>
{
    /// <summary>成功したかどうか。</summary>
    public bool IsSuccess { get; }

    /// <summary>成功時の値。失敗時はdefault。</summary>
    public T Value { get; }

    /// <summary>失敗時のエラー情報。成功時はnull。</summary>
    public Error? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Value = default!;
        Error = error;
    }

    // ジェネリック型にstaticファクトリメソッドを持たせるとCA1000(呼び出し時に型引数の指定が
    // 必要で使いにくい)に抵触するため設けない。UseCase内では下記の暗黙変換だけを使い、
    // `return dto;` / `return Error.NotFound(...);` と書けるようにする。
    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}

/// <summary>戻り値を持たないUseCase(Delete等)の実行結果。</summary>
public sealed class Result
{
    /// <summary>成功したかどうか。</summary>
    public bool IsSuccess { get; }

    /// <summary>失敗時のエラー情報。成功時はnull。</summary>
    public Error? Error { get; }

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>成功結果を生成する。</summary>
    public static Result Success() => new(true, null);

    /// <summary>失敗結果を生成する。</summary>
    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}
