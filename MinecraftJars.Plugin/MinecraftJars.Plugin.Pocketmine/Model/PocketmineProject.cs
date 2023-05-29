using MinecraftJars.Core.Projects;

namespace MinecraftJars.Plugin.Pocketmine.Model;

public record PocketmineProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;