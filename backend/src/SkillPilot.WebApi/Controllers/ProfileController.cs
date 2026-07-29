using Microsoft.AspNetCore.Mvc;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Profile.GetProfile;
using SkillPilot.Application.Profile.UpdateProfile;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Controllers;

/// <summary>自分のプロフィールを扱う。</summary>
[ApiController]
[Route("api/v1/users/me")]
public sealed class ProfileController : ControllerBase
{
    private readonly IGetProfileUseCase _getProfileUseCase;
    private readonly IUpdateProfileUseCase _updateProfileUseCase;

    public ProfileController(IGetProfileUseCase getProfileUseCase, IUpdateProfileUseCase updateProfileUseCase)
    {
        _getProfileUseCase = getProfileUseCase;
        _updateProfileUseCase = updateProfileUseCase;
    }

    /// <summary>
    /// 自分のプロフィールを取得する。フロントエンドがログイン状態(セッションの有効性)を
    /// 確認する用途にも使う。
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _getProfileUseCase.ExecuteAsync(User.GetUserId(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>自分のプロフィール(表示名)を更新する。</summary>
    [HttpPut]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var result = await _updateProfileUseCase.ExecuteAsync(User.GetUserId(), request, ct);
        return result.ToActionResult(this);
    }
}
