using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Mojang.Models;

namespace MinecraftJars.Plugin.Mojang;

internal class MojangProjectFactory
{
    public static readonly IEnumerable<MojangProject> Projects = new List<MojangProject>
    {
        new(ProjectGroup: ProjectGroup.Server,
            Name: "Vanilla",
            Description: "The Mojang vanilla Minecraft experience without mod support.",
            Url: "https://www.minecraft.net/download/server",
            ProjectRuntime: ProjectRuntime.Java,
            Logo: Properties.Resources.Vanilla),
        new(ProjectGroup: ProjectGroup.Bedrock,
            Name: "Bedrock",
            Description: "Minecraft: Bedrock Edition refers to the multi-platform versions of Minecraft based on the Bedrock codebase.",
            Url: "https://www.minecraft.net/download/server/bedrock",
            ProjectRuntime: ProjectRuntime.None,
            Logo: Properties.Resources.Bedrock)
    };
}