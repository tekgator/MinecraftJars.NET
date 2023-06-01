﻿using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Fabric.Model;

namespace MinecraftJars.Plugin.Fabric;

internal static class FabricProjectFactory
{
    public const string Fabric = "Fabric"; 
    
    public static readonly IEnumerable<FabricProject> Projects = new List<FabricProject>
    {
        new(Group: Group.Server,
            Name: Fabric,
            Description: "Fabric is a lightweight, experimental modding toolchain for Minecraft.",
            Url: "https://fabricmc.net",
            Logo: Properties.Resources.Fabric)
    };
}