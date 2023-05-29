using System.Text.Json.Serialization;

namespace MinecraftJars.Plugin.Fabric.Model.FabricApi;

public class Versions
{
    [JsonPropertyName("game")] 
    public List<Game> Games { get; set; } = new();

//    [JsonPropertyName("mappings")]
//    public List<Mapping> Mappings { get; set; }

//    [JsonPropertyName("intermediary")]
//    public List<Intermediary> Intermediary { get; set; }

    [JsonPropertyName("loader")]
    public List<Loader> Loaders { get; set; } = new();

    [JsonPropertyName("installer")]
    public List<Installer> Installers { get; set; } = new(); 
}