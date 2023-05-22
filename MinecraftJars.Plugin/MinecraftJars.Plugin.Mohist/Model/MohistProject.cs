using MinecraftJars.Core.Projects;
using MinecraftJars.Core.Versions;

namespace MinecraftJars.Plugin.Mohist.Model;

public record MohistProject(
    Group Group,
    string Name,
    string Description,
    string Url,
    byte[] Logo) : IProject;
