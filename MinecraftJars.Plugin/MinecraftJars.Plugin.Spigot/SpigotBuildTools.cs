using System.Diagnostics;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Plugin.Spigot.Model;

namespace MinecraftJars.Plugin.Spigot;

public class SpigotBuildTools
{
    private const string BuildToolsRequestUri = "https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar";
    
    private readonly HttpClient _client;
    private readonly DownloadOptions _options;
    private readonly SpigotVersion _version;

    public SpigotBuildTools(
        HttpClient client,
        DownloadOptions options,
        SpigotVersion version)
    {
        _client = client;
        _options = options;
        _version = version;
    }

    public async Task<IDownload> Build(CancellationToken cancellationToken = default!)
    {
        var tempDir = CreateTempDir();
        try
        {
            var buildTools = await DownloadBuildTools(tempDir, cancellationToken);

            await RunBuildTools(tempDir, buildTools, cancellationToken);

            return new SpigotDownload(FileName: "todo",
                Size: 0,
                BuildId: "",
                Url: "todo",
                ReleaseTime: _version.ReleaseTime);

        }
        finally
        {   
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private async Task RunBuildTools(string dir, string buildTools, CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _options.Java,
                WorkingDirectory = dir,
                Arguments = $"-jar {buildTools} --rev {_version.Version}",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            }
        };
        process.Start();

        _ = Task.Run(() => ConnectOutput(process.StandardOutput));
        _ = Task.Run(() => ConnectError(process.StandardError));

        await process.WaitForExitAsync(cancellationToken);
    }
    
    private async Task ConnectOutput(StreamReader output)
    {
        if (_options.BuildJarOutput == null)
            return;
        
        while (!output.EndOfStream)
        {
            var line = await output.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            _options.BuildJarOutput(line);
        }
    }
    
    private async Task ConnectError(StreamReader error)
    {
        if (_options.BuildJarOutput == null)
            return;
        
        while (!error.EndOfStream)
        {
            var line = await error.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            _options.BuildJarOutput(line);
        }
    }
    
    private async Task<string> DownloadBuildTools(string dir, CancellationToken cancellationToken)
    {
        var response = await _client.GetStreamAsync(BuildToolsRequestUri, cancellationToken);
        var fileName = Path.Combine(dir, "BuildTools.jar");
        var buildTools = new FileStream(fileName, FileMode.Create);
        
        await response.CopyToAsync(buildTools, cancellationToken);
        await buildTools.DisposeAsync();

        return fileName;
    }
    
    private string CreateTempDir()
    {
        string tempPath;
        do
        {
            tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        } while (Directory.Exists(tempPath));

        Directory.CreateDirectory(tempPath);
        
        return tempPath;
    }
}