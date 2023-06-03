using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Mojang.Models;

namespace MinecraftJars.Tests;

[TestFixture, Order(3)]
public class VersionTests
{
    private static readonly MinecraftJar MinecraftJar = new();
    private static IEnumerable<string> Projects() => 
        MinecraftJar.GetProviders().SelectMany(p => p.Projects.Select(t => t.Name));

    [TestCaseSource(nameof(Projects)), Order(1)]
    public void GetProjectsByName_Success(string projectName)
    {
        Assert.DoesNotThrow(() => MinecraftJar.GetProject(projectName));
        TestContext.Progress.WriteLine("{0}: Project found by name {1}", 
            nameof(GetProjectsByName_Success), projectName);
    }     
    
    [TestCaseSource(nameof(Projects)), Order(2)]
    public async Task GetVersions_Success(string projectName)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));
        var versions = (await project.GetVersions()).ToList();

        Assert.That(versions, Is.Not.Empty);
        Assert.That(versions.All(v => v.Project.Name.Equals(project.Name)), Is.True);
        
        TestContext.Progress.WriteLine("{0}: {1} versions found for {2}", 
            nameof(GetVersions_Success), versions.Count, project.Name);
    }
    
    [Test, Order(3)]
    public async Task GetVersions_MaxRecordsMax(
        [ValueSource(nameof(Projects))] string projectName, 
        [Values(5, 10)] int maxRecords)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));
        var versions = (await project.GetVersions(new VersionOptions
        {
            MaxRecords = maxRecords
        })).ToList();
        
        Assert.That(versions, Has.Count.AtMost(maxRecords));
        
        TestContext.Progress.WriteLine("{0}: {1} versions found for {2} with max. limit {3}", 
            nameof(GetVersions_MaxRecordsMax), versions.Count, project.Name, maxRecords);
    }    

    [TestCaseSource(nameof(Projects)), Order(4)]
    public async Task GetVersions_SpecificVersion(string projectName)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));
        var version = (await project.GetVersions()).First();
        var versions = (await project.GetVersions(new VersionOptions
        {
            Version = version.Version
        })).ToList();

        var count = version is MojangVersion { Os: not null } ? 2 : 1; 

        Assert.That(versions, Has.Count.EqualTo(count));
        Assert.That(versions.First(), Is.EqualTo(version));
        
        TestContext.Progress.WriteLine("{0}: Specific version {1} found", 
            nameof(GetVersions_SpecificVersion), version.Version);
    }  
    
    [TestCase("Vanilla")]
    [TestCase("Bedrock")]
    [TestCase("Fabric")]
    [TestCase("Pocketmine")]
    [TestCase("Spigot")]
    [TestCase("Paper")]
    [TestCase("Velocity")]
    [Order(5)]
    public async Task GetVersions_ContainsSnapshot(string projectName)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));

        var versions = (await project.GetVersions(new VersionOptions
        {
            IncludeSnapshotBuilds = true
        })).ToList();

        Assert.That(versions.Any(v => v.IsSnapShot), Is.True);
        
        TestContext.Progress.WriteLine("{0}: {1} snapshot version found",
            nameof(GetVersions_ContainsSnapshot), versions.Count(v => v.IsSnapShot));
    }
    
    [TestCase("Vanilla")]
    [TestCase("Bedrock")]
    [TestCase("Fabric")]
    [TestCase("Pocketmine")]
    [TestCase("Spigot")]
    [TestCase("Paper")]
    [TestCase("Velocity")]
    [Order(6)]
    public async Task GetVersions_ContainsNoSnapshot(string projectName)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));
        var versions = (await project.GetVersions(new VersionOptions
        {
            IncludeSnapshotBuilds = false
        })).ToList();

        Assert.That(versions.Any(v => v.IsSnapShot), Is.False);
        
        TestContext.Progress.WriteLine("{0}: No snapshot version found", nameof(GetVersions_ContainsNoSnapshot));
    }    
}