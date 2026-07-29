using System.Text.Json.Serialization;

namespace SkillPilot.Infrastructure.Ai.Claude;

// Anthropic Messages API (POST /v1/messages) のリクエスト/レスポンスをそのまま表す型。
// Application層のJobSkillAnalysisResult等とは別物であり、Infrastructure層に閉じる。

internal sealed record MessagesRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("max_tokens")] int MaxTokens,
    [property: JsonPropertyName("messages")] IReadOnlyList<MessageRequestItem> Messages);

internal sealed record MessageRequestItem(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed record MessagesResponse(
    [property: JsonPropertyName("content")] IReadOnlyList<ContentBlock>? Content);

internal sealed record ContentBlock(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] string? Text);

// Claudeにプロンプトで出力を強制するJSONスキーマに対応するDTO。
internal sealed record AnalysisResultPayload(
    [property: JsonPropertyName("skills")] IReadOnlyList<SkillPayload>? Skills,
    [property: JsonPropertyName("roadmap")] IReadOnlyList<RoadmapItemPayload>? Roadmap);

internal sealed record SkillPayload(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("level")] string? Level,
    [property: JsonPropertyName("category")] string? Category);

internal sealed record RoadmapItemPayload(
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("week")] int Week,
    [property: JsonPropertyName("relatedSkillName")] string? RelatedSkillName);
