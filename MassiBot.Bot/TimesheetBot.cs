using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassiBot.Bot.AdaptiveCards;
using MassiBot.Core.FileServices;
using MassiBot.Core.TimesheetQuery;
using MassiBot.Core.Validation;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace MassiBot.Bot;

public class TimesheetBot : ActivityHandler
{
    private readonly IUploader _uploader;
    private readonly IDownloader _downloader;
    private readonly ITimesheetRepository _timesheetRepository;
    
    public TimesheetBot(IUploader uploader, IDownloader downloader, ITimesheetRepository timesheetRepository)
    {
        _uploader = uploader;
        _downloader = downloader;
        _timesheetRepository = timesheetRepository;
    }

    /// <summary>
    /// Called when [ message activity ]. This is the entry point for message activities. It determines what to do with the message activity and calls the appropriate method
    /// </summary>
    /// <param name="turnContext">The context of the message activity</param>
    /// <param name="cancellationToken">The cancellation token to use</param>
    /// <returns>A task representing the asynchronous operation of processing the message activity. The task will complete when the turn has</returns>
    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        // Process the uploaded files.
        if (turnContext.Activity.Attachments != null)
        {
            var attachments = turnContext.Activity.Attachments.Where(a => a.ContentType == FileDownloadInfo.ContentType)
                .ToList();
            // Process the uploaded files. If any of the attachments is empty then the file is processed.
            if (attachments.Any())
            {
                await ProcessUploadedFiles(turnContext, attachments, cancellationToken);
                return;
            }
        }

        // Process lookup request if the Activity. Value is a TimesheetLookup.
        if (turnContext.Activity.Value != null)
        {
            var value = JObject.FromObject(turnContext.Activity.Value);
            var operation = value.GetValue("Operation")?.Value<string>();
            // TimesheetLookup is the operation of the TimesheetLookup operation.
            if (operation is AdaptiveCardsSettings.Operation.TimesheetLookup)
            {
                await ProcessLookupRequest(turnContext, value, cancellationToken);
                return;
            }
        }
        
        // Send greeting if none of the defined actions matches the message activity
        var greetingCardAttachment = Greeting.CreateGreetingMessage(turnContext.Activity.From.Name).ToAttachment();
        await turnContext.SendActivityAsync(MessageFactory.Attachment(greetingCardAttachment), cancellationToken);
    }

    /// <summary>
    /// Processes a timesheet lookup request. This is the method that gets called by MassiBot to get the list of timesheets and send them to the client
    /// </summary>
    /// <param name="turnContext">The context of the turn</param>
    /// <param name="value">The value of the request as JSON object.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request</param>
    private async Task ProcessLookupRequest(ITurnContext turnContext, JObject value, CancellationToken cancellationToken)
    {
        var poCode = value.GetValue(AdaptiveCardsSettings.MessageFieldId.PoCode)?.Value<string>();
        var employeeId = value.GetValue(AdaptiveCardsSettings.MessageFieldId.EmployeeId)?.Value<string>();
        
        var records = await _timesheetRepository.Query(poCode, employeeId);
        
        var resultCardAttachment = TimesheetLookup.CreateResponseColumnSet(records, _uploader).ToAttachment();
        await turnContext.SendActivityAsync(MessageFactory.Attachment(resultCardAttachment), cancellationToken);
    }

    /// <summary>
    /// Processes the uploaded files. This is the main entry point for the Timesheet submission process. It checks to see if the file is valid and if it isn't it sends an activity to the turn context to upload it
    /// </summary>
    /// <param name="turnContext">The TurnContext to send activity to</param>
    /// <param name="attachments">The list of attachments to process. This should be sorted by filename</param>
    /// <param name="cancellationToken">The cancellation token to use for the</param>
    private async Task ProcessUploadedFiles(ITurnContext turnContext, List<Attachment> attachments, CancellationToken cancellationToken)
    {
        foreach (var attachment in attachments)
        {
            // Transfer the file to the current file.
            if (attachment.Name.TryGetValidFileName(out var validFileName))
            {
                attachment.Name = validFileName;
                await TransferFile(turnContext, attachment, cancellationToken);
                continue;
            }
            
            var invalidFileNameReply = TimesheetSubmission.CreateInvalidFileReply(attachment.Name).ToAttachment();
            var message = MessageFactory.Attachment(invalidFileNameReply);
            message.Importance = "high";
            await turnContext.SendActivityAsync(message, cancellationToken);
        }
    }

    /// <summary>
    /// Transfer a file to Timesheet. This is a blocking call. The file will be transferred to the server as long as there is room to send it.
    /// </summary>
    /// <param name="turnContext">The context in which the message is sent.</param>
    /// <param name="attachment">The attachment to transfer. Must have a Content property that is a JObject containing the content to be transferred.</param>
    /// <param name="cancellationToken">The cancellation token to use for this operation</param>
    private async Task TransferFile(ITurnContext turnContext, Attachment attachment, CancellationToken cancellationToken)
    {
        var fileDownload = JObject.FromObject(attachment.Content).ToObject<FileDownloadInfo>();
        var content = await _downloader.Download(fileDownload!.DownloadUrl);

        var tags = new Dictionary<string, string>()
        {
            { "ConversationId", turnContext.Activity.Conversation.Id }
        };
        
        var isSuccessful = await _uploader.Upload(content, attachment.Name, tags);
            
        // Send upload status reply
        var poCode = attachment.Name.GetPoCodeFromFileName();
        var uploadStatusReply = TimesheetSubmission.CreateUploadStatusReply(poCode, isSuccessful).ToAttachment();
        await turnContext.SendActivityAsync(MessageFactory.Attachment(uploadStatusReply), cancellationToken);
    }
}