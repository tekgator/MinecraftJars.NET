using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public class MohistProject : IProject
{
    public required Group Group { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Url { get; init; }
    public required byte[] Logo { get; init; }
}