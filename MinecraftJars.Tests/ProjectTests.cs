using MinecraftJars.Core.Projects;

namespace MinecraftJars.Tests;

[TestFixture, Order(2)]
public class ProjectTests
{
    private static readonly ProviderManager ProviderManager = new();
    private static IEnumerable<string> Providers() => ProviderManager.GetProviders().Select(p => p.Name);
    private static IEnumerable<Group> Groups() => Enum.GetValues<Group>();
    
    [TestCaseSource(nameof(Providers)), Order(1)]
    public void GetProviderByProject_Success(string providerName)
    {
        var provider = ProviderManager.GetProvider(providerName);
        
        foreach (var project in provider.Projects)
        {
            var providerByProject = ProviderManager.GetProvider(project);
            Assert.That(providerByProject, Is.SameAs(provider));
            
            TestContext.Progress.WriteLine("{0}: Provider for project {1} is {2}", 
                nameof(GetProviderByProject_Success), providerByProject.Name, project.Name);
        }
    }
    
    [TestCase, Order(2)]
    public void GetProjects_Success()
    {
        var projects = ProviderManager.GetProjects().ToList();
        Assert.That(projects, Is.Not.Empty);
        
        TestContext.Progress.WriteLine("{0}: {1} projects found", nameof(GetProjects_Success), projects.Count);
    }
    
    [TestCaseSource(nameof(Groups)), Order(3)]
    public void GetProjectsByGroup_Success(Group group)
    {
        var projects = ProviderManager.GetProjects(group).ToList();
        Assert.That(projects.All(p => p.Group == group), Is.True);
        
        TestContext.Progress.WriteLine("{0}: {1} projects found for {2}", nameof(GetProjectsByGroup_Success), projects.Count, group);
    }    
}