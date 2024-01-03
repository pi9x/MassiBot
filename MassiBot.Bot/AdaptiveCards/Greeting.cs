using System.Collections.Generic;
using AdaptiveCards;

namespace MassiBot.Bot.AdaptiveCards;

public class Greeting
{
    /// <summary>
    /// Creates a greeting message. It's used to say what the user can do with this application.
    /// </summary>
    /// <param name="userName">The name of the user</param>
    /// <returns>An AdaptiveCard with a body</returns>
    public static AdaptiveCard CreateGreetingMessage(string userName)
    {
        return new AdaptiveCard(AdaptiveCardsSettings.SchemaVersion)
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock($"Howdy, {userName}! What would you like to do?")
                {
                    Wrap = true
                },
                new AdaptiveActionSet
                {
                    Actions = new List<AdaptiveAction>
                    {
                        ShowSubmissionInstruction,
                        ShowTimesheetLookup
                    }
                }
            }
        };
    }
    
    private static readonly AdaptiveShowCardAction ShowSubmissionInstruction = new()
    {
        Title = "Timesheet Submission",
        Card = TimesheetSubmission.CreateInstruction()
    };
    
    private static readonly AdaptiveShowCardAction ShowTimesheetLookup = new()
    {
        Title = "Timesheet Review",
        Card = TimesheetLookup.CreateLookupForm()
    };
}