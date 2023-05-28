using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mojang.Models;

namespace MinecraftJars.Tests;

[TestFixture, Order(3)]
public class VersionTests
{
    private static readonly ProviderManager ProviderManager = new ProviderManager();
    private static IEnumerable<string> Projects() => 
        ProviderManager.GetProviders().SelectMany(p => p.Projects.Select(t => t.Name));
    
    [TestCaseSource(nameof(Projects)), Order(1)]
    public async Task GetVersions_Success(string projectName)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);
        var versions = (await provider.GetVersions(project.Name)).ToList();

        Assert.That(versions, Is.Not.Empty);
        Assert.That(versions.All(v => v.Project.Name.Equals(project.Name)), Is.True);
        
        TestContext.Progress.WriteLine("{0}: {1} versions found for {2}", 
            nameof(GetVersions_Success), versions.Count, project.Name);
    }
    
    [Test, Order(2)]
    public async Task GetVersions_MaxRecordsMax(
        [ValueSource(nameof(Projects))] string projectName, 
        [Values(5, 10)] int maxRecords)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);

        var versions =
            (await provider.GetVersions(project.Name, new VersionOptions { MaxRecords = maxRecords })).ToList();
        Assert.That(versions, Has.Count.AtMost(maxRecords));
        
        TestContext.Progress.WriteLine("{0}: {1} versions found for {2} with max. limit {3}", 
            nameof(GetVersions_MaxRecordsMax), versions.Count, project.Name, maxRecords);
    }    

    [TestCaseSource(nameof(Projects)), Order(3)]
    public async Task GetVersions_SpecificVersion(string projectName)
    {
        var project = ProviderManager.GetProjects().Single(p => p.Name.Equals(projectName));
        var provider = ProviderManager.GetProvider(project);

        var version = (await provider.GetVersions(project.Name)).First();

        var versions =
            (await provider.GetVersions(project.Name, new VersionOptions { Version = version.Version })).ToList();

        var count = version is MojangVersion { Os: not null } ? 2 : 1; 

        Assert.That(versions, Has.Count.EqualTo(count));
        Assert.That(versions.First(), Is.EqualTo(version));
        
        TestContext.Progress.WriteLine("{0}: Specific version {1} found", 
            nameof(GetVersions_SpecificVersion), version.Version);
    }  
}