using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Paper.Model;

public record PaperProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;