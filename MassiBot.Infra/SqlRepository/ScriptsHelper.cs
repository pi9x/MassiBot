using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Options;

namespace MassiBot.Infra.SqlRepository;

public interface IScriptsHelper
{
    string GetRawScript(string fileName);
}

public class ScriptsHelper : IScriptsHelper
{
    private readonly IOptions<ExecutionContextOptions> _executionContext;

    public ScriptsHelper(IOptions<ExecutionContextOptions> executionContext)
    {
        _executionContext = executionContext;
    }

    public string GetRawScript(string fileName)
    {
        var rootDir = _executionContext.Value.AppDirectory;
        var filePath = Path.Combine(rootDir, "SqlRepository", "Scripts", fileName);
        
        return File.ReadAllText(filePath);
    }
}