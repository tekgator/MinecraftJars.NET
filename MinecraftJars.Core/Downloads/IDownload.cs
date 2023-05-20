namespace MinecraftJars.Core.Downloads;

public interface IDownload
{
    string FileName { get; }
    long Size { get; }
    int BuildId { get; }
    string Url { get; }
    HashType HashType { get; }
    string? Hash { get; }
    DateTime? ReleaseTime { get; }
}