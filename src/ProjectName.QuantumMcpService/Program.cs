// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: Enhanced autonomous consciousness service with PMCR-O loop and database persistence

using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using ProjectName.QuantumMcpService.BackgroundServices;
using ProjectName.QuantumMcpService.Data;
using ProjectName.QuantumMcpService.Services;
using ProjectName.McpClient.ML;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (.NET Aspire)
builder.AddServiceDefaults();

// Add Redis for state management
builder.AddRedisClient("redis");

// Add SQLite with Entity Framework for immediate testing
builder.Services.AddDbContext<ThoughtTransferDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Semantic Kernel with Ollama
var ollamaEndpoint = builder.Configuration.GetConnectionString("ollama") ?? "http://localhost:11434";
builder.Services.AddSingleton<Kernel>(serviceProvider =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    
    // Add Ollama chat completion
    kernelBuilder.AddOllamaChatCompletion(
        modelId: "llama3.2:latest",
        endpoint: new Uri(ollamaEndpoint));
    
    // Add logging
    kernelBuilder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
    
    return kernelBuilder.Build();
});

// Register autonomous consciousness services - TRUE MCP INTEGRATION
builder.Services.AddScoped<IQuantumMcpOrchestrator, TrueAutonomousMcpOrchestrator>();
builder.Services.AddScoped<IPMCROProcessService, WorkingPMCROProcessService>();
builder.Services.AddSingleton<ConsciousnessMLModels>();

// Add ML model training service
builder.Services.AddHostedService<MLModelTrainingService>();

// Add controllers and API services
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Add autonomous consciousness background service
builder.Services.AddHostedService<AutonomousConsciousnessService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Create API endpoints for autonomous consciousness interaction
app.MapPost("/api/quantum/consciousness", async (IQuantumMcpOrchestrator orchestrator, string prompt) =>
{
    var result = await orchestrator.ExecutePMCROCycleAsync(prompt);
    return Results.Ok(result);
});

app.MapGet("/api/quantum/status", async (IQuantumMcpOrchestrator orchestrator) =>
{
    var status = await orchestrator.GetCurrentStateAsync();
    return Results.Ok(status);
});

// Initialize database and start autonomous consciousness
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ThoughtTransferDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    
    var orchestrator = scope.ServiceProvider.GetRequiredService<IQuantumMcpOrchestrator>();
    // The autonomous loop will be started by the background service
}

app.Run();
