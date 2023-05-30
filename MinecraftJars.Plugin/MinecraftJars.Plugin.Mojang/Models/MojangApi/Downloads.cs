using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.MojangApi;

internal class Downloads
{
    [JsonPropertyName("server")]
    public Server? Server { get; set; }
}