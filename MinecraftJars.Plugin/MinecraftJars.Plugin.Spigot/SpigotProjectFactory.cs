using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Spigot.Model;

namespace MinecraftJars.Plugin.Spigot;

internal static class SpigotProjectFactory
{
    public static readonly IEnumerable<SpigotProject> Projects = new List<SpigotProject>
    {
        new(ProjectGroup: ProjectGroup.Server,
            Name: "Spigot",
            Description: "Spigot is a modified Minecraft server which provides additional performance optimizations, configuration options and features, whilst still remaining compatible with all existing plugins and consistent with Vanilla Minecraft game mechanics.",
            Url:  "https://www.spigotmc.org/wiki/spigot",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Spigot),
        new(ProjectGroup: ProjectGroup.Proxy,
            Name: "BungeeCord",
            Description: "BungeeCord acts as a proxy between the player's client and multiple connected Minecraft servers.",
            Url:  "https://www.spigotmc.org/wiki/bungeecord",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.BungeeCord),
        
    };
}