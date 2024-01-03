using MassiBot.Core.FileServices;
using Microsoft.Extensions.DependencyInjection;

namespace MassiBot.Infra.FileServices;

public class HttpDownloader : IDownloader
{
    private readonly IHttpClientFactory _clientFactory;

    public HttpDownloader(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public async Task<Stream> Download(string url)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStreamAsync();
    }
}

public static class HttpDownloaderRegistration
{
    /// <summary>
    /// Adds HTTPDownloader to the specified.
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The so that additional calls can be chained ; this is also the same as the passed in but can be</returns>
    public static IServiceCollection UseHttpDownloader(this IServiceCollection services)
    {
        return services.AddHttpClient().AddScoped<IDownloader, HttpDownloader>();
    }
}