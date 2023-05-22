using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Purpur.Model;

public record PurpurProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;