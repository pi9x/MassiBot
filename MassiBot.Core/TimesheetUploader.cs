using MassiBot.Core.FileServices;

namespace MassiBot.Core;

public class TimesheetUploader
{
    private readonly IDownloader _downloader;
    private readonly IUploader _uploader;

    public TimesheetUploader(IDownloader downloader, IUploader uploader)
    {
        _downloader = downloader;
        _uploader = uploader;
    }

    public async Task<bool> TransferFile(string downloadUrl, string fileName, string conversationId)
    {
        var content = await _downloader.Download(downloadUrl);

        var tags = new Dictionary<string, string>()
        {
            { "ConversationId", conversationId }
        };
        
        return await _uploader.Upload(content, fileName, tags);
    }
}