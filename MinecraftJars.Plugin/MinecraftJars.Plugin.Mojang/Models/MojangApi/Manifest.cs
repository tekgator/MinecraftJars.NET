using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.MojangApi;

internal class Manifest
{
    [JsonPropertyName("latest")]
    public Latest Latest { get; set; } = new();

    [JsonPropertyName("versions")] 
    public List<Version> Versions { get; set; } = new();
}