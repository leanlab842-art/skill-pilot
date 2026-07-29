using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillPilot.WebApi.Swagger;

/// <summary>
/// <see cref="AuthorizeAttribute"/>が付与されたアクション(またはFallbackPolicyで認証必須になる
/// アクション)にのみ、Swagger UI上で鍵アイコン(認証が必要である表示)を付ける。
/// </summary>
/// <remarks>
/// 本プロジェクトは<c>AddAuthorization</c>のFallbackPolicyで既定を認証必須にしているため、
/// <see cref="AllowAnonymousAttribute"/>が付いていないアクションはすべて認証必須とみなす。
/// </remarks>
public sealed class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (hasAllowAnonymous)
            return;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Cookie")] = [],
        });
    }
}
