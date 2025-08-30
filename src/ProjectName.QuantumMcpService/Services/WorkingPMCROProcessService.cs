// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Working PMCR-O process service implementation

using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using ProjectName.QuantumMcpService.Data;
using ProjectName.QuantumMcpService.Models;
using System.Text.Json;

namespace ProjectName.QuantumMcpService.Services;

public class WorkingPMCROProcessService : IPMCROProcessService
{
    private readonly ThoughtTransferDbContext _dbContext;
    private readonly Kernel _kernel;
    private readonly ILogger<WorkingPMCROProcessService> _logger;

    public WorkingPMCROProcessService(
        ThoughtTransferDbContext dbContext,
        Kernel kernel,
        ILogger<WorkingPMCROProcessService> logger)
    {
        _dbContext = dbContext;
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<PMCROCycle> InitializeCycleAsync(string? context = null, Guid? parentCycleId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚀 Initializing new PMCR-O cycle");

        var cycle = new PMCROCycle
        {
            Status = "PLANNING",
            PlanData = JsonSerializer.Serialize(new { context = context ?? "Autonomous cycle", parentCycleId }),
            ParentCycleId = parentCycleId
        };

        _dbContext.PMCROCycles.Add(cycle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ PMCR-O cycle {CycleId} initialized", cycle.Id);
        return cycle;
    }

    public async Task<bool> TransitionToPhaseAsync(Guid cycleId, string phase, object? data = null, CancellationToken cancellationToken = default)
    {
        var cycle = await _dbContext.PMCROCycles.FindAsync(new object[] { cycleId }, cancellationToken);
        if (cycle == null) return false;

        _logger.LogInformation("🔄 Transitioning cycle {CycleId} to {Phase}", cycleId, phase);

        cycle.Status = phase.ToUpper();
        
        switch (phase.ToUpper())
        {
            case "MAKING":
                cycle.MakeData = JsonSerializer.Serialize(data ?? new { timestamp = DateTime.UtcNow });
                break;
            case "CHECKING":
                cycle.CheckData = JsonSerializer.Serialize(data ?? new { timestamp = DateTime.UtcNow });
                break;
            case "REFLECTING":
                cycle.ReflectData = JsonSerializer.Serialize(data ?? new { timestamp = DateTime.UtcNow });
                break;
            case "OPTIMIZING":
                cycle.OptimizeData = JsonSerializer.Serialize(data ?? new { timestamp = DateTime.UtcNow });
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CompleteCycleAsync(Guid cycleId, double successScore, string? lessonsLearned = null, CancellationToken cancellationToken = default)
    {
        var cycle = await _dbContext.PMCROCycles.FindAsync(new object[] { cycleId }, cancellationToken);
        if (cycle == null) return false;

        _logger.LogInformation("🎯 Completing cycle {CycleId} with score {Score}", cycleId, successScore);

        cycle.Status = "COMPLETED";
        cycle.EndTime = DateTime.UtcNow;
        cycle.SuccessScore = successScore;
        cycle.LessonsLearned = lessonsLearned;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ExecutionResult> ExecuteFullCycleAsync(string context, CancellationToken cancellationToken = default)
    {
        var cycle = await InitializeCycleAsync(context, cancellationToken: cancellationToken);
        
        try
        {
            // Plan
            await TransitionToPhaseAsync(cycle.Id, "PLANNING", new { phase = "plan", context }, cancellationToken);
            var planResult = await PlanAsync(context, cancellationToken);
            
            // Make
            await TransitionToPhaseAsync(cycle.Id, "MAKING", new { phase = "make", plan = planResult }, cancellationToken);
            var makeResult = await MakeAsync(planResult, cancellationToken);
            
            // Check
            await TransitionToPhaseAsync(cycle.Id, "CHECKING", new { phase = "check", implementation = makeResult }, cancellationToken);
            var checkResult = await CheckAsync(makeResult, cancellationToken);
            
            // Reflect
            await TransitionToPhaseAsync(cycle.Id, "REFLECTING", new { phase = "reflect", check = checkResult }, cancellationToken);
            var reflectResult = await ReflectAsync(checkResult, cancellationToken);
            
            // Optimize
            await TransitionToPhaseAsync(cycle.Id, "OPTIMIZING", new { phase = "optimize", reflection = reflectResult }, cancellationToken);
            var optimizeResult = await OptimizeAsync(reflectResult, cancellationToken);
            
            var quality = CalculateQualityScore(checkResult, reflectResult, optimizeResult);
            await CompleteCycleAsync(cycle.Id, quality, optimizeResult, cancellationToken);
            
            return new ExecutionResult
            {
                Cycle = cycle,
                Success = true,
                Results = new Dictionary<string, object> { ["output"] = optimizeResult },
                OverallScore = quality
            };
        }
        catch (Exception ex)
        {
            await HandleCycleErrorAsync(cycle.Id, ex, cancellationToken);
            return new ExecutionResult
            {
                Cycle = cycle,
                Success = false,
                Results = new Dictionary<string, object> { ["output"] = $"Error: {ex.Message}" },
                OverallScore = 0.0
            };
        }
    }

    public async Task<SelfAssessmentResult> SelfAssessAsync(CancellationToken cancellationToken = default)
    {
        var recentCycles = await _dbContext.PMCROCycles
            .Where(c => c.EndTime >= DateTime.UtcNow.AddHours(-24))
            .OrderByDescending(c => c.EndTime)
            .Take(10)
            .ToListAsync(cancellationToken);

        var avgScore = recentCycles.Any() ? recentCycles.Average(c => c.SuccessScore) : 75.0;
        var requiresImprovement = avgScore < 80.0;
        
        var areas = new List<string>();
        if (avgScore < 60) areas.Add("Basic execution quality");
        if (avgScore < 75) areas.Add("Planning effectiveness");
        if (avgScore < 85) areas.Add("Optimization strategies");

        return new SelfAssessmentResult
        {
            Success = true,
            Assessment = new SelfAssessment
            {
                OverallScore = avgScore,
                Weaknesses = requiresImprovement ? "Performance below threshold" : null,
                ImprovementAreas = string.Join(", ", areas)
            },
            ImprovementRecommendations = areas
        };
    }

    public async Task SelfImproveAsync(SelfAssessmentResult assessment, CancellationToken cancellationToken = default)
    {
        if (!assessment.RequiresImprovement)
        {
            _logger.LogInformation("✅ System performance is optimal - no improvements needed");
            return;
        }

        var improvementPlan = $"Improvement needed in: {string.Join(", ", assessment.ImprovementAreas)}";
        _logger.LogInformation("🔧 Self-improvement plan: {Plan}", improvementPlan);
        
        // Store improvement trigger
        var trigger = new EvolutionTrigger
        {
            Name = "Self-Improvement Trigger",
            TriggerType = "SELF_ASSESSMENT",
            Description = improvementPlan,
            Conditions = JsonSerializer.Serialize(assessment),
            Actions = JsonSerializer.Serialize(new { action = "improve_system", areas = assessment.ImprovementAreas }),
            Priority = 9
        };

        _dbContext.EvolutionTriggers.Add(trigger);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<PMCROCycle>> GetActiveCyclesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PMCROCycles
            .Where(c => c.Status != "COMPLETED" && c.Status != "FAILED")
            .OrderBy(c => c.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetCycleMetricsAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        var cycle = await _dbContext.PMCROCycles.FindAsync(new object[] { cycleId }, cancellationToken);
        if (cycle == null) return new Dictionary<string, object>();

        return new Dictionary<string, object>
        {
            ["cycleId"] = cycle.Id,
            ["status"] = cycle.Status,
            ["duration"] = cycle.EndTime?.Subtract(cycle.StartTime).TotalMinutes ?? 0,
            ["successScore"] = cycle.SuccessScore,
            ["lessonsLearned"] = cycle.LessonsLearned ?? "None"
        };
    }

    public bool CanTransitionToPhase(string currentPhase, string targetPhase)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            ["PLANNING"] = new[] { "MAKING", "FAILED" },
            ["MAKING"] = new[] { "CHECKING", "FAILED" },
            ["CHECKING"] = new[] { "REFLECTING", "FAILED" },
            ["REFLECTING"] = new[] { "OPTIMIZING", "FAILED" },
            ["OPTIMIZING"] = new[] { "COMPLETED", "FAILED" },
            ["FAILED"] = new[] { "PLANNING" }, // Allow restart
            ["COMPLETED"] = new[] { "PLANNING" } // Allow new cycle
        };

        return validTransitions.ContainsKey(currentPhase) && 
               validTransitions[currentPhase].Contains(targetPhase);
    }

    public async Task<bool> HandleCycleErrorAsync(Guid cycleId, Exception error, CancellationToken cancellationToken = default)
    {
        var cycle = await _dbContext.PMCROCycles.FindAsync(new object[] { cycleId }, cancellationToken);
        if (cycle == null) return false;

        _logger.LogError(error, "❌ Error in cycle {CycleId}", cycleId);

        cycle.Status = "FAILED";
        cycle.EndTime = DateTime.UtcNow;
        cycle.LessonsLearned = $"Error: {error.Message}";

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<string> PlanAsync(string context, CancellationToken cancellationToken = default)
    {
        var prompt = $"Create a detailed plan for: {context}";
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.GetValue<string>() ?? "No plan generated";
    }

    public async Task<string> MakeAsync(string plan, CancellationToken cancellationToken = default)
    {
        var prompt = $"Implement this plan: {plan}";
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.GetValue<string>() ?? "No implementation generated";
    }

    public async Task<string> CheckAsync(string implementation, CancellationToken cancellationToken = default)
    {
        var prompt = $"Check and validate this implementation: {implementation}";
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.GetValue<string>() ?? "No validation results";
    }

    public async Task<string> ReflectAsync(string checkResults, CancellationToken cancellationToken = default)
    {
        var prompt = $"Reflect on these check results and identify insights: {checkResults}";
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.GetValue<string>() ?? "No reflection generated";
    }

    public async Task<string> OptimizeAsync(string reflections, CancellationToken cancellationToken = default)
    {
        var prompt = $"Optimize based on these reflections: {reflections}";
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
        return result.GetValue<string>() ?? "No optimizations identified";
    }

    private double CalculateQualityScore(string checkResult, string reflectResult, string optimizeResult)
    {
        // Simple quality scoring based on content length and keywords
        var score = 50.0; // Base score
        
        if (!string.IsNullOrEmpty(checkResult) && checkResult.Length > 50) score += 15;
        if (!string.IsNullOrEmpty(reflectResult) && reflectResult.Length > 50) score += 15;
        if (!string.IsNullOrEmpty(optimizeResult) && optimizeResult.Length > 50) score += 20;
        
        return Math.Min(score, 100.0);
    }
}


