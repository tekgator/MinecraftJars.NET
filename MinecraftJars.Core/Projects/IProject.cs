namespace MinecraftJars.Core.Projects;

public interface IProject
{
    Group Group { get; }
    string Name { get; }
    string Description { get; }
    string Url { get; }
    byte[] Logo { get; }
}