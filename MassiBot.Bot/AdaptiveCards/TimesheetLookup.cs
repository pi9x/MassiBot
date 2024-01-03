using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdaptiveCards;
using MassiBot.Core.FileServices;
using MassiBot.Core.TimesheetQuery;
using MassiBot.Core.Validation;

namespace MassiBot.Bot.AdaptiveCards;

public class TimesheetLookup
{
    /// <summary>
    /// Creates and returns a form to lookup a timesheet.
    /// </summary>
    /// <returns>A new instance of the class with the contents filled in. The user will be prompted for PO Code and Employee</returns>
    public static AdaptiveCard CreateLookupForm() => new(AdaptiveCardsSettings.SchemaVersion)
    {
        Body = new List<AdaptiveElement>
        {
            new AdaptiveTextBlock("Please enter PO Code and/or Employee Account.")
            {
                Wrap = true
            },
            new AdaptiveTextInput
            {
                Label = "PO Code",
                Placeholder = "Example: PET-ICTFML201215137",
                Regex = RegexPatterns.PoCodeLookup,
                ErrorMessage = "Invalid PO Code",
                Id = AdaptiveCardsSettings.MessageFieldId.PoCode,
            },
            new AdaptiveTextInput
            {
                Label = "Employee Account",
                Placeholder = "Example: DangNH28, dangnh28",
                Regex = RegexPatterns.EmployeeAccountLookup,
                ErrorMessage = "Invalid Employee Account",
                Id = AdaptiveCardsSettings.MessageFieldId.EmployeeId,
            },
            new AdaptiveActionSet
            {
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "Lookup",
                        Data = new { Operation = AdaptiveCardsSettings.Operation.TimesheetLookup },
                    }
                }
            }
        }
    };

    #region Response in ColumnSet format
    /// <summary>
    /// Creates a column set that contains the timesheet rows. This is used to populate the response columns.
    /// </summary>
    /// <param name="records">The timesheets to include in the column set.</param>
    /// <param name="uploader">The uploader to use for uploading the timesheet.</param>
    /// <returns>A newly created response column set containing the records from the timesheet table in the order they appear in</returns>
    public static AdaptiveCard CreateResponseColumnSet(IEnumerable<TimesheetRow> records, IUploader uploader)
    {
        var body = new List<AdaptiveElement>
        {
            new AdaptiveTextBlock("This is the summary of timesheet that match your search.\n")
            {
                Wrap = true,
                Italic = true
            },
        };
        
        body.Add(HeaderColumnSet);
        body.AddRange(CreateTableRowsColumnSet(records, uploader));

        return new AdaptiveCard(AdaptiveCardsSettings.SchemaVersion)
        {
            Body = body,
        };
    }

    /// <summary>
    /// Creates column set for time sheet rows. This is used to populate the data table. It's not a public method because it doesn't have access to the database
    /// </summary>
    /// <param name="records">The timesheet rows to convert</param>
    /// <param name="uploader">The uploader to use for uploading data</param>
    /// <returns>AdaptiveColumnSet containing the data table columns for the timesheet rows or an empty column set if records is</returns>
    private static IEnumerable<AdaptiveColumnSet> CreateTableRowsColumnSet(IEnumerable<TimesheetRow> records, IUploader uploader)
    {
        // Returns an enumeration of the records in the list.
        if (records is null)
        {
            return Enumerable.Empty<AdaptiveColumnSet>();
        }

        return records.Select(r =>
        {
            var startDate = r.CycleStartDate.HasValue ? r.CycleStartDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            var endDate = r.CycleEndDate.HasValue ? r.CycleEndDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            
            return new AdaptiveColumnSet()
            {
                Columns = new List<AdaptiveColumn>()
                {
                    CreateSingleValueColumn(r.EmployeeId),
                    CreateSingleValueColumn(startDate),
                    CreateSingleValueColumn(endDate),
                    CreateSingleValueColumn(r.ActualWorkingHours.ToString(CultureInfo.InvariantCulture)),
                    CreateSingleValueColumn(r.ActualWorkingDays.ToString(CultureInfo.InvariantCulture)),
                    CreateSingleValueColumn(r.MissingDays.ToString(CultureInfo.InvariantCulture)),
                    CreateSingleValueColumn($"[{r.FileName}]({uploader.GetDownloadUrl(r.FileName)})"),
                },
                Separator = true
            };
        });
    }

    private static readonly AdaptiveColumnSet HeaderColumnSet = new()
    {
        Columns = new List<AdaptiveColumn>()
        {
            CreateSingleValueColumn("Employee", true),
            CreateSingleValueColumn("Cycle start date", true),
            CreateSingleValueColumn("Cycle end date", true),
            CreateSingleValueColumn("Actual working hours", true),
            CreateSingleValueColumn("Actual working days", true),
            CreateSingleValueColumn("Missing days", true),
            CreateSingleValueColumn("Timesheet", true),
        },
        Separator = true,
        Spacing = AdaptiveSpacing.Medium
    };

    /// <summary>
    /// Creates a column that contains a single value. This is used to create columns that are separated by commas and can be used for formatting
    /// </summary>
    /// <param name="value">The value to be displayed</param>
    /// <param name="isHeading">Whether or not the value is a heading</param>
    /// <returns>An instance of AdaptiveColumn that contains the value and a style for formatting the value in the same</returns>
    private static AdaptiveColumn CreateSingleValueColumn(string value, bool isHeading = false) => new()
    {
        Items = new List<AdaptiveElement>()
        {
            new AdaptiveTextBlock(value)
            {
                Wrap = true,
                Style = isHeading ? AdaptiveTextBlockStyle.Heading : AdaptiveTextBlockStyle.Paragraph,
                Separator = true,
                Weight = isHeading ? AdaptiveTextWeight.Bolder : AdaptiveTextWeight.Default
            }
        }
    };

    #endregion
}