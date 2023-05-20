using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

internal class Server
{
    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")] 
    public string Url { get; set; } = string.Empty;
}