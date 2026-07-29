using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SkillPilot.Application.Abstractions.Ai;
using SkillPilot.Domain.Enums;

namespace SkillPilot.Infrastructure.Ai.Claude;

/// <summary>Claude API(Anthropic Messages API)を用いた<see cref="IJobSkillAnalyzer"/>の実装。</summary>
public sealed class ClaudeJobSkillAnalyzer : IJobSkillAnalyzer
{
    private const string AnthropicVersion = "2023-06-01";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly ClaudeOptions _options;

    public ClaudeJobSkillAnalyzer(HttpClient httpClient, IOptions<ClaudeOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<JobSkillAnalysisResult> AnalyzeAsync(string jobDescription, CancellationToken ct)
    {
        try
        {
            var request = new MessagesRequest(
                Model: _options.Model,
                MaxTokens: 2048,
                Messages: [new MessageRequestItem("user", BuildPrompt(jobDescription))]);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/messages")
            {
                Content = JsonContent.Create(request, options: JsonOptions),
            };
            httpRequest.Headers.Add("x-api-key", _options.ApiKey);
            httpRequest.Headers.Add("anthropic-version", AnthropicVersion);

            using var httpResponse = await _httpClient.SendAsync(httpRequest, ct);
            httpResponse.EnsureSuccessStatusCode();

            var messagesResponse = await httpResponse.Content.ReadFromJsonAsync<MessagesResponse>(JsonOptions, ct);
            var text = messagesResponse?.Content?.FirstOrDefault(c => c.Type == "text")?.Text
                ?? throw new AiAnalysisException("Claude APIのレスポンスにテキストが含まれていません。");

            var payload = JsonSerializer.Deserialize<AnalysisResultPayload>(ExtractJson(text), JsonOptions)
                ?? throw new AiAnalysisException("Claude APIのレスポンスをJSONとして解釈できませんでした。");

            return MapToResult(payload);
        }
        catch (AiAnalysisException)
        {
            throw;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException or NotSupportedException)
        {
            // 通信エラー・タイムアウト・パース失敗など、原因を問わずAiAnalysisExceptionにラップする。
            // 呼び出し元(JobAnalysisRunner)はこの型だけを見て「想定内の外部要因の失敗」として扱う。
            throw new AiAnalysisException("Claude APIによる求人分析に失敗しました。", ex);
        }
    }

    private static string BuildPrompt(string jobDescription) => $$"""
        あなたは求人票を分析するアシスタントです。以下の求人本文から、必要なスキルと
        学習ロードマップを抽出してください。

        # 出力形式
        説明文を含めず、次のJSONスキーマに厳密に従ったJSONのみを出力してください。
        {
          "skills": [
            { "name": string, "level": "Beginner"|"Intermediate"|"Advanced", "category": "Required"|"Preferred" }
          ],
          "roadmap": [
            { "title": string, "description": string | null, "week": number, "relatedSkillName": string | null }
          ]
        }

        # 求人本文
        {{jobDescription}}
        """;

    // Claudeが前後に説明文を付けてしまった場合に備え、最初の"{"から最後の"}"までを取り出す。
    private static string ExtractJson(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start < 0 || end < start)
            throw new AiAnalysisException("Claude APIのレスポンスからJSONを抽出できませんでした。");

        return text[start..(end + 1)];
    }

    private static JobSkillAnalysisResult MapToResult(AnalysisResultPayload payload)
    {
        var skills = (payload.Skills ?? [])
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .Select(s => new ExtractedSkill(
                s.Name!,
                // 未知の値だった場合、Levelは最も無難な初級に、Categoryはマッチ率算出で
                // 不利にならないよう歓迎(Preferred)側に倒す(必須(Required)と誤判定しない)。
                ParseEnum<SkillLevel>(s.Level) ?? SkillLevel.Beginner,
                ParseEnum<SkillCategory>(s.Category) ?? SkillCategory.Preferred))
            .ToList();

        var roadmap = (payload.Roadmap ?? [])
            .Where(r => !string.IsNullOrWhiteSpace(r.Title) && r.Week >= 1)
            .Select(r => new SuggestedRoadmapItem(r.Title!, r.Description, r.Week, r.RelatedSkillName))
            .ToList();

        return new JobSkillAnalysisResult(skills, roadmap);
    }

    private static TEnum? ParseEnum<TEnum>(string? value) where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : null;
}
