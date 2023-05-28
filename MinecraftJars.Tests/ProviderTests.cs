using MinecraftJars.Core.Projects;

namespace MinecraftJars.Tests;

[TestFixture, Order(1)]
public class ProviderTests
{
    private static readonly ProviderManager ProviderManager = new();
    private static IEnumerable<Group> Groups() => Enum.GetValues<Group>();

    [TestCase, Order(1)]
    public void GetProviders_Success()
    {
        var providers = ProviderManager.GetProviders().ToList();
        Assert.That(providers, Is.Not.Empty);
        
        TestContext.Progress.WriteLine("{0}: {1} providers found", nameof(GetProviders_Success), providers.Count);
    }

    [TestCaseSource(nameof(Groups)), Order(2)]
    public void GetProvidersByGroup_Success(Group group) 
    {
         var providers = ProviderManager.GetProviders(group).ToList();
         Assert.That(providers, Is.Not.Empty);
         
         TestContext.Progress.WriteLine("{0}: {1} providers found for {2}", nameof(GetProvidersByGroup_Success), providers.Count, group);
    } 
    
    [TestCase(100), Order(3)]
    public void GetProvidersByGroup_InvalidGroup(Group group)
    {
        Assert.That(ProviderManager.GetProviders(group), Is.Empty);
        TestContext.Progress.WriteLine("{0}: Group {1} is invalid", nameof(GetProvidersByGroup_InvalidGroup), group);
    }     
    
    [TestCase, Order(4)]
    public void GetProviderByName_Success()
    {
        foreach (var name in ProviderManager.GetProviders().Select(p => p.Name))
        {
            var provider = ProviderManager.GetProvider(name);
            Assert.That(provider, Is.Not.Null);
            
            TestContext.Progress.WriteLine("{0}: Provider for name {1} found", nameof(GetProviderByName_Success), name);
        }
    }

    [TestCase("InvalidProviderName"), Order(5)]
    public void GetProviderByName_InvalidProvider(string name)
    {
        Assert.Throws<InvalidOperationException>(() => ProviderManager.GetProvider(name));
        TestContext.Progress.WriteLine("{0}: Provider name {1} invalid", nameof(GetProviderByName_InvalidProvider), name);
    }
}