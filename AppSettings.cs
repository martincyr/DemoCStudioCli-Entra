namespace CsAgentClient
{
    internal sealed class AppSettings
    {
        public string ClientId { get; init; } = string.Empty;

        public string TenantId { get; init; } = string.Empty;

        public string EnvironmentId { get; init; } = string.Empty;

        public string BotId { get; init; } = string.Empty;

        public string SchemaName { get; init; } = string.Empty;
    }
}
