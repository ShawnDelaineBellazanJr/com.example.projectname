using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// Minimal MCP client + optional "intent" mode for MSBuild/automation.
// Now also supports a JSON config via --config intent.json

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
var configPath = GetArg("--config");

Console.WriteLine("Starting MCP client...");

// Defaults; can be overridden by --config
string serverName = "Everything";
string serverCommand = "npx";
string[] serverArgs = ["-y", "@modelcontextprotocol/server-everything"]; 

// If a config file is provided, pre-read it to possibly override server settings
JsonObject? configObj = null;
if (!string.IsNullOrWhiteSpace(configPath))
{
    var jsonText = await File.ReadAllTextAsync(configPath);
    configObj = JsonNode.Parse(jsonText) as JsonObject;
    var s = configObj?["server"] as JsonObject;
    if (s is not null)
    {
        serverName = s["name"]?.GetValue<string>() ?? serverName;
        serverCommand = s["command"]?.GetValue<string>() ?? serverCommand;
        if (s["args"] is JsonArray arr)
        {
            serverArgs = arr.Select(n => n!.GetValue<string>()).ToArray();
        }
    }
    // Allow out path override inside config
    outPath ??= configObj?["out"]?.GetValue<string>();
}

// CLI overrides take precedence over config
var cliServerCommand = GetArg("--serverCommand") ?? GetArg("--server");
var cliServerArgs = GetArg("--serverArgs");
if (!string.IsNullOrWhiteSpace(cliServerCommand))
{
    serverCommand = cliServerCommand!;
}
if (!string.IsNullOrWhiteSpace(cliServerArgs))
{
    serverArgs = ParseArgsList(cliServerArgs!);
}

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = serverName,
    Command = serverCommand,
    Arguments = serverArgs,
});

var client = await McpClientFactory.CreateAsync(transport);

if (!string.IsNullOrWhiteSpace(configPath))
{
    // Config-driven mode: perform one or more tool calls as defined in the JSON file.
    // Schema (minimal):
    // {
    //   "server": { "name": "Everything", "command": "npx", "args": ["-y","@modelcontextprotocol/server-everything"] },
    //   "call": { "tool": "echo", "params": { "message": "Hello from intent.json" } },
    //   "out": "out/intent-result.json",
    //   "listTools": false
    // }

    bool listTools = configObj?["listTools"]?.GetValue<bool?>() ?? false;
    if (listTools)
    {
        Console.WriteLine("Tools available:");
        foreach (var tool in await client.ListToolsAsync())
        {
            Console.WriteLine($"- {tool.Name}: {tool.Description}");
        }
    }

    var calls = new List<JsonObject>();

    // Multi-call support: either single `call` or array `calls`
    var callsArray = configObj?["calls"] as JsonArray;
    if (callsArray is not null && callsArray.Count > 0)
    {
        foreach (var cNode in callsArray)
        {
            var call = cNode as JsonObject;
            if (call is null) continue;
            var toolName = call["tool"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(toolName))
            {
                Console.Error.WriteLine("Config error: calls[i].tool is required.");
                Environment.ExitCode = 2;
                break;
            }

            var @params = new Dictionary<string, object?>();
            if (call["params"] is JsonObject p)
            {
                @params = ResolveParams(p, calls);
            }
            var res = await client.CallToolAsync(toolName!, @params, cancellationToken: CancellationToken.None);
            var text = res.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "<no text content returned>";
            Console.WriteLine($"{toolName} -> {text}");
            calls.Add(new JsonObject
            {
                ["Tool"] = toolName,
                ["Params"] = call["params"]?.DeepClone(),
                ["Text"] = text,
                ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
            });
        }
    }
    else
    {
        // Single call fallback
        var call = configObj?["call"] as JsonObject;
        if (call is not null)
        {
            var toolName = call["tool"]?.GetValue<string>();
            var @params = new Dictionary<string, object?>();
            if (call["params"] is JsonObject p)
            {
                @params = ResolveParams(p, calls);
            }

            if (string.IsNullOrWhiteSpace(toolName))
            {
                Console.Error.WriteLine("Config error: call.tool is required.");
                Environment.ExitCode = 2;
            }
            else
            {
                var res = await client.CallToolAsync(toolName!, @params, cancellationToken: CancellationToken.None);
                var text = res.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "<no text content returned>";
                Console.WriteLine($"{toolName} -> {text}");
                calls.Add(new JsonObject
                {
                    ["Tool"] = toolName,
                    ["Params"] = call["params"]?.DeepClone(),
                    ["Text"] = text,
                    ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
                });
            }
        }
    }

    if (!string.IsNullOrWhiteSpace(outPath))
    {
        var output = new JsonObject
        {
            ["Server"] = new JsonObject
            {
                ["Name"] = serverName,
                ["Command"] = serverCommand,
                ["Arguments"] = new JsonArray(serverArgs.Select(a => JsonValue.Create(a)!).ToArray())
            },
            ["Calls"] = new JsonArray(calls.ToArray()),
            ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
        await File.WriteAllTextAsync(outPath, output.ToJsonString(options));
        Console.WriteLine($"Wrote output to: {outPath}");
    }
}
else if (!string.IsNullOrWhiteSpace(intent))
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

// Local functions for config-driven templating and calls
static string ResolveTemplate(string value, List<JsonObject> priorCalls)
{
    if (priorCalls.Count == 0 || string.IsNullOrEmpty(value)) return value;

    // {{last.Text}}
    if (value.Contains("{{last.Text}}", StringComparison.Ordinal))
    {
        var lastText = priorCalls.LastOrDefault()?["Text"]?.GetValue<string>();
        if (!string.IsNullOrEmpty(lastText))
            value = value.Replace("{{last.Text}}", lastText, StringComparison.Ordinal);
    }

    // {{Calls[<index>].Text}}
    var m = Regex.Matches(value, "\\{\\{Calls\\[(\\d+)\\]\\.Text\\}\\}");
    foreach (Match match in m.Cast<Match>())
    {
        if (int.TryParse(match.Groups[1].Value, out var idx) && idx >= 0 && idx < priorCalls.Count)
        {
            var txt = priorCalls[idx]["Text"]?.GetValue<string>();
            if (txt is not null)
            {
                value = value.Replace(match.Value, txt, StringComparison.Ordinal);
            }
        }
    }

    return value;
}

static Dictionary<string, object?> ResolveParams(JsonObject rawParams, List<JsonObject> priorCalls)
{
    var dict = new Dictionary<string, object?>();
    foreach (var kv in rawParams)
    {
        if (kv.Value is null)
        {
            dict[kv.Key] = null;
        }
        else if (kv.Value is JsonValue v && v.TryGetValue<string>(out var s))
        {
            dict[kv.Key] = ResolveTemplate(s!, priorCalls);
        }
        else
        {
            // Pass-through for non-string primitives/objects
            dict[kv.Key] = kv.Value.GetValue<object?>();
        }
    }
    return dict;
}

// Parses a space-delimited argument string into an array, honoring simple quotes
static string[] ParseArgsList(string raw)
{
    if (string.IsNullOrWhiteSpace(raw)) return Array.Empty<string>();
    var matches = Regex.Matches(raw, @"""[^""]+""|[^\s]+");
    return matches.Cast<Match>()
        .Select(m => m.Value.Trim().Trim('"'))
        .Where(s => s.Length > 0)
        .ToArray();
}
