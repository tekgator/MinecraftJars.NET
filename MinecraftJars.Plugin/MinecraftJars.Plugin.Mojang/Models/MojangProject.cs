using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Mojang.Models;

public record MojangProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;