using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Agents.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using System.Text.Json;

namespace CsAgentClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await MainAsync();

            static async Task MainAsync()
            {
                var configuration = new ConfigurationManager();
                configuration
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Local.json", optional: true);

                var settings = configuration
                    .GetRequiredSection("CopilotStudio")
                    .Get<AppSettings>()
                    ?? throw new InvalidOperationException(
                        "The CopilotStudio configuration section is empty.");

                var app = PublicClientApplicationBuilder
                    .Create(settings.ClientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
                    .WithDefaultRedirectUri()
                    .Build();

                var token = await app
                    .AcquireTokenInteractive(new[] { "https://api.powerplatform.com/CopilotStudio.Copilots.Invoke" })
                    .ExecuteAsync();

                var agentClientConnectionSettings = new ConnectionSettings
                {
                    EnvironmentId = settings.EnvironmentId,
                    SchemaName = settings.SchemaName,
                    CdsBotId = settings.BotId,
                };

                var services = new ServiceCollection();

#if DEBUG
                services
                    .AddTransient<HttpTracingHandler>()
                    .AddHttpClient("cs-agent-client")
                    .AddHttpMessageHandler<HttpTracingHandler>();
#else
                services.AddHttpClient("cs-agent-client");
#endif

                using var serviceProvider = services.BuildServiceProvider();

                var agentClient = new CopilotClient(
                    agentClientConnectionSettings,
                    serviceProvider.GetRequiredService<IHttpClientFactory>(),
                    _ => Task.FromResult(token.AccessToken),
                    NullLogger.Instance,
                    "cs-agent-client");

                await foreach (var _ in agentClient.StartConversationAsync()) { }

                const string message = "Who am I?";
                Console.WriteLine($"You: {message}");

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };


                await foreach (var reply in agentClient.AskQuestionAsync(message, null))
                {
                    if (reply.Type == "message")
                    {
                        Console.WriteLine($"Agent: {reply.Text}");
                    }

                    if (reply.Name == "connectors/consentCard")
                    {
                        var consentReply = new Activity
                        {
                            Type = ActivityTypes.Invoke,
                            Name = "connectors/consentCard",
                            ReplyToId = reply.Id,
                            Value = new
                            {
                                action = "Allow",
                                actionSubmitId = "Allow",
                                id = "submit",
                                shouldAwaitUserInput = true,
                            },
                        };

                        await foreach (var _ in agentClient.SendActivityAsync(consentReply)) { }
                    }
                }
            }
        }
    }
}