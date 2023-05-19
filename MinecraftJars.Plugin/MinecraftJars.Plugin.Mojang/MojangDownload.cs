using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mojang;

public class MojangDownload : IDownload
{
    public required string FileName { get; init; }
    public required long Size { get; init; }
    public int BuildId { get; init; }
    public required string Url { get; init; }
    public required HashType HashType { get; init; }
    public string? Hash { get; init; }
}