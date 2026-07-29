using Microsoft.AspNetCore.Mvc;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Profile.AddUserSkill;
using SkillPilot.Application.Profile.DeleteUserSkill;
using SkillPilot.Application.Profile.GetUserSkills;
using SkillPilot.Application.Profile.UpdateUserSkill;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Controllers;

/// <summary>自分の保有スキルを扱う。</summary>
[ApiController]
[Route("api/v1/users/me/skills")]
public sealed class UserSkillController : ControllerBase
{
    private readonly IGetUserSkillsUseCase _getUserSkillsUseCase;
    private readonly IAddUserSkillUseCase _addUserSkillUseCase;
    private readonly IUpdateUserSkillUseCase _updateUserSkillUseCase;
    private readonly IDeleteUserSkillUseCase _deleteUserSkillUseCase;

    public UserSkillController(
        IGetUserSkillsUseCase getUserSkillsUseCase,
        IAddUserSkillUseCase addUserSkillUseCase,
        IUpdateUserSkillUseCase updateUserSkillUseCase,
        IDeleteUserSkillUseCase deleteUserSkillUseCase)
    {
        _getUserSkillsUseCase = getUserSkillsUseCase;
        _addUserSkillUseCase = addUserSkillUseCase;
        _updateUserSkillUseCase = updateUserSkillUseCase;
        _deleteUserSkillUseCase = deleteUserSkillUseCase;
    }

    /// <summary>自分の保有スキル一覧を取得する。</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserSkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        var result = await _getUserSkillsUseCase.ExecuteAsync(User.GetUserId(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>保有スキルを登録する。</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserSkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add([FromBody] AddUserSkillRequest request, CancellationToken ct)
    {
        var result = await _addUserSkillUseCase.ExecuteAsync(User.GetUserId(), request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>保有スキルの習熟レベルを更新する。</summary>
    [HttpPut("{skillId:guid}")]
    [ProducesResponseType(typeof(UserSkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid skillId, [FromBody] UpdateUserSkillRequest request, CancellationToken ct)
    {
        var result = await _updateUserSkillUseCase.ExecuteAsync(User.GetUserId(), skillId, request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>保有スキルを削除する。</summary>
    [HttpDelete("{skillId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid skillId, CancellationToken ct)
    {
        var result = await _deleteUserSkillUseCase.ExecuteAsync(User.GetUserId(), skillId, ct);
        return result.ToActionResult(this);
    }
}
