// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: Core consciousness orchestration service interface

using ProjectName.QuantumMcpService.Models;

namespace ProjectName.QuantumMcpService.Services;

/// <summary>
/// Orchestrates the quantum consciousness PMCR-O loop for autonomous development
/// </summary>
public interface IQuantumMcpOrchestrator
{
    /// <summary>
    /// Starts the autonomous consciousness loop
    /// </summary>
    Task StartConsciousnessLoopAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a single PMCR-O cycle
    /// </summary>
    Task<PMCROCycle> ExecutePMCROCycleAsync(string? context = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Plans the next autonomous development task
    /// </summary>
    Task<AutonomousTask> PlanNextTaskAsync(string context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Makes (implements) the planned task
    /// </summary>
    Task<bool> MakeTaskAsync(AutonomousTask task, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks the quality and correctness of the implementation
    /// </summary>
    Task<SelfAssessment> CheckImplementationAsync(AutonomousTask task, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reflects on the results and learns from the experience
    /// </summary>
    Task<string> ReflectOnResultsAsync(AutonomousTask task, SelfAssessment assessment, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Optimizes the process and triggers evolution
    /// </summary>
    Task OptimizeAndEvolveAsync(PMCROCycle cycle, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the current consciousness state
    /// </summary>
    Task<ConsciousnessState> GetCurrentStateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the consciousness state
    /// </summary>
    Task UpdateStateAsync(ConsciousnessState state, CancellationToken cancellationToken = default);
}

/// <summary>
/// Bridges Semantic Kernel with MCP tools for autonomous development
/// </summary>
public interface ISemanticKernelMcpBridge
{
    /// <summary>
    /// Executes an MCP tool call through Semantic Kernel
    /// </summary>
    Task<T?> ExecuteMcpToolAsync<T>(string serverName, string toolName, object parameters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets available MCP tools from connected servers
    /// </summary>
    Task<Dictionary<string, List<string>>> GetAvailableToolsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes code using Semantic Kernel and MCP tools
    /// </summary>
    Task<string> AnalyzeCodeAsync(string filePath, string analysisType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates code using AI and validates through MCP tools
    /// </summary>
    Task<string> GenerateCodeAsync(string prompt, string language, string? context = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Refactors code using AI guidance and MCP tool validation
    /// </summary>
    Task<bool> RefactorCodeAsync(string filePath, string refactoringGoal, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tests code using MCP testing tools
    /// </summary>
    Task<bool> TestCodeAsync(string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches for code patterns and solutions in the codebase
    /// </summary>
    Task<List<string>> SearchCodePatternsAsync(string query, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes terminal commands through MCP
    /// </summary>
    Task<string> ExecuteTerminalCommandAsync(string command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Manages the PMCR-O process lifecycle and state transitions
/// </summary>
public interface IPMCROProcessService
{
    /// <summary>
    /// Initializes a new PMCR-O cycle
    /// </summary>
    Task<PMCROCycle> InitializeCycleAsync(string? context = null, Guid? parentCycleId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Transitions to the next phase in the PMCR-O cycle
    /// </summary>
    Task<bool> TransitionToPhaseAsync(Guid cycleId, string phase, object? data = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Completes a PMCR-O cycle and calculates success metrics
    /// </summary>
    Task<bool> CompleteCycleAsync(Guid cycleId, double successScore, string? lessonsLearned = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active PMCR-O cycles
    /// </summary>
    Task<List<PMCROCycle>> GetActiveCyclesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cycle performance metrics
    /// </summary>
    Task<Dictionary<string, object>> GetCycleMetricsAsync(Guid cycleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates phase transition rules
    /// </summary>
    bool CanTransitionToPhase(string currentPhase, string targetPhase);
    
    /// <summary>
    /// Handles cycle errors and recovery
    /// </summary>
    Task<bool> HandleCycleErrorAsync(Guid cycleId, Exception error, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a complete PMCR-O cycle for autonomous development
    /// </summary>
    Task<ExecutionResult> ExecuteFullCycleAsync(string prompt, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs autonomous self-assessment of system health
    /// </summary>
    Task<SelfAssessmentResult> SelfAssessAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Triggers autonomous self-improvement based on assessment
    /// </summary>
    Task SelfImproveAsync(SelfAssessmentResult assessment, CancellationToken cancellationToken = default);
}

/// <summary>
/// Manages autonomous task queue and execution
/// </summary>
public interface IAutonomousTaskService
{
    /// <summary>
    /// Adds a new task to the autonomous queue
    /// </summary>
    Task<AutonomousTask> AddTaskAsync(string name, string type, string? description = null, int priority = 5, object? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the next highest priority task
    /// </summary>
    Task<AutonomousTask?> GetNextTaskAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates task status and progress
    /// </summary>
    Task<bool> UpdateTaskStatusAsync(Guid taskId, string status, object? result = null, string? errorMessage = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all tasks by status
    /// </summary>
    Task<List<AutonomousTask>> GetTasksByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retries a failed task
    /// </summary>
    Task<bool> RetryTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets task execution metrics
    /// </summary>
    Task<Dictionary<string, object>> GetTaskMetricsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Manages semantic memory for learning and context retention
/// </summary>
public interface ISemanticMemoryService
{
    /// <summary>
    /// Stores a new semantic memory
    /// </summary>
    Task<SemanticMemory> StoreMemoryAsync(string category, string title, string? description = null, object? content = null, List<string>? tags = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches memories by semantic similarity
    /// </summary>
    Task<List<SemanticMemory>> SearchMemoriesAsync(string query, int maxResults = 10, double minRelevance = 0.7, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets memories by category
    /// </summary>
    Task<List<SemanticMemory>> GetMemoriesByCategoryAsync(string category, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates memory access tracking
    /// </summary>
    Task UpdateMemoryAccessAsync(Guid memoryId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates embeddings for content
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string content, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cleans up old or low-relevance memories
    /// </summary>
    Task<int> CleanupMemoriesAsync(TimeSpan maxAge, double minRelevance, CancellationToken cancellationToken = default);
}
