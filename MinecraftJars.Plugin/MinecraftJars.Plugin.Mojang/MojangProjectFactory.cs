using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Mojang.Models;

namespace MinecraftJars.Plugin.Mojang;

internal class MojangProjectFactory
{
    public static readonly IEnumerable<MojangProject> Projects = new List<MojangProject>
    {
        new(Group: Group.Server,
            Name: "Vanilla",
            Description: "The Mojang vanilla Minecraft experience without mod support.",
            Url: "https://www.minecraft.net/download/server",
            Runtime: Runtime.Java,
            Logo: Properties.Resources.Vanilla),
        new(Group: Group.Bedrock,
            Name: "Bedrock",
            Description: "Minecraft: Bedrock Edition refers to the multi-platform versions of Minecraft based on the Bedrock codebase.",
            Url: "https://www.minecraft.net/download/server/bedrock",
            Runtime: Runtime.None,
            Logo: Properties.Resources.Bedrock)
    };
}