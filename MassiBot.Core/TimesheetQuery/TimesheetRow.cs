namespace MassiBot.Core.TimesheetQuery;

public class TimesheetRow
{
    public string EmployeeId { get; init; }
    public string? EmployeeName { get; init; }
    public DateTime? CycleStartDate { get; init; }
    public DateTime? CycleEndDate { get; init; }
    public int ActualWorkingDays { get; init; }
    public int ActualWorkingHours => ActualWorkingDays * 8;
    public int MissingDays { get; init; }
    public string Url { get; init; } = string.Empty;
    public string FileName => Url.Split('/').LastOrDefault(string.Empty);
}