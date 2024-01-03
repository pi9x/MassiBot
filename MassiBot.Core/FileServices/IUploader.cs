namespace MassiBot.Core.FileServices;

public interface IUploader
{
    /// <summary>
    /// Uploads the specified content to storage.
    /// </summary>
    /// <param name="content">The content to upload. Must be a Stream</param>
    /// <param name="fileName">The name of the file to upload</param>
    /// <param name="tags">A dictionary of key / value tags to associate with</param>
    Task<bool> Upload(Stream content, string fileName, Dictionary<string, string> tags);
    
    /// <summary>
    /// Gets the download URL. This is used to download files from the web.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The download URL or null if the file could not be found or not downloaded at all. In this case the return value is a string containing the URL</returns>
    string GetDownloadUrl(string fileName);
}