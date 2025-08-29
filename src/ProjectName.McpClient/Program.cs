using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;
using System.Text.Json.Nodes;

// Minimal MCP client + optional "intent" mode for MSBuild/automation.

string? GetArg(string name)
{
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == name && i + 1 < args.Length)
            return args[i + 1];
        var prefix = name + "=";
        if (args[i].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return args[i].Substring(prefix.Length);
    }
    return null;
}

var intent = GetArg("--intent");
var outPath = GetArg("--out");

Console.WriteLine("Starting MCP client...");

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "Everything",
    Command = "npx",
    Arguments = ["-y", "@modelcontextprotocol/server-everything"],
});

var client = await McpClientFactory.CreateAsync(transport);

if (!string.IsNullOrWhiteSpace(intent))
{
    // Intent mode: call echo with provided message and optionally write JSON output.
    var intentResult = await client.CallToolAsync(
        "echo",
        new Dictionary<string, object?> { ["message"] = intent },
        cancellationToken: CancellationToken.None);

    var intentText = intentResult.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text
                    ?? "<no text content returned>";
    Console.WriteLine($"Intent echo: {intentText}");

    if (!string.IsNullOrWhiteSpace(outPath))
    {
        var node = new JsonObject
        {
            ["Dynamic"] = new JsonObject
            {
                ["Intent"] = intent,
                ["Echo"] = intentText,
                ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
            }
        };
        var options = new JsonSerializerOptions { WriteIndented = true };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
        await File.WriteAllTextAsync(outPath, node.ToJsonString(options));
        Console.WriteLine($"Wrote dynamic output to: {outPath}");
    }
}
else
{
    // Default demo: list tools and call echo("Hello MCP!")
    Console.WriteLine("Tools available:");
    foreach (var tool in await client.ListToolsAsync())
    {
        Console.WriteLine($"- {tool.Name}: {tool.Description}");
    }

    var result = await client.CallToolAsync(
        "echo",
        new Dictionary<string, object?> { ["message"] = "Hello MCP!" },
        cancellationToken: CancellationToken.None);

    var textBlock = result.Content.OfType<TextContentBlock>().FirstOrDefault();
    Console.WriteLine(textBlock is not null
        ? $"Echo result: {textBlock.Text}"
        : "Echo result: <no text content returned>");
}
