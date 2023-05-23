using MinecraftJars.Core.Projects;

namespace MinecraftJars.Core.Versions;

public class VersionOptions
{
    public Group? Group { get; set; }
    public string? ProjectName { get; set; }
    public string? Version { get; set; }
    public int? MaxRecords { get; set; }
}