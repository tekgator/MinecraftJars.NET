namespace MinecraftJars.Core.Downloads;

public class DownloadOptions
{
    public bool LoadFilesize { get; set; } = true;
    public bool BuildJar { get; set; } = false;
    public string JavaBin { get; set; } = "java";
    public Action<string>? BuildJarOutput;
}