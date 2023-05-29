using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Fabric.Model;

public record FabricProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;
