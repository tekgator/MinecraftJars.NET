using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.ManifestApi;

#pragma warning disable CS8618
internal class Manifest
{
    [JsonPropertyName("latest")]
    public Latest Latest { get; set; }

    [JsonPropertyName("versions")] 
    public List<Version> Versions { get; set; }
}
#pragma warning restore CS8618