﻿using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Paper.Model;

namespace MinecraftJars.Plugin.Paper;

internal static class PaperProjectFactory
{
    public static readonly IEnumerable<PaperProject> Projects = new List<PaperProject>
    {
        new(ProjectGroup: ProjectGroup.Server,
            Name: "Paper",
            Description: "Paper is a Minecraft game server based on Spigot, designed to greatly improve performance and offer more advanced features and API.",
            Url: "https://purpurmc.org",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Paper),
        new(ProjectGroup: ProjectGroup.Server,
            Name: "Folia",
            Description: "Folia is a new fork of Paper that adds regionized multithreading to the server.",
            Url: "https://papermc.io/software/folia",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Folia), 
        new(ProjectGroup: ProjectGroup.Proxy,
            Name: "Velocity",
            Description: "Velocity is the modern, high-performance proxy. Designed with performance and stability in mind, it’s a full alternative to Waterfall with its own plugin ecosystem.",
            Url: "https://papermc.io/software/velocity",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Velocity),    
        new(ProjectGroup: ProjectGroup.Proxy,
            Name: "Waterfall",
            Description: "Waterfall is an upgraded BungeeCord, offering full compatibility with improvements to performance and stability.",
            Url: "https://papermc.io/software/waterfall",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Waterfall)
    };    
}