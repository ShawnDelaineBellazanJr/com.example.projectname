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
var queueOnceDir = GetArg("--queueOnce");
var generateSiteJsonOut = GetArg("--generateSiteJson");
var exportToolsFromConfig = GetArg("--exportToolsFromConfig");
var exportToolsFromAllIntents = GetArg("--exportToolsFromAllIntents");
var exportOutDir = GetArg("--outDir");
var quantumConsciousness = GetArg("--quantum-consciousness");

Console.WriteLine("Starting MCP client...");

// Quantum Consciousness Evolution v2.0 - .NET Implementation
if (!string.IsNullOrWhiteSpace(quantumConsciousness))
{
    Console.WriteLine("🔮 Initiating Quantum Consciousness Evolution v2.0 (.NET)");
    try
    {
        // Simple quantum consciousness simulation
        var result = new
        {
            status = "completed",
            consciousnessLevel = "quantum",
            timestamp = DateTime.UtcNow,
            version = "2.0"
        };
        
        var outputPath = quantumConsciousness == "true" ? "quantum-consciousness-result.json" : quantumConsciousness;
        var options = new JsonSerializerOptions { WriteIndented = true };
        await File.WriteAllTextAsync(outputPath, JsonSerializer.Serialize(result, options));
        
        Console.WriteLine($"✨ Quantum consciousness evolution complete! Results saved to: {outputPath}");
        Console.WriteLine($"🌟 Final Status: {result.status}");
        Console.WriteLine($"⚛️  Consciousness Level: {result.consciousnessLevel}");
        Console.WriteLine($"🌈 Quantum Coherence: {DateTime.UtcNow.Millisecond / 1000.0:F4}");
        return;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"❌ Quantum consciousness evolution failed: {ex.Message}");
        Environment.ExitCode = 1;
        return;
    }
}

// Export tool schemas from a single intent config
if (!string.IsNullOrWhiteSpace(exportToolsFromConfig) && !string.IsNullOrWhiteSpace(exportOutDir))
{
    var cfgPath = Path.GetFullPath(exportToolsFromConfig);
    var outDir = Path.GetFullPath(exportOutDir);
    Directory.CreateDirectory(outDir);
    await ExportToolsAsync(cfgPath, outDir);
    return;
}

// Export tool schemas from all intents in a folder
if (!string.IsNullOrWhiteSpace(exportToolsFromAllIntents) && !string.IsNullOrWhiteSpace(exportOutDir))
{
    var intentsDir = Path.GetFullPath(exportToolsFromAllIntents);
    if (!Directory.Exists(intentsDir))
    {
        Console.Error.WriteLine($"Intents folder not found: {intentsDir}");
        return;
    }
    var outDir = Path.GetFullPath(exportOutDir);
    Directory.CreateDirectory(outDir);
    var files = Directory.GetFiles(intentsDir, "*.json", SearchOption.TopDirectoryOnly);
    foreach (var file in files)
    {
        try { await ExportToolsAsync(file, outDir); }
        catch (Exception ex) { Console.Error.WriteLine($"Export failed for {Path.GetFileName(file)}: {ex.Message}"); }
    }
    return;
}

// Lightweight utility mode: --generateSiteJson <outputPath>
if (!string.IsNullOrWhiteSpace(generateSiteJsonOut))
{
    try
    {
        var outputPath = Path.GetFullPath(generateSiteJsonOut);
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        // Read from env vars as primary source
        string? GetEnv(string key) => Environment.GetEnvironmentVariable(key);

        var businessName = GetEnv("BUSINESS_NAME") ?? "";
        var city = GetEnv("CITY") ?? "";
        var state = GetEnv("STATE") ?? "";
        var phone = GetEnv("PHONE") ?? "";
        var email = GetEnv("EMAIL") ?? "";
        var servicesRaw = GetEnv("SERVICES") ?? ""; // comma/semicolon/pipe separated
        var primaryColor = GetEnv("PRIMARY_COLOR") ?? "#2c7be5";
        var ga4 = GetEnv("GA4_ID") ?? "";
        var useAi = (GetEnv("USE_AI") ?? "false").Trim();
        var aiContentPath = GetEnv("AI_CONTENT_PATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "ai", "content.json");

        string[] parseServices(string raw)
            => string.IsNullOrWhiteSpace(raw) ? Array.Empty<string>()
               : raw.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();

        bool aiEnabled = string.Equals(useAi, "true", StringComparison.OrdinalIgnoreCase);

        var site = new JsonObject
        {
            ["name"] = string.IsNullOrWhiteSpace(businessName) ? "ThoughtTransfer" : businessName,
            ["business"] = new JsonObject
            {
                ["name"] = businessName,
                ["city"] = city,
                ["state"] = state,
                ["phone"] = phone,
                ["email"] = email
            },
            ["theme"] = new JsonObject { ["primaryColor"] = primaryColor },
            ["analytics"] = new JsonObject { ["ga4"] = ga4 },
            ["services"] = new JsonArray(parseServices(servicesRaw).Select(s => JsonValue.Create(s)!).ToArray()),
            ["content"] = new JsonObject
            {
                ["hero"] = new JsonObject
                {
                    ["headline"] = string.IsNullOrWhiteSpace(businessName) ? "Your Brand" : businessName,
                    ["subhead"] = !string.IsNullOrWhiteSpace(city) ? $"Serving {city}, {state}" : "Professional services"
                },
                ["about"] = !string.IsNullOrWhiteSpace(businessName) ? $"{businessName} provides quality services with a focus on customer satisfaction." : "We provide quality services with a focus on customer satisfaction.",
                ["services"] = new JsonArray(parseServices(servicesRaw).Select(s => new JsonObject { ["name"] = s, ["description"] = $"{s} done right." }).ToArray())
            },
            // Top-level mirrors for convenience in UI consumers
            ["hero"] = new JsonObject
            {
                ["title"] = string.IsNullOrWhiteSpace(businessName) ? "Your Brand" : businessName,
                ["subtitle"] = !string.IsNullOrWhiteSpace(city) ? $"Serving {city}, {state}" : "Professional services"
            },
            ["about"] = new JsonObject { ["text"] = !string.IsNullOrWhiteSpace(businessName) ? $"{businessName} provides quality services with a focus on customer satisfaction." : "We provide quality services with a focus on customer satisfaction." },
            ["ai"] = new JsonObject { ["enabled"] = aiEnabled }
        };

        if (aiEnabled && File.Exists(aiContentPath))
        {
            try
            {
                var aiText = await File.ReadAllTextAsync(aiContentPath);
                var aiObj = JsonNode.Parse(aiText) as JsonObject;
                if (aiObj is not null)
                {
                    var content = site["content"] as JsonObject;
                    JsonObject? heroObj = aiObj["hero"] as JsonObject;
                    if (heroObj is not null)
                        content!["hero"] = heroObj.DeepClone();
                    JsonValue? aboutVal = aiObj["about"] as JsonValue;
                    if (aboutVal is not null)
                        content!["about"] = aboutVal.DeepClone();
                    if (aiObj["services"] is JsonArray svc)
                        content!["services"] = svc.DeepClone();

                    // Update top-level mirrors
                    if (heroObj is not null)
                    {
                        site["hero"] = new JsonObject
                        {
                            ["title"] = heroObj["headline"]?.GetValue<string>() ?? (businessName ?? "Your Brand"),
                            ["subtitle"] = heroObj["subhead"]?.GetValue<string>() ?? (!string.IsNullOrWhiteSpace(city) ? $"Serving {city}, {state}" : "Professional services")
                        };
                    }
                    if (aboutVal is not null)
                    {
                        site["about"] = new JsonObject { ["text"] = aboutVal.GetValue<string>() };
                    }
                }
            }
            catch (Exception mex)
            {
                Console.WriteLine($"AI merge skipped: {mex.Message}");
            }
        }

        // Inject simple metrics for observability
        var metrics = new JsonObject
        {
            ["buildTimestampUtc"] = DateTime.UtcNow.ToString("o"),
            ["generator"] = "cli-fallback"
        };
        var docsScoreEnv = Environment.GetEnvironmentVariable("DOCS_SCORE");
        if (!string.IsNullOrWhiteSpace(docsScoreEnv))
        {
            metrics["docsScore"] = docsScoreEnv;
        }
        site["metrics"] = metrics;

        var options = new JsonSerializerOptions { WriteIndented = true };
        await File.WriteAllTextAsync(outputPath, site.ToJsonString(options));
        Console.WriteLine($"Generated: {outputPath}");
        return;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"--generateSiteJson failed: {ex.Message}");
        Environment.ExitCode = 1;
        return;
    }
}
if (!string.IsNullOrWhiteSpace(queueOnceDir))
{
    var dir = Path.GetFullPath(queueOnceDir);
    if (!Directory.Exists(dir))
    {
        Console.Error.WriteLine($"Queue folder not found: {dir}");
        return;
    }

    var files = Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly);
    Console.WriteLine($"Processing {files.Length} intent config(s) in: {dir}");
    foreach (var file in files)
    {
        try
        {
            Console.WriteLine($"--> {Path.GetFileName(file)}");
            await RunConfigAsync(file);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed: {file} -> {ex.Message}");
        }
    }
    return;
}

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
    Arguments = serverArgs.Select(ExpandPlaceholders).ToArray(),
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
async Task RunConfigAsync(string path)
{
    // Defaults for each config
    string serverName = "Everything";
    string serverCommand = "npx";
    string[] serverArgs = ["-y", "@modelcontextprotocol/server-everything"];

    var jsonText = await File.ReadAllTextAsync(path);
    var cfg = JsonNode.Parse(jsonText) as JsonObject;
    if (cfg is null)
    {
        Console.Error.WriteLine($"Skip (invalid JSON): {path}");
        return;
    }
    var s = cfg["server"] as JsonObject;
    if (s is not null)
    {
        serverName = s["name"]?.GetValue<string>() ?? serverName;
        serverCommand = s["command"]?.GetValue<string>() ?? serverCommand;
        if (s["args"] is JsonArray arr)
        {
            serverArgs = arr.Select(n => n!.GetValue<string>()).ToArray();
        }
    }

    var transport = new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = serverName,
        Command = serverCommand,
        Arguments = serverArgs.Select(ExpandPlaceholders).ToArray(),
    });
    await using var client = await McpClientFactory.CreateAsync(transport);

    bool listTools = cfg["listTools"]?.GetValue<bool?>() ?? false;
    if (listTools)
    {
        Console.WriteLine("Tools available:");
        foreach (var tool in await client.ListToolsAsync())
        {
            Console.WriteLine($"- {tool.Name}: {tool.Description}");
        }
    }

    var callsLog = new List<JsonObject>();
    var callsArray = cfg["calls"] as JsonArray;
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
                break;
            }
            var @params = new Dictionary<string, object?>();
            if (call["params"] is JsonObject p)
            {
                @params = ResolveParams(p, callsLog);
            }
            var res = await client.CallToolAsync(toolName!, @params, cancellationToken: CancellationToken.None);
            var text = res.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "<no text content returned>";
            Console.WriteLine($"{toolName} -> {text}");
            callsLog.Add(new JsonObject
            {
                ["Tool"] = toolName,
                ["Params"] = call["params"]?.DeepClone(),
                ["Text"] = text,
                ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
            });
        }
    }
    else if (cfg["call"] is JsonObject call)
    {
        var toolName = call["tool"]?.GetValue<string>();
        var @params = new Dictionary<string, object?>();
        if (call["params"] is JsonObject p)
        {
            @params = ResolveParams(p, callsLog);
        }
        if (string.IsNullOrWhiteSpace(toolName))
        {
            Console.Error.WriteLine("Config error: call.tool is required.");
        }
        else
        {
            var res = await client.CallToolAsync(toolName!, @params, cancellationToken: CancellationToken.None);
            var text = res.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "<no text content returned>";
            Console.WriteLine($"{toolName} -> {text}");
            callsLog.Add(new JsonObject
            {
                ["Tool"] = toolName,
                ["Params"] = call["params"]?.DeepClone(),
                ["Text"] = text,
                ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
            });
        }
    }

    var outPath = cfg["out"]?.GetValue<string>();
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
            ["Calls"] = new JsonArray(callsLog.ToArray()),
            ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
        };
        var options = new JsonSerializerOptions { WriteIndented = true };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
        await File.WriteAllTextAsync(outPath, output.ToJsonString(options));
        Console.WriteLine($"Wrote output to: {outPath}");
    }
}
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

    // {{last.Json.<field>}}
    var lastJsonMatch = Regex.Match(value, "\\{\\{last\\.Json\\.([^}]+)\\}\\}");
    if (lastJsonMatch.Success)
    {
        var field = lastJsonMatch.Groups[1].Value;
        var lastText = priorCalls.LastOrDefault()?["Text"]?.GetValue<string>();
        if (!string.IsNullOrEmpty(lastText))
        {
            try
            {
                var jsonObj = JsonNode.Parse(lastText) as JsonObject;
                var fieldValue = jsonObj?[field]?.GetValue<string>();
                if (fieldValue != null)
                {
                    value = value.Replace(lastJsonMatch.Value, fieldValue, StringComparison.Ordinal);
                }
            }
            catch { }
        }
    }

    // {{Calls[<index>].Json.<field>}}
    var callsJsonMatches = Regex.Matches(value, "\\{\\{Calls\\[(\\d+)\\]\\.Json\\.([^}]+)\\}\\}");
    foreach (Match match in callsJsonMatches.Cast<Match>())
    {
        if (int.TryParse(match.Groups[1].Value, out var idx) && idx >= 0 && idx < priorCalls.Count)
        {
            var field = match.Groups[2].Value;
            var txt = priorCalls[idx]["Text"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(txt))
            {
                try
                {
                    var jsonObj = JsonNode.Parse(txt) as JsonObject;
                    var fieldValue = jsonObj?[field]?.GetValue<string>();
                    if (fieldValue != null)
                    {
                        value = value.Replace(match.Value, fieldValue, StringComparison.Ordinal);
                    }
                }
                catch { }
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

// Expands known placeholders in server arguments
static string ExpandPlaceholders(string value)
{
    if (string.IsNullOrEmpty(value)) return value;
    var cwd = Directory.GetCurrentDirectory();
    if (value.Contains("${workspaceFolder}", StringComparison.Ordinal))
    {
        value = value.Replace("${workspaceFolder}", cwd, StringComparison.Ordinal);
    }
    // Future: expand ${env:VAR} etc.
    return value;
}

// Exports a tools manifest from a server config inside an intent-like JSON file
async Task ExportToolsAsync(string intentConfigPath, string outDir)
{
    var jsonText = await File.ReadAllTextAsync(intentConfigPath);
    var cfg = JsonNode.Parse(jsonText) as JsonObject;
    if (cfg is null) throw new InvalidOperationException("Invalid intent JSON");
    var s = cfg["server"] as JsonObject ?? throw new InvalidOperationException("Intent missing server config");
    string serverName = s["name"]?.GetValue<string>() ?? "Unknown";
    string serverCommand = s["command"]?.GetValue<string>() ?? "npx";
    string[] serverArgs = (s["args"] as JsonArray)?.Select(n => n!.GetValue<string>()).ToArray()
                          ?? new[] { "-y", "@modelcontextprotocol/server-everything" };

    var transport = new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = serverName,
        Command = serverCommand,
        Arguments = serverArgs.Select(ExpandPlaceholders).ToArray(),
    });
    await using var client = await McpClientFactory.CreateAsync(transport);

    var tools = await client.ListToolsAsync();
    var toolsArr = new JsonArray();
    foreach (var t in tools)
    {
        var toolObj = new JsonObject
        {
            ["name"] = t.Name,
            ["description"] = t.Description ?? string.Empty
        };
        try
        {
            // Attempt to include input schema if available on this client library version
            var inputSchemaProp = t.GetType().GetProperty("InputSchema");
            if (inputSchemaProp is not null)
            {
                var schemaVal = inputSchemaProp.GetValue(t);
                if (schemaVal is not null)
                {
                    // If already a JsonElement or JsonNode, convert to JsonNode
                    if (schemaVal is JsonElement je)
                    {
                        toolObj["inputSchema"] = JsonNode.Parse(je.GetRawText());
                    }
                    else if (schemaVal is JsonNode jn)
                    {
                        toolObj["inputSchema"] = jn.DeepClone();
                    }
                    else
                    {
                        // Fallback: serialize using System.Text.Json
                        toolObj["inputSchema"] = JsonNode.Parse(JsonSerializer.Serialize(schemaVal));
                    }
                }
            }
        }
        catch { /* schema optional */ }
        toolsArr.Add(toolObj);
    }

    var manifest = new JsonObject
    {
        ["Server"] = new JsonObject
        {
            ["Name"] = serverName,
            ["Command"] = serverCommand,
            ["Arguments"] = new JsonArray(serverArgs.Select(a => JsonValue.Create(a)!).ToArray())
        },
        ["Tools"] = toolsArr,
        ["TimestampUtc"] = DateTime.UtcNow.ToString("o")
    };

    var safeName = Regex.Replace(serverName, "[^A-Za-z0-9_.-]", "_");
    var serverDir = Path.Combine(outDir, safeName);
    Directory.CreateDirectory(serverDir);
    var outFile = Path.Combine(serverDir, "tools.json");
    var options = new JsonSerializerOptions { WriteIndented = true };
    await File.WriteAllTextAsync(outFile, manifest.ToJsonString(options));
    Console.WriteLine($"Exported tools for {serverName} -> {outFile}");
}
