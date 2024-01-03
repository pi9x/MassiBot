SELECT
    p.PoId,
    p.PoIdName,
    e.EmployeeId,
    e.Name AS EmployeeName,
    te.StartDate AS CycleStartDate,
    te.EndDate AS CycleEndDate,
    te.TotalDays AS ActualWorkingDays,
    te.MissingDays AS MissingDays,
    te.Url
FROM Timesheet_Entry te
INNER JOIN Po p ON
    te.PoId = p.PoId
INNER JOIN Employee e ON
    te.EmployeeId = e.EmployeeId
WHERE 1 = 1
--<<WHERE_CLAUSE>>--