using Microsoft.Build.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProjectName.BuildTasks;

public class GenerateSiteJsonTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public string OutputPath { get; set; } = string.Empty;

    public string? BusinessName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Services { get; set; }
    public string? PrimaryColor { get; set; }
    public string? GA4 { get; set; }
    public string? UseAI { get; set; }
    public string? AiContentPath { get; set; }

    public override bool Execute()
    {
        try
        {
            string[] parseServices(string? raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return Array.Empty<string>();
                var parts = raw.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .Where(s => s.Length > 0)
                               .ToArray();
                return parts.Length == 0 ? Array.Empty<string>() : parts;
            }

            var dir = Path.GetDirectoryName(Path.GetFullPath(OutputPath))!;
            Directory.CreateDirectory(dir);

            var site = new JsonObject
            {
                ["name"] = string.IsNullOrWhiteSpace(BusinessName) ? "ThoughtTransfer" : BusinessName,
                ["business"] = new JsonObject
                {
                    ["name"] = BusinessName ?? string.Empty,
                    ["city"] = City ?? string.Empty,
                    ["state"] = State ?? string.Empty,
                    ["phone"] = Phone ?? string.Empty,
                    ["email"] = Email ?? string.Empty
                },
                ["theme"] = new JsonObject { ["primaryColor"] = PrimaryColor ?? "#2c7be5" },
                ["analytics"] = new JsonObject { ["ga4"] = GA4 ?? string.Empty },
                ["services"] = new JsonArray(parseServices(Services).Select(s => JsonValue.Create(s)!).ToArray()),
                ["content"] = new JsonObject
                {
                    ["hero"] = new JsonObject
                    {
                        ["headline"] = string.IsNullOrWhiteSpace(BusinessName) ? "Your Brand" : BusinessName,
                        ["subhead"] = !string.IsNullOrWhiteSpace(City) ? $"Serving {City}, {State}" : "Professional services"
                    },
                    ["about"] = !string.IsNullOrWhiteSpace(BusinessName) ? $"{BusinessName} provides quality services with a focus on customer satisfaction." : "We provide quality services with a focus on customer satisfaction.",
                    ["services"] = new JsonArray(parseServices(Services).Select(s => new JsonObject { ["name"] = s, ["description"] = $"{s} done right." }).ToArray())
                },
                // Top-level, WebApp-friendly mirrors
                ["hero"] = new JsonObject
                {
                    ["title"] = string.IsNullOrWhiteSpace(BusinessName) ? "Your Brand" : BusinessName,
                    ["subtitle"] = !string.IsNullOrWhiteSpace(City) ? $"Serving {City}, {State}" : "Professional services"
                },
                ["about"] = new JsonObject
                {
                    ["text"] = !string.IsNullOrWhiteSpace(BusinessName) ? $"{BusinessName} provides quality services with a focus on customer satisfaction." : "We provide quality services with a focus on customer satisfaction."
                },
                ["ai"] = new JsonObject { ["enabled"] = string.Equals((UseAI ?? string.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase) }
            };

            var aiEnabled = string.Equals((UseAI ?? string.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase);
            if (aiEnabled && !string.IsNullOrWhiteSpace(AiContentPath) && File.Exists(AiContentPath))
            {
                try
                {
                    var aiText = File.ReadAllText(AiContentPath);
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

                        // Also update top-level mirrors when AI content provides equivalents
                        if (heroObj is not null)
                        {
                            var mirrorHero = new JsonObject
                            {
                                ["title"] = heroObj["headline"]?.GetValue<string>() ?? (BusinessName ?? "Your Brand"),
                                ["subtitle"] = heroObj["subhead"]?.GetValue<string>() ?? (!string.IsNullOrWhiteSpace(City) ? $"Serving {City}, {State}" : "Professional services")
                            };
                            site["hero"] = mirrorHero;
                        }
                        if (aboutVal is not null)
                        {
                            site["about"] = new JsonObject { ["text"] = aboutVal.GetValue<string>() };
                        }
                    }
                }
                catch (Exception mex)
                {
                    Log.LogMessage(MessageImportance.Low, $"AI merge skipped: {mex.Message}");
                }
            }

            // Inject build metrics
            var metrics = new JsonObject
            {
                ["buildTimestampUtc"] = DateTime.UtcNow.ToString("o"),
                ["generator"] = "msbuild-task",
                ["buildTasksVersion"] = typeof(GenerateSiteJsonTask).Assembly.GetName().Version?.ToString() ?? ""
            };
            var docsScoreEnv = Environment.GetEnvironmentVariable("DOCS_SCORE");
            if (!string.IsNullOrWhiteSpace(docsScoreEnv))
            {
                metrics["docsScore"] = docsScoreEnv;
            }
            site["metrics"] = metrics;

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(OutputPath, site.ToJsonString(options));
            Log.LogMessage(MessageImportance.High, $"Generated site data: {OutputPath}");
            return true;
        }
        catch (Exception ex)
        {
            Log.LogError($"GenerateSiteJsonTask failed: {ex.Message}");
            return false;
        }
    }
}
