using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.ManifestApi;

#pragma warning disable CS8618
internal class Latest
{
    [JsonPropertyName("release")]
    public string Release { get; set; }

    [JsonPropertyName("snapshot")]
    public string Snapshot { get; set; }    
}
#pragma warning restore CS8618