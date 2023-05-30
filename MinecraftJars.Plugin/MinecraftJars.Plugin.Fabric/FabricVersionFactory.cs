using System.Net.Http.Json;
using MinecraftJars.Core.Downloads;
using MinecraftJars.Core.Versions;
using MinecraftJars.Plugin.Fabric.Model;
using MinecraftJars.Plugin.Fabric.Model.FabricApi;

namespace MinecraftJars.Plugin.Fabric;

internal static class FabricVersionFactory
{
    private const string FabricBaseRequestUri = "https://meta.fabricmc.net/v2";
    private const string FabricVersionsRequestUri = FabricBaseRequestUri + "/versions";
    private const string FabricLoaderRequestUri = FabricBaseRequestUri + "/versions/loader/{0}";
    private const string FabricDownloadRequestUri = FabricBaseRequestUri + "/versions/loader/{0}/{1}/{2}/server/jar";    
    
    public static HttpClient HttpClient { get; set; } = default!;
    
    public static async Task<List<FabricVersion>> GetVersion(
        string projectName,
        VersionOptions options,
        CancellationToken cancellationToken)
    {
        var project = FabricProjectFactory.Projects.Single(p => p.Name.Equals(projectName));

        var versionsApi = await HttpClient.GetFromJsonAsync<Versions>(FabricVersionsRequestUri, cancellationToken);        
    
        if (versionsApi == null) 
            throw new InvalidOperationException("Could not acquire game type details.");
   
        if (!string.IsNullOrWhiteSpace(options.Version))
            versionsApi.Games.RemoveAll(v => !v.Version.Equals(options.Version));

        if (!options.IncludeSnapshotBuilds)
            versionsApi.Games.RemoveAll(v => !v.Stable);

        var versions = versionsApi.Games
            .Select(game => new FabricVersion(
                Project: project, 
                Version: game.Version,
                IsSnapShot: !game.Stable)
                {
                    InstallerVersion = versionsApi.Installers.First().Version
                }).ToList();

        return options.MaxRecords.HasValue 
            ? versions.Take(options.MaxRecords.Value).ToList() 
            : versions;
    }

    public static async Task<IDownload> GetDownload(
        DownloadOptions options, 
        FabricVersion version, 
        CancellationToken cancellationToken)
    {
        var requestUriCompatibleLoaders = string.Format(FabricLoaderRequestUri, version.Version);
        var compatibleLoaders = await HttpClient
            .GetFromJsonAsync<List<CompatibleLoader>>(requestUriCompatibleLoaders, cancellationToken);

        if (compatibleLoaders == null || !compatibleLoaders.Any()) 
            throw new InvalidOperationException("Could not acquire download details.");

        var compatibleLoader = compatibleLoaders.First().Loader;
        
        var requestUriDownload = string
            .Format(FabricDownloadRequestUri, version.Version, compatibleLoader.Version, version.InstallerVersion);
        
        long contentLength = 0;
        var fileName = $"{version.Project.Name}-mc.{version.Version}-loader.{compatibleLoader.Version}-launcher{version.InstallerVersion}.jar";

        // Server does not return file size, therefore this request is not necessary
        //
        // if (options.LoadFilesize)
        // {
        //     using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUriDownload);
        //     using var httpResponse = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        //
        //     if (httpResponse.IsSuccessStatusCode)
        //     {
        //         contentLength = httpResponse.Content.Headers.ContentLength ?? contentLength;
        //         fileName = httpResponse.Content.Headers.ContentDisposition?.FileName ?? fileName;
        //     }
        // }

        return new FabricDownload(
            FileName: fileName,
            Size: contentLength,
            BuildId: compatibleLoader.Build.ToString(),
            Url: requestUriDownload
        );
    }
}