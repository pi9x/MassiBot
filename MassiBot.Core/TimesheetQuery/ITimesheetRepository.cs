namespace MassiBot.Core.TimesheetQuery;

public interface ITimesheetRepository
{
    /// <summary>
    /// Queries timesheet by PO code or/and Employee Id
    /// </summary>
    /// <param name="poCode">Po code of employee to query</param>
    /// <param name="employeeId">Employee's ID to query</param>
    /// <returns>TimesheetRow for specified PO or null if not found in the database or no rows are returned by</returns>
    Task<IEnumerable<TimesheetRow>> Query(string? poCode, string? employeeId);
}