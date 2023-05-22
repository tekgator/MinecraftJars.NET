using MinecraftJars.Core.Projects;
using MinecraftJars.Plugin.Mojang.Models;

namespace MinecraftJars.Plugin.Mojang;

internal class MojangProjectFactory
{
    public const string Vanilla = "Vanilla"; 
    public const string VanillaSnapshot = "Vanilla Snapshot";
    public const string Bedrock = "Bedrock";
    public const string BedrockPreview = "Bedrock Preview";
    
    public static readonly IEnumerable<MojangProject> Projects = new List<MojangProject>
    {
        new()
        {
            Group = Group.Server,
            Name = Vanilla,
            Description = "The Mojang vanilla Minecraft experience without mod support.",
            Url = "https://www.minecraft.net/download/server",
            Logo = Properties.Resources.Vanilla
        },
        new()
        {
            Group = Group.Server,
            Name = VanillaSnapshot,
            Description = "A Snapshot is a testing version of Minecraft, periodically released by Mojang Studios.",
            Url = "https://feedback.minecraft.net/hc/en-us/sections/360002267532-Snapshot-Information-and-Changelogs",
            Logo = Properties.Resources.VanillaSnapshot
        },
        new()
        {
            Group = Group.Bedrock,
            Name = Bedrock,
            Description =
                "Minecraft: Bedrock Edition refers to the multi-platform versions of Minecraft based on the Bedrock codebase.",
            Url = "https://www.minecraft.net/download/server/bedrock",
            Logo = Properties.Resources.Bedrock
        },
        new()
        {
            Group = Group.Bedrock,
            Name = BedrockPreview,
            Description =
                "Minecraft Preview is an app players can use to test out beta features from Bedrock Edition. It works as a separate app, rather than the previous system of opt in beta content inside of the Bedrock Edition game itself.",
            Url = "https://www.minecraft.net/download/server/bedrock",
            Logo = Properties.Resources.BedrockPreview
        }
    };
}