using MinecraftJars.Core.Downloads;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotDownload(
    string FileName,
    long Size,
    int BuildId,
    string Url,
    DateTime? ReleaseTime,
    HashType HashType = HashType.None,
    string? Hash = null) : IDownload;