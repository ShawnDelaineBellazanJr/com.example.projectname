using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("🚀 Starting Basic Calculator MCP Server...");
Console.WriteLine("📡 This server provides basic arithmetic operations via MCP protocol");

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr for MCP protocol compliance
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add MCP server services
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

Console.WriteLine("✅ MCP Server configured and ready");
Console.WriteLine("🔌 Waiting for client connections via stdio...");

await builder.Build().RunAsync();
