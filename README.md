# Copilot Studio Agent Client

## Microsoft Entra ID prerequisites

This console application uses interactive authentication and requires a Microsoft Entra app registration configured as a public client.

1. In the Microsoft Entra admin center, create or open an **App registration**.
2. Under **Authentication**, add the **Mobile and desktop applications** platform with `http://localhost` as its redirect URI.
3. Under **Authentication** > **Advanced settings**, set **Allow public client flows** to **Yes**.
4. Under **API permissions**, select **Add a permission**.
5. Select **APIs my organization uses**, then find **Power Platform API**.
6. Add the delegated permission **CopilotStudio.Copilots.Invoke**.
7. Grant consent for the permission. Depending on tenant policy, an administrator may need to select **Grant admin consent**.

Set the app registration's **Application (client) ID** and **Directory (tenant) ID** in the ignored `appsettings.Local.json` file.

## Copilot Studio agent setup

1. In Copilot Studio, create an agent that uses the **standard harness**.
2. Add a single tool to the agent: **User IQ**.
3. Enable the **Native** channel and publish the agent.
4. Open **Settings** > **Advanced** > **Metadata** for the agent.
5. Copy the **Environment ID**, **Bot ID**, and **Schema name** into the corresponding `EnvironmentId`, `BotId`, and `SchemaName` values in `appsettings.Local.json`.