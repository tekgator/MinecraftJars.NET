using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Paper.Model;

public record PaperDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType,
    string? Hash) : IMinecraftDownload;
