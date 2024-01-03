namespace MassiBot.Bot.AdaptiveCards;

internal class AdaptiveCardsSettings
{
    internal const string SchemaVersion = "1.3";
    
    internal class MessageFieldId
    {
        internal const string PoCode = "MassiBot.TimesheetLookup.Inputs.PoCode";
        internal const string EmployeeId = "MassiBot.TimesheetLookup.Inputs.Employee";
    }
    
    internal class Operation
    {
        internal const string TimesheetLookup = "TimesheetLookup";
    }
}