using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Spigot.Model;

public record SpigotProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;