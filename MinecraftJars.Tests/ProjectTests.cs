using MinecraftJars.Core.Projects;

namespace MinecraftJars.Tests;

[TestFixture, Order(2)]
public class ProjectTests
{
    private static readonly MinecraftJar MinecraftJar = new();
    private static IEnumerable<string> Providers() => MinecraftJar.GetProviders().Select(p => p.Name);
    private static IEnumerable<ProjectGroup> Groups() => Enum.GetValues<ProjectGroup>();
    
    [TestCaseSource(nameof(Providers)), Order(1)]
    public void GetProviderByProject_Success(string providerName)
    {
        var provider = MinecraftJar.GetProvider(providerName);
        Assert.That(provider, Is.Not.Null);
        
        foreach (var project in provider!.Projects)
        {
            var providerByProject = MinecraftJar.GetProvider(project);
            Assert.That(providerByProject, Is.SameAs(provider));
            
            TestContext.Progress.WriteLine("{0}: Provider for project {1} is {2}", 
                nameof(GetProviderByProject_Success), providerByProject!.Name, project.Name);
        }
    }
    
    [TestCase, Order(2)]
    public void GetProjects_Success()
    {
        var projects = MinecraftJar.GetProjects().ToList();
        Assert.That(projects, Is.Not.Empty);
        
        TestContext.Progress.WriteLine("{0}: {1} projects found", nameof(GetProjects_Success), projects.Count);
    }
    
    [TestCaseSource(nameof(Groups)), Order(3)]
    public void GetProjectsByGroup_Success(ProjectGroup projectGroup)
    {
        var projects = MinecraftJar.GetProjects(projectGroup).ToList();
        Assert.That(projects.All(p => p.ProjectGroup == projectGroup), Is.True);
        
        TestContext.Progress.WriteLine("{0}: {1} projects found for {2}", nameof(GetProjectsByGroup_Success), projects.Count, projectGroup);
    }    
}