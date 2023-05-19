using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

#pragma warning disable CS8618
internal class Detail
{
    [JsonPropertyName("downloads")]
    public Downloads Downloads { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("javaVersion")]
    public JavaVersion JavaVersion { get; set; }

    [JsonPropertyName("mainClass")]
    public string MainClass { get; set; }

    [JsonPropertyName("minimumLauncherVersion")]
    public int MinimumLauncherVersion { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }    
}
#pragma warning restore CS8618