namespace SkillPilot.Application.Common.Dtos;

/// <summary>ページング済みの一覧結果。</summary>
/// <typeparam name="T">要素の型。</typeparam>
/// <param name="Items">現在のページの要素一覧。</param>
/// <param name="Page">現在のページ番号(1始まり)。</param>
/// <param name="PageSize">1ページあたりの件数。</param>
/// <param name="TotalCount">全体の件数。</param>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
