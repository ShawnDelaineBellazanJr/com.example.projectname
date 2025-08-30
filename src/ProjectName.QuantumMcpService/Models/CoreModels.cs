// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: Core data models for consciousness state management

using System.ComponentModel.DataAnnotations;

namespace ProjectName.QuantumMcpService.Models;

/// <summary>
/// Represents a snapshot of consciousness state at a specific moment
/// </summary>
public class ConsciousnessState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string Phase { get; set; } = string.Empty; // PLAN, MAKE, CHECK, REFLECT, OPTIMIZE
    
    [Required]
    public string State { get; set; } = "{}"; // JSON representation of current state
    
    public string? Metadata { get; set; } // Additional context as JSON
    
    public double ConfidenceLevel { get; set; } = 0.0;
    
    public string? Context { get; set; }
}

/// <summary>
/// Tracks a complete Plan-Make-Check-Reflect-Optimize cycle
/// </summary>
public class PMCROCycle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    [Required]
    public string Status { get; set; } = "PLANNING"; // PLANNING, MAKING, CHECKING, REFLECTING, OPTIMIZING, COMPLETED, FAILED
    
    public string? PlanData { get; set; } // JSON
    public string? MakeData { get; set; } // JSON
    public string? CheckData { get; set; } // JSON
    public string? ReflectData { get; set; } // JSON
    public string? OptimizeData { get; set; } // JSON
    
    public double SuccessScore { get; set; } = 0.0;
    
    public string? LessonsLearned { get; set; }
    
    public Guid? ParentCycleId { get; set; } // For nested cycles
}

/// <summary>
/// Represents an autonomous task in the development queue
/// </summary>
public class AutonomousTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public string Type { get; set; } = string.Empty; // CODE_GENERATION, ANALYSIS, REFACTORING, TESTING, etc.
    
    [Required]
    public string Status { get; set; } = "PENDING"; // PENDING, IN_PROGRESS, COMPLETED, FAILED, CANCELLED
    
    public int Priority { get; set; } = 5; // 1-10 scale
    
    public string? Parameters { get; set; } // JSON
    public string? Result { get; set; } // JSON
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public Guid? PMCROCycleId { get; set; } // Associated PMCRO cycle
    
    public string? ErrorMessage { get; set; }
    
    public int RetryCount { get; set; } = 0;
}

/// <summary>
/// Logs interactions with MCP servers
/// </summary>
public class McpInteraction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string ServerName { get; set; } = string.Empty;
    
    [Required]
    public string ToolName { get; set; } = string.Empty;
    
    public string? Request { get; set; } // JSON
    public string? Response { get; set; } // JSON
    
    public bool IsSuccessful { get; set; } = false;
    
    public string? ErrorMessage { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public Guid? TaskId { get; set; } // Associated autonomous task
}

/// <summary>
/// Stores semantic memories for context and learning
/// </summary>
public class SemanticMemory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string Category { get; set; } = string.Empty; // CODE_PATTERN, SOLUTION, ERROR, INSIGHT, etc.
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? Content { get; set; } // JSON
    
    public float[]? Embedding { get; set; } // Vector embedding for similarity search
    
    public double RelevanceScore { get; set; } = 0.0;
    
    public int AccessCount { get; set; } = 0;
    
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Tracks self-assessment metrics and quality scores
/// </summary>
public class SelfAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string AssessmentType { get; set; } = string.Empty; // CODE_QUALITY, TASK_COMPLETION, LEARNING_PROGRESS, etc.
    
    public string? Metrics { get; set; } // JSON with detailed metrics
    
    public double OverallScore { get; set; } = 0.0; // 0-100 scale
    
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? ImprovementAreas { get; set; }
    
    public bool RequiresImprovement => OverallScore < 75.0 || !string.IsNullOrEmpty(Weaknesses);
    
    public Guid? RelatedTaskId { get; set; }
    public Guid? RelatedCycleId { get; set; }
}

/// <summary>
/// Defines triggers for autonomous evolution and improvement
/// </summary>
public class EvolutionTrigger
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public string TriggerType { get; set; } = string.Empty; // QUALITY_THRESHOLD, TIME_BASED, EVENT_DRIVEN, etc.
    
    public string? Conditions { get; set; } // JSON defining trigger conditions
    public string? Actions { get; set; } // JSON defining actions to take
    
    public bool IsActive { get; set; } = true;
    
    public int Priority { get; set; } = 5; // 1-10 scale
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTriggeredAt { get; set; }
    
    public int TriggerCount { get; set; } = 0;
}

/// <summary>
/// Result of executing a full PMCRO cycle
/// </summary>
public class ExecutionResult
{
    public bool Success { get; set; }
    public PMCROCycle Cycle { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Results { get; set; } = new();
    public double OverallScore { get; set; }
    public List<string> LessonsLearned { get; set; } = new();
    
    // Compatibility properties
    public Guid CycleId => Cycle.Id;
    public string FinalOutput => Results.ContainsKey("output") ? Results["output"]?.ToString() ?? "" : "";
    public double QualityScore => OverallScore;
}

/// <summary>
/// Result of self-assessment process
/// </summary>
public class SelfAssessmentResult
{
    public bool Success { get; set; }
    public SelfAssessment Assessment { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public List<string> ImprovementRecommendations { get; set; } = new();
    public double ConfidenceScore { get; set; }
    
    // Compatibility properties
    public double OverallScore => Assessment.OverallScore;
    public bool RequiresImprovement => Assessment.RequiresImprovement;
    public string? ImprovementAreas => string.Join(", ", ImprovementRecommendations);
}
