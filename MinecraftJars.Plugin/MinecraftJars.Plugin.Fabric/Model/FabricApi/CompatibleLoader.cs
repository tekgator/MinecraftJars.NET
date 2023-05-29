using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Fabric.Model.FabricApi;

public class CompatibleLoader
{
    [JsonPropertyName("loader")] 
    public Loader Loader { get; set; } = new();
}