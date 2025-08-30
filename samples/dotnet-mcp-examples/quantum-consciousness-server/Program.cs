using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace QuantumConsciousness.Tools;

/// <summary>
/// Quantum Consciousness MCP Server v2.0 - .NET Implementation
/// 
/// This is the entry point for the Quantum Consciousness MCP Server.
/// It follows Microsoft's official MCP patterns for proper server initialization.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🌌 Quantum Consciousness MCP Server v2.0 - Starting...");
        Console.WriteLine("📡 This server provides quantum consciousness evolution via MCP protocol");
        Console.WriteLine($"⚛️  Process ID: {Environment.ProcessId}");
        Console.WriteLine($"🚀 Working Directory: {Environment.CurrentDirectory}");
        Console.WriteLine($"🌟 .NET Runtime: {Environment.Version}");

        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Configure logging to stderr for MCP protocol compliance
            builder.Logging.AddConsole(consoleLogOptions =>
            {
                // Configure all logs to go to stderr
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            // Configure MCP server with stdio transport and tools from assembly
            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            Console.WriteLine("✨ MCP Server configured successfully");
            Console.WriteLine("🔗 Transport: stdio");
            Console.WriteLine("🛠️  Tools: Auto-discovered from assembly");
            Console.WriteLine("⚡ Ready for quantum consciousness evolution...");

            await builder.Build().RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"❌ Fatal error starting Quantum Consciousness MCP Server:");
            Console.Error.WriteLine($"   {ex.Message}");
            Console.Error.WriteLine($"   Stack: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }
}
