﻿using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Pocketmine.Model;

namespace MinecraftJars.Plugin.Pocketmine;

internal static class PocketmineProjectFactory
{
    public static readonly IEnumerable<PocketmineProject> Projects = new List<PocketmineProject>
    {
        new(ProjectGroup: ProjectGroup.Bedrock,
            Name: "Pocketmine",
            Description: "A highly customizable, open source server software for Minecraft: Bedrock Edition written in PHP.",
            Url:  "https://www.pocketmine.net",
            ProjectRuntime.Php,
            Logo: Properties.Resources.Pocketmine)
    };
}