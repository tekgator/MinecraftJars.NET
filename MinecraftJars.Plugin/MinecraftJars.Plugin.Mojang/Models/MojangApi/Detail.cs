using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.MojangApi;

internal class Detail
{
    [JsonPropertyName("downloads")]
    public Downloads Downloads { get; set; } = new();

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("javaVersion")]
    public JavaVersion JavaVersion { get; set; } = new();

    [JsonPropertyName("mainClass")]
    public string? MainClass { get; set; }

    [JsonPropertyName("minimumLauncherVersion")]
    public int MinimumLauncherVersion { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }    
}