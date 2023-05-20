using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Mojang.Models;

public class MojangDownload : IDownload
{
    public required string FileName { get; init; }
    public required long Size { get; init; }
    public int BuildId => 0;
    public required string Url { get; init; }
    public DateTime? ReleaseTime { get; init; }
    public required HashType HashType { get; init; }
    public string? Hash { get; init; }
}