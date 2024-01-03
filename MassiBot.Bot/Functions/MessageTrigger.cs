using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Logging;

namespace MassiBot.Bot.Functions;

public class MessageTrigger
{
    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly IBot _bot;
    public MessageTrigger(IBotFrameworkHttpAdapter adapter, IBot bot)
    {
        _adapter = adapter;
        _bot = bot;
    }
    
    /// <summary>
    /// Trigger that processes a request. The request is passed to the adapter for processing as well as the response to the request
    /// </summary>
    /// <param name="req">The request to be processed</param>
    /// <param name="log">The logger to log to. If null no logging is</param>
    [FunctionName("MessageTrigger")]
    public async Task RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var correlationId = Guid.NewGuid();
        log.LogInformation("[CorrelationId: {CorrelationId}] Received request: {Request}", correlationId, await req.ReadAsStringAsync());

        var res = req.HttpContext.Response;
        await _adapter.ProcessAsync(req, res, _bot);
        log.LogInformation("[CorrelationId: {CorrelationId}] Response status: {ResponseStatusCode}", correlationId, res.StatusCode);
    }
}