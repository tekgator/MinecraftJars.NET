using System.Diagnostics;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Fabric.Model;

namespace MinecraftJars.Tests;

[TestFixture, Order(4)]
public class DownloadTests
{
    private static readonly ProviderManager ProviderManager = new();
    private static IEnumerable<string> Projects() => 
        ProviderManager.GetProviders().SelectMany(p => p.Projects.Select(t => t.Name));
    
    [Test, Order(1)]
    [Ignore("Very time consuming should only be utilised when necessary")]
    public async Task GetDownloads_Success(
        [ValueSource(nameof(Projects))] string projectName,        
        [Values(true, false)] bool loadFileSize)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);
        
        foreach (var version in await provider.GetVersions(project.Name))
        {
            TestContext.Progress.WriteLine("{0}: Retrieving download for version {1}", 
                nameof(GetDownloads_Success), version.Version);
            
            var download = await version.GetDownload(new DownloadOptions { LoadFilesize = loadFileSize });
            
            Assert.That(download, Is.Not.Null);
            Assert.That(download.BuildId, Has.Length.AtLeast(1));
            if (loadFileSize && !string.IsNullOrWhiteSpace(download.Url) && download is not FabricDownload)
                Assert.That(download.Size, Is.AtLeast(1));
            
            TestContext.Progress.WriteLine("{0}: Download found with BuildId {1} and Url {2} for version {3} ", 
                nameof(GetDownloads_Success), download.BuildId, download.Url, version.Version);
        }
    }
    
    [Test, Order(2)]
    public async Task GetDownloads_LatestVersion(
        [ValueSource(nameof(Projects))] string projectName,        
        [Values(true, false)] bool loadFileSize)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);
        var version = (await provider.GetVersions(project.Name, new VersionOptions { MaxRecords = 1 })).First();
        
        TestContext.Progress.WriteLine("{0}: Retrieving download for version {1}", 
            nameof(GetDownloads_LatestVersion), version.Version);
        
        var download = await version.GetDownload(new DownloadOptions { LoadFilesize = loadFileSize });
        
        Assert.That(download, Is.Not.Null);
        Assert.That(download.BuildId, Has.Length.AtLeast(1));
        if (loadFileSize && !string.IsNullOrWhiteSpace(download.Url) && download is not FabricDownload)
            Assert.That(download.Size, Is.AtLeast(1));
        
        TestContext.Progress.WriteLine("{0}: Download found with BuildId {1} and Url {2} for version {3} ", 
            nameof(GetDownloads_LatestVersion), download.BuildId, download.Url, version.Version);
    }  
    
    [TestCase("Spigot"), Order(3)]
    [Ignore("Very time consuming should only be utilised when necessary. Git and Java must be installed")]
    public async Task BuildSpigot_Success(string projectName)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);
        var version = (await provider.GetVersions(project.Name, new VersionOptions { MaxRecords = 1 })).First();
        
        TestContext.Progress.WriteLine("{0}: Start building {1} version {2}", 
            nameof(BuildSpigot_Success), projectName, version.Version);
        
        var download = await version.GetDownload(new DownloadOptions
        {
            BuildJar = true, 
            BuildJarOutput = TestContext.Progress.WriteLine
        });

        Assert.That(download, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(download.BuildId, Has.Length.AtLeast(1));
            Assert.That(File.Exists(download.Url), Is.True);
        });

        TestContext.Progress.WriteLine("{0}: Build {1} finished with BuildId {2} and Url {3} for version {4} ", 
            nameof(GetDownloads_LatestVersion), projectName, download.BuildId, download.Url, version.Version);

        try
        {
            var path = Path.GetDirectoryName(download.Url);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch { /* ignore */ }
    }
    
    [TestCase("Spigot"), Order(4)]
    [Ignore("Very time consuming should only be utilised when necessary. Git and Java must be installed")]
    public async Task BuildSpigot_Cancel(string projectName)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);
        var version = (await provider.GetVersions(project.Name, new VersionOptions { MaxRecords = 1 })).First();
        
        TestContext.Progress.WriteLine("{0}: Start building {1} version {2}", 
            nameof(BuildSpigot_Success), projectName, version.Version);

        var startTime = Stopwatch.GetTimestamp();
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await version.GetDownload(new DownloadOptions
            {
                BuildJar = true, 
                BuildJarOutput = TestContext.Progress.WriteLine
            }, cancellationTokenSource.Token));

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        
        Assert.That(elapsedTime.Seconds, Is.AtMost(15));
        
        TestContext.Progress.WriteLine("{0}: Build {1} successful cancelled after {2}s", 
            nameof(GetDownloads_LatestVersion), projectName, elapsedTime.Seconds);
    }    
}