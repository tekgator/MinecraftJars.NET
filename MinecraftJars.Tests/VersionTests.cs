using MinecraftJars.Core.Versions;

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
        var platform = OperatingSystem.IsWindows() ? VersionOs.Windows : VersionOs.Linux;
        
        var version = (await project.GetVersions(new VersionOptions
        {
            VersionOs = platform
        })).First();

        var versions = (await project.GetVersions(new VersionOptions
        {
            Version = version.Version,
            VersionOs = platform
        })).ToList();

        Assert.That(versions, Has.Count.EqualTo(1));
        Assert.That(versions.First(), Is.EqualTo(version));
        
        TestContext.Progress.WriteLine("{0}: Specific version {1} found", 
            nameof(GetVersions_SpecificVersion), version.Version);
    }  
    
    [TestCase("Bedrock"), Order(5)]
    public async Task GetVersions_SpecificOs(string projectName)
    {
        var project = MinecraftJar.GetProjects().Single(p => p.Name.Equals(projectName));

        foreach (var versionOs in Enum.GetValues<VersionOs>().Where(e => e != VersionOs.None))
        {
            var versions = (await project.GetVersions(new VersionOptions
            {
                VersionOs = versionOs
            })).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(versions.Any(v => v.Os != versionOs), Is.False);
                Assert.That(versions.Any(), Is.True);
            });
            
            TestContext.Progress.WriteLine("{0}: Search for {1} successful, no other Os found", 
                nameof(GetVersions_SpecificOs), versionOs);
        }
    }    
    
    [TestCase("Vanilla")]
    [TestCase("Bedrock")]
    [TestCase("Fabric")]
    [TestCase("Pocketmine")]
    [TestCase("Spigot")]
    [TestCase("Paper")]
    [TestCase("Velocity")]
    [Order(6)]
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
    [Order(7)]
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