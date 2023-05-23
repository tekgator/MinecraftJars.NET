using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Spigot.Model.JenkinsApi;

internal class Build
{
    [JsonPropertyName("artifacts")] 
    public List<Artifact> Artifacts { get; set; } = new();

    [JsonPropertyName("inProgress")]
    public bool InProgress { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}