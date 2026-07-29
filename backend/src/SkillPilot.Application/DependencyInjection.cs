using Microsoft.Extensions.DependencyInjection;
using SkillPilot.Application.Analyses;
using SkillPilot.Application.Analyses.CompleteRoadmapItem;
using SkillPilot.Application.Analyses.CreateJobAnalysis;
using SkillPilot.Application.Analyses.DeleteJobAnalysis;
using SkillPilot.Application.Analyses.GetJobAnalyses;
using SkillPilot.Application.Analyses.GetJobAnalysisDetail;
using SkillPilot.Application.Analyses.UpdateJobAnalysis;
using SkillPilot.Application.Auth.Login;
using SkillPilot.Application.Auth.Register;
using SkillPilot.Application.Profile.AddUserSkill;
using SkillPilot.Application.Profile.DeleteUserSkill;
using SkillPilot.Application.Profile.GetProfile;
using SkillPilot.Application.Profile.GetUserSkills;
using SkillPilot.Application.Profile.UpdateProfile;
using SkillPilot.Application.Profile.UpdateUserSkill;

namespace SkillPilot.Application;

/// <summary>Application層のサービスをDIコンテナへ登録する。</summary>
/// <remarks>
/// 規約ベースのアセンブリスキャンは「何が登録されているか」がコードから追えなくなるため使わず、
/// UseCaseを1つずつ明示的に列挙する。
/// </remarks>
public static class DependencyInjection
{
    /// <summary>Application層のUseCase・共通サービスを登録する。</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();

        // Profile
        services.AddScoped<IGetProfileUseCase, GetProfileUseCase>();
        services.AddScoped<IUpdateProfileUseCase, UpdateProfileUseCase>();
        services.AddScoped<IGetUserSkillsUseCase, GetUserSkillsUseCase>();
        services.AddScoped<IAddUserSkillUseCase, AddUserSkillUseCase>();
        services.AddScoped<IUpdateUserSkillUseCase, UpdateUserSkillUseCase>();
        services.AddScoped<IDeleteUserSkillUseCase, DeleteUserSkillUseCase>();

        // Analyses
        services.AddScoped<JobAnalysisRunner>();
        services.AddScoped<IGetJobAnalysesUseCase, GetJobAnalysesUseCase>();
        services.AddScoped<ICreateJobAnalysisUseCase, CreateJobAnalysisUseCase>();
        services.AddScoped<IGetJobAnalysisDetailUseCase, GetJobAnalysisDetailUseCase>();
        services.AddScoped<IUpdateJobAnalysisUseCase, UpdateJobAnalysisUseCase>();
        services.AddScoped<IDeleteJobAnalysisUseCase, DeleteJobAnalysisUseCase>();
        services.AddScoped<ICompleteRoadmapItemUseCase, CompleteRoadmapItemUseCase>();

        return services;
    }
}
