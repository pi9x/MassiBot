using System.Collections.Generic;
using AdaptiveCards;

namespace MassiBot.Bot.AdaptiveCards;

public class TimesheetSubmission
{
    /// <summary>
    /// Creates a new instance of AdaptiveCard. Includes instructions to upload timesheet.
    /// </summary>
    /// <returns>A new instance of AdaptiveCard or null if creation failed for some reason ( such as invalid XML</returns>
    public static AdaptiveCard CreateInstruction() => new(AdaptiveCardsSettings.SchemaVersion)
    {
        Body = new List<AdaptiveElement>
        {
            new AdaptiveTextBlock("Please upload your timesheet in this chat.")
            {
                Wrap = true
            },
            FileNameFacts
        }
    };
    
    /// <summary>
    /// Creates a reply to the user that their timesheet was uploaded. This is used to reply to the user when they've received a succesful upload or failed
    /// </summary>
    /// <param name="poCode">The PO code for whom the timesheet was uploaded</param>
    /// <param name="isSuccessful">A value indicating whether the timesheet was uploaded successfully</param>
    /// <returns>A reply to the user that they've received a succesful upload or failed to upload the</returns>
    public static AdaptiveCard CreateUploadStatusReply(string poCode, bool isSuccessful)
    {
        var message = isSuccessful
            ? $"Your timesheet for PO Code {poCode} is well-received."
            : $"Error uploading timesheet for PO Code {poCode}. Please try again or contact Massi team.";
        
        return new AdaptiveCard(AdaptiveCardsSettings.SchemaVersion)
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock(message)
                {
                    Wrap = true
                }
            }
        };
    }

    /// <summary>
    /// Creates a reply to the user that the uploaded file does not meet the requirements. This is used to inform the user of invalid files
    /// </summary>
    /// <param name="fileName">The name of the file that is invalid.</param>
    /// <returns>The card that was created for the file or null if none was created for the file or the user didn't have permission to upload</returns>
    public static AdaptiveCard CreateInvalidFileReply(string fileName) => new(AdaptiveCardsSettings.SchemaVersion)
    {
        Body = new List<AdaptiveElement>
        {
            new AdaptiveTextBlock($"Your uploaded file '{fileName}' does not meet the requirements.")
            {
                Wrap = true
            },
            FileNameFacts
        }
    };

    /// <summary>
    /// Creates timesheet processing status reply. This is used to reply to user when time sheet processing is finished
    /// </summary>
    /// <param name="fileName">Timesheet file name that was processed</param>
    /// <param name="message">Message to be displayed to user. It will be wrapped in a div</param>
    /// <returns>AdaptiveCard containing the status</returns>
    public static AdaptiveCard CreateTimesheetProcessingStatusReply(string fileName, string message)
    {
        return new AdaptiveCard(AdaptiveCardsSettings.SchemaVersion)
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock("Timesheet processing result")
                {
                    Wrap = true
                },
                new AdaptiveFactSet
                {
                    Facts = new List<AdaptiveFact>
                    {
                        new("Timesheet", fileName),
                        new("Result", message),
                    }
                }
            }
        };
    }

    private static readonly AdaptiveFactSet FileNameFacts = new()
    {
        Facts = new List<AdaptiveFact>
        {
            new("Naming convention", "<FPT account>_<PO code>_yyyy_MM_x, in which 'x' is the timesheet cycle."),
            new("File format", "PDF"),
            new("Example", "DangNH28_PET-ICTFML201215137_2023_10_2.pdf; i.e. Timesheet for account DangNH28, PO code PET-ICTFML201215137, the 2nd cycle of October 2023.")
        }
    };
}