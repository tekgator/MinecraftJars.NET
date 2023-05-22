using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Purpur.Model;

namespace MinecraftJars.Plugin.Purpur;

internal static class PurpurProjectFactory
{
    public static readonly IEnumerable<PurpurProject> Projects = new List<PurpurProject>
    {
        new(Group: Group.Server,
            Name: "Purpur",
            Description: "Purpur is a drop-in replacement for Paper servers designed for configurability, new fun and exciting gameplay features, and performance built on top of Paper.",
            Url:  "https://purpurmc.org",
            Logo: Properties.Resources.Purpur)
    };
}