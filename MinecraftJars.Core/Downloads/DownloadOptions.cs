namespace MinecraftJars.Core.Downloads;

public class DownloadOptions
{
    public bool LoadFilesize { get; set; } = true;
    public bool BuildJar { get; set; } = true;
    public string Java { get; set; } = "java";
    public Action<string>? BuildJarOutput;
}