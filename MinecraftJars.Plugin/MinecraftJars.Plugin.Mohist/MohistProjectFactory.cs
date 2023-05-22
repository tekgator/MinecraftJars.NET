using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Mohist.Model;

namespace MinecraftJars.Plugin.Mohist;

internal static class MohistProjectFactory
{
    public static readonly IEnumerable<MohistProject> Projects = new List<MohistProject>
    {
        new(Group: Group.Server,
            Name: "MohistMC",
            Description: "Minecraft Forge Server Software Implementing Paper/Spigot/Bukkit API.",
            Url: "https://mohistmc.com",
            Logo: Properties.Resources.Mohist)
    };
}