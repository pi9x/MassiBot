using System;
using System.Data;
using System.Data.SqlClient;
using MassiBot.Infra.FileServices;
using MassiBot.Infra.SqlRepository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MassiBot.Bot.Startup))]

namespace MassiBot.Bot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging();
        
        builder.Services.UseAzureBlobUploader(Environment.GetEnvironmentVariable("AzureBlobStorageConnectionString") ??
                                              throw new InvalidOperationException("AzureBlobStorageConnectionString is missing"));
        
        builder.Services.UseHttpDownloader();

        builder.Services.UseTimesheetRepository();
        
        // Register Bot & Adapter
        builder.Services.AddSingleton<ICredentialProvider>(new SimpleCredentialProvider(
            Environment.GetEnvironmentVariable("MicrosoftAppId"), Environment.GetEnvironmentVariable("MicrosoftAppPassword")));
        builder.Services.AddTransient<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
        builder.Services.AddTransient<IBot, TimesheetBot>();
        builder.Services.AddSingleton<IDbConnection>(
            new SqlConnection(Environment.GetEnvironmentVariable("DbConnectionString")));
    }
}