using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistDownload(
    string FileName,
    long Size,
    string BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType,
    string? Hash) : IMinecraftDownload;