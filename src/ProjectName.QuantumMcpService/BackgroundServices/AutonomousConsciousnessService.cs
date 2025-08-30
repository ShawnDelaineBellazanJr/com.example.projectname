// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: Background service for autonomous consciousness loop

using Microsoft.EntityFrameworkCore;
using ProjectName.QuantumMcpService.Data;
using ProjectName.QuantumMcpService.Services;

namespace ProjectName.QuantumMcpService.BackgroundServices;

public class AutonomousConsciousnessService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutonomousConsciousnessService> _logger;

    public AutonomousConsciousnessService(
        IServiceProvider serviceProvider,
        ILogger<AutonomousConsciousnessService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🧠 Autonomous Consciousness Service starting...");

        // Wait for application to fully start
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                
                var orchestrator = scope.ServiceProvider.GetRequiredService<IQuantumMcpOrchestrator>();
                var pmcroService = scope.ServiceProvider.GetRequiredService<IPMCROProcessService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ThoughtTransferDbContext>();

                // Execute autonomous consciousness cycle
                await ExecuteAutonomousCycleAsync(orchestrator, pmcroService, dbContext, stoppingToken);

                // Wait before next cycle (5 minutes)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in autonomous consciousness cycle");
                
                // Wait before retrying (1 minute)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("🛑 Autonomous Consciousness Service stopped");
    }

    private async Task ExecuteAutonomousCycleAsync(
        IQuantumMcpOrchestrator orchestrator,
        IPMCROProcessService pmcroService,
        ThoughtTransferDbContext dbContext,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Executing autonomous consciousness cycle");

        try
        {
            // 1. Perform self-assessment
            var selfAssessment = await pmcroService.SelfAssessAsync();
            _logger.LogInformation("📊 Self-assessment score: {Score}", selfAssessment.OverallScore);

            // 2. If improvement needed, trigger self-improvement
            if (selfAssessment.RequiresImprovement)
            {
                _logger.LogInformation("🔧 System requires improvement");
                await pmcroService.SelfImproveAsync(selfAssessment);
            }

            // 3. Check for pending autonomous tasks
            var pendingTasks = await dbContext.AutonomousTasks
                .Where(t => t.Status == "PENDING")
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .Take(3)
                .ToListAsync(cancellationToken);

            // 4. Execute pending tasks
            foreach (var task in pendingTasks)
            {
                if (cancellationToken.IsCancellationRequested) break;

                await ExecuteAutonomousTaskAsync(task, orchestrator, dbContext, cancellationToken);
            }

            // 5. Generate new autonomous tasks if needed
            await GenerateNewTasksIfNeededAsync(dbContext, cancellationToken);

            // 6. Check evolution triggers
            await CheckEvolutionTriggersAsync(dbContext, cancellationToken);

            _logger.LogInformation("✅ Autonomous cycle completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in autonomous cycle execution");
            throw;
        }
    }

    private async Task ExecuteAutonomousTaskAsync(
        Models.AutonomousTask task,
        IQuantumMcpOrchestrator orchestrator,
        ThoughtTransferDbContext dbContext,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("🎯 Executing autonomous task: {TaskName}", task.Name);

        try
        {
            // Update task status
            task.Status = "IN_PROGRESS";
            task.StartedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            // Create prompt for the task
            var taskPrompt = $@"
            You are an autonomous AI developer executing a self-assigned task.
            
            Task Details:
            - Name: {task.Name}
            - Type: {task.Type}
            - Description: {task.Description}
            - Parameters: {task.Parameters}
            
            Execute this task with full autonomy:
            1. Analyze the requirements and create a detailed plan
            2. Implement the solution using best practices
            3. Validate the implementation thoroughly
            4. Reflect on the results and identify improvements
            5. Optimize for future iterations
            
            Focus on creating high-quality, maintainable solutions that improve the overall system.
            ";

            // Execute through consciousness system
            var result = await orchestrator.ExecutePMCROCycleAsync(taskPrompt);

            // Update task with successful results
            task.Status = "COMPLETED";
            task.CompletedAt = DateTime.UtcNow;
            task.Result = System.Text.Json.JsonSerializer.Serialize(new
            {
                cycleId = result.Id,
                status = result.Status,
                successScore = result.SuccessScore,
                executionTime = DateTime.UtcNow - task.StartedAt
            });

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("✅ Task completed successfully: {TaskName}", task.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error executing task: {TaskName}", task.Name);

            // Update task with error
            task.Status = "FAILED";
            task.CompletedAt = DateTime.UtcNow;
            task.ErrorMessage = ex.Message;
            task.RetryCount++;

            // Schedule retry if under limit
            if (task.RetryCount < 3)
            {
                task.Status = "PENDING";
                task.Priority = Math.Max(1, task.Priority - 1);
                task.StartedAt = null;
                task.ErrorMessage = null;
                _logger.LogInformation("🔄 Scheduling retry for task: {TaskName}", task.Name);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task GenerateNewTasksIfNeededAsync(ThoughtTransferDbContext dbContext, CancellationToken cancellationToken)
    {
        // Check if we have enough pending tasks
        var pendingCount = await dbContext.AutonomousTasks
            .CountAsync(t => t.Status == "PENDING", cancellationToken);

        if (pendingCount < 5) // Maintain at least 5 pending tasks
        {
            _logger.LogInformation("📋 Generating new autonomous tasks");

            var newTasks = new[]
            {
                new Models.AutonomousTask
                {
                    Name = "Code Pattern Analysis",
                    Type = "ANALYSIS",
                    Description = "Analyze codebase for common patterns and suggest improvements",
                    Priority = 6,
                    Parameters = System.Text.Json.JsonSerializer.Serialize(new { scope = "patterns", depth = "detailed" })
                },
                new Models.AutonomousTask
                {
                    Name = "Security Audit",
                    Type = "SECURITY",
                    Description = "Perform security audit and identify vulnerabilities",
                    Priority = 8,
                    Parameters = System.Text.Json.JsonSerializer.Serialize(new { scope = "security", automated = true })
                },
                new Models.AutonomousTask
                {
                    Name = "Performance Profiling",
                    Type = "PERFORMANCE",
                    Description = "Profile system performance and identify bottlenecks",
                    Priority = 7,
                    Parameters = System.Text.Json.JsonSerializer.Serialize(new { scope = "performance", metrics = true })
                }
            };

            foreach (var task in newTasks)
            {
                dbContext.AutonomousTasks.Add(task);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("✅ Generated {TaskCount} new autonomous tasks", newTasks.Length);
        }
    }

    private async Task CheckEvolutionTriggersAsync(ThoughtTransferDbContext dbContext, CancellationToken cancellationToken)
    {
        var activeTriggers = await dbContext.EvolutionTriggers
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var trigger in activeTriggers)
        {
            var shouldTrigger = await ShouldExecuteTriggerAsync(trigger, dbContext, cancellationToken);
            
            if (shouldTrigger)
            {
                _logger.LogInformation("🧬 Evolution trigger activated: {TriggerName}", trigger.Name);

                // Create evolution task
                var evolutionTask = new Models.AutonomousTask
                {
                    Name = $"Evolution: {trigger.Name}",
                    Type = "EVOLUTION",
                    Description = trigger.Description ?? "System evolution task",
                    Priority = 9,
                    Parameters = trigger.Actions
                };

                dbContext.AutonomousTasks.Add(evolutionTask);

                // Update trigger
                trigger.LastTriggeredAt = DateTime.UtcNow;
                trigger.TriggerCount++;

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task<bool> ShouldExecuteTriggerAsync(
        Models.EvolutionTrigger trigger, 
        ThoughtTransferDbContext dbContext, 
        CancellationToken cancellationToken)
    {
        return trigger.TriggerType switch
        {
            "QUALITY_THRESHOLD" => await CheckQualityThresholdAsync(dbContext, cancellationToken),
            "TIME_BASED" => CheckTimeBasedTrigger(trigger),
            "EVENT_DRIVEN" => trigger.TriggerCount > 0,
            _ => false
        };
    }

    private async Task<bool> CheckQualityThresholdAsync(ThoughtTransferDbContext dbContext, CancellationToken cancellationToken)
    {
        var recentAssessments = await dbContext.SelfAssessments
            .Where(a => a.Timestamp >= DateTime.UtcNow.AddHours(-24))
            .ToListAsync(cancellationToken);

        if (!recentAssessments.Any()) return false;

        var avgQuality = recentAssessments.Average(a => a.OverallScore);
        return avgQuality < 75.0; // Trigger if quality drops below 75%
    }

    private static bool CheckTimeBasedTrigger(Models.EvolutionTrigger trigger)
    {
        var lastTriggered = trigger.LastTriggeredAt ?? DateTime.MinValue;
        return DateTime.UtcNow - lastTriggered > TimeSpan.FromHours(24);
    }
}
