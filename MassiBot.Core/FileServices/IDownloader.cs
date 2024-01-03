namespace MassiBot.Core.FileServices;

public interface IDownloader
{
    /// <summary>
    /// Downloads the specified URL.
    /// </summary>
    /// <param name="url">The URL to download. This can be a URL or a file name.</param>
    /// <returns>A stream that can be used to read the file or null if the download failed for any reason. The stream will be closed when the download is complete</returns>
    Task<Stream> Download(string url);
}