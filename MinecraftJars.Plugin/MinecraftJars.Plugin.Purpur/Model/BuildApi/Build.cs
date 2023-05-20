using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Purpur.Model.BuildApi;

internal class Build
{
    [JsonPropertyName("build")]
    public string BuildId { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }

    [JsonPropertyName("project")]
    public string? Project { get; set; }

    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }    
}