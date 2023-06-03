namespace MinecraftJars.Core.Projects;

public enum ProjectRuntime
{
    /// <summary>
    /// No special runtime required e.g. compiled executable for this os (e.g. Mojang Bedrock)
    /// </summary>
    None,
    
    /// <summary>
    /// Java installation required (most of the projects)
    /// </summary>
    Java,
    
    /// <summary>
    /// PHP installation required e.g. Pocketmine which requires a special PHP installation
    /// </summary>
    Php
}