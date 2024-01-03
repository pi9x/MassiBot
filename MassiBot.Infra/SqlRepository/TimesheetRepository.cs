using System.Data;
using System.Text;
using Dapper;
using MassiBot.Core.TimesheetQuery;
using Microsoft.Extensions.DependencyInjection;

namespace MassiBot.Infra.SqlRepository;

public class TimesheetRepository : ITimesheetRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IScriptsHelper _scriptsHelper;

    public TimesheetRepository(IDbConnection dbConnection, IScriptsHelper scriptsHelper)
    {
        _dbConnection = dbConnection;
        _scriptsHelper = scriptsHelper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TimesheetRow>> Query(string? poCode, string? employeeId)
    {
        var whereBuilder = new StringBuilder();
        var parameters = new DynamicParameters();
        
        // Add a PoCode to the where statement.
        if (!string.IsNullOrWhiteSpace(poCode))
        {
            whereBuilder.AppendLine("AND p.PoIdName = @poCode");
            parameters.Add("@poCode", poCode, DbType.String);
        }
        
        // Add employeeId to the where statement.
        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            whereBuilder.AppendLine("AND e.EmployeeId = @employeeId");
            parameters.Add("@employeeId", employeeId, DbType.String);
        }
        
        var whereClause = whereBuilder.ToString();
        
        var rawScript = _scriptsHelper.GetRawScript("QueryTimesheet.sql").Replace("--<<WHERE_CLAUSE>>--", whereClause);
        
        return await _dbConnection.QueryAsync<TimesheetRow>(rawScript, parameters, commandType: CommandType.Text);
    }
}

public static class TimesheetRepositoryRegistration
{
    /// <summary>
    /// Adds HTTPDownloader to the specified.
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The so that additional calls can be chained ; this is also the same as the passed in but can be</returns>
    public static IServiceCollection UseTimesheetRepository(this IServiceCollection services)
    {
        return services
            .AddScoped<IScriptsHelper, ScriptsHelper>()
            .AddScoped<ITimesheetRepository, TimesheetRepository>();
    }
}