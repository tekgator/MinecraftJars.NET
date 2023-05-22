using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistDownload(
    string FileName,
    long Size,
    int BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType,
    string? Hash) : IDownload;

/*
public class MohistDownload : IDownload
{
    public required string FileName { get; init; }
    public required long Size { get; init; }
    public required int BuildId { get; init; }
    public required string Url { get; init; }
    public required DateTime? ReleaseTime { get; init; }
    public required HashType HashType { get; init; }
    public required string? Hash { get; init; }
}*/