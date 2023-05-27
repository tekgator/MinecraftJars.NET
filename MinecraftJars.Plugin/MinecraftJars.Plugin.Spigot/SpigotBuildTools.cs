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

    public async Task<SpigotDownload> Build(string buildId, CancellationToken cancellationToken = default!)
    {
        var tempDir = CreateTempDir();
        var finalDir = CreateTempDir();
        try
        {
            var buildTools = await DownloadBuildTools(tempDir, cancellationToken);
            await RunBuildTools(tempDir, buildTools, finalDir, cancellationToken);
            var filePath = Path.Combine(finalDir, Directory.GetFiles(finalDir).First());
            
            return new SpigotDownload(
                FileName: Path.GetFileName(filePath),
                Size: new FileInfo(filePath).Length,
                BuildId: buildId,
                Url: filePath,
                ReleaseTime: _version.ReleaseTime);

        }
        catch
        {
            if (Directory.Exists(finalDir))
                DeleteDirectory(finalDir);

            throw;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                DeleteDirectory(tempDir);
        }
    }

   
    private async Task RunBuildTools(string dir, string buildTools, string outputDir, CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _options.JavaBin,
                WorkingDirectory = dir,
                Arguments = $"-jar {buildTools} --rev {_version.Version} --output-dir {outputDir}",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            }
        };
        process.Start();

        if (_options.BuildJarOutput != null)
        {
            _ = Task.Run(() => ConnectOutput(process.StandardOutput), cancellationToken);
            _ = Task.Run(() => ConnectError(process.StandardError), cancellationToken);
        }
        
        await process.WaitForExitAsync(cancellationToken);
    }
    
    private async Task ConnectOutput(StreamReader output)
    {
        while (!output.EndOfStream)
        {
            var line = await output.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            _options.BuildJarOutput!(line);
        }
    }
    
    private async Task ConnectError(StreamReader error)
    {
        while (!error.EndOfStream)
        {
            var line = await error.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            _options.BuildJarOutput!(line);
        }
    }
    
    private async Task<string> DownloadBuildTools(string dir, CancellationToken cancellationToken)
    {
        var response = await _client.GetStreamAsync(BuildToolsRequestUri, cancellationToken);
        var fileName = Path.Combine(dir, Path.GetFileName(new Uri(BuildToolsRequestUri).LocalPath));
        var buildTools = new FileStream(fileName, FileMode.Create);
        
        await response.CopyToAsync(buildTools, cancellationToken);
        await buildTools.DisposeAsync();

        return fileName;
    }
    
    private static string CreateTempDir()
    {
        string tempPath;
        do
        {
            tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        } while (Directory.Exists(tempPath));

        Directory.CreateDirectory(tempPath);
        
        return tempPath;
    }
    
    private static void DeleteDirectory(string path)
    {
        var root = new DirectoryInfo(path);
        var directories = new Stack<DirectoryInfo>();
        directories.Push(root);
        
        while (directories.Count > 0)
        {
            var directory = directories.Pop();
            directory.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
            foreach (var subDirectories in directory.GetDirectories())
            {
                directories.Push(subDirectories);
            }
            foreach (var files in directory.GetFiles())
            {
                files.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                files.Delete();
            }
        }
        root.Delete(true);
    }
}