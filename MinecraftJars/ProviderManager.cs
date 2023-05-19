using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MinecraftJars.Core;
using MinecraftJars.Core.Providers;

namespace MinecraftJars;

public class ProviderManager
{
    [ImportMany(typeof(IProvider))]
    private IEnumerable<IProvider> _providers;

    [Export]
    private ProviderOptions ProviderOptions { get; }
    
    public ProviderManager(ProviderOptions? options = null)
    {
        ProviderOptions = options ?? new ProviderOptions();
        
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new DirectoryCatalog(path));

        var container = new CompositionContainer(catalog);
        container.ComposeParts(this);

        _providers ??= Enumerable.Empty<IProvider>();
    }
    
    public IEnumerable<IProvider> GetProviders()
    {
        return _providers;
    }

    public IProvider GetProvider(string provider)
    {
        return _providers.First(p => p.Name.Equals(provider));
    }

}