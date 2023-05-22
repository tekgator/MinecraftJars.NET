using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Paper.Model;

public class PaperProject : IProject
{
    public required Group Group { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Url { get; init; }
    public required byte[] Logo { get; init; }
}