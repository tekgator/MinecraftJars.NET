using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Mojang.Models.DetailApi;

#pragma warning disable CS8618
internal class Downloads
{
    [JsonPropertyName("server")]
    public Server? Server { get; set; }
}
#pragma warning restore CS8618