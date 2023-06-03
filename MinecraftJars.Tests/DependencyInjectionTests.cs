using Microsoft.Extensions.DependencyInjection;
using MinecraftJars.Extension.DependencyInjection;

namespace MinecraftJars.Tests;

[TestFixture, Order(5)]
public class DependencyInjectionTests
{
    [Test]
    public void DependencyInjection_Success()
    {
        using var serviceProvider = new ServiceCollection().AddMinecraftJar().BuildServiceProvider();
        Assert.That(serviceProvider.GetService<IMinecraftJar>(), Is.Not.Null);
        
        TestContext.Progress.WriteLine("{0}: Injected {1} successful", 
            nameof(DependencyInjection_Success), nameof(IMinecraftJar));
    } 
}