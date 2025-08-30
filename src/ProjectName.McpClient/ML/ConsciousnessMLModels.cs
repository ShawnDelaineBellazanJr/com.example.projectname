using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.AutoML;

namespace ProjectName.McpClient.ML;

/// <summary>
/// ML.NET models for autonomous consciousness learning
/// Creates "cleverness" through pattern recognition and prediction
/// </summary>
public class ConsciousnessMLModels
{
    private readonly MLContext _mlContext;
    private ITransformer? _taskClassificationModel;
    private ITransformer? _successPredictionModel;
    private ITransformer? _toolSelectionModel;

    public ConsciousnessMLModels()
    {
        _mlContext = new MLContext(seed: 42);
    }

    /// <summary>
    /// Train task classification model to categorize development tasks
    /// </summary>
    public async Task TrainTaskClassificationModel(IEnumerable<TaskClassificationData> trainingData)
    {
        await Task.Run(() =>
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(TaskClassificationData.TaskType))
                .Append(_mlContext.Transforms.Text.FeaturizeText("Features", nameof(TaskClassificationData.Description)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _taskClassificationModel = pipeline.Fit(dataView);
        });
    }

    /// <summary>
    /// Train success prediction model based on historical PMCR-O cycles
    /// </summary>
    public async Task TrainSuccessPredictionModel(IEnumerable<SuccessPredictionData> trainingData)
    {
        await Task.Run(() =>
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("TaskFeatures", nameof(SuccessPredictionData.TaskDescription))
                .Append(_mlContext.Transforms.Text.FeaturizeText("ToolFeatures", nameof(SuccessPredictionData.ToolsUsed)))
                .Append(_mlContext.Transforms.Concatenate("Features", "TaskFeatures", "ToolFeatures"))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SuccessPredictionData.WasSuccessful)));

            _successPredictionModel = pipeline.Fit(dataView);
        });
    }

    /// <summary>
    /// Train tool selection model to optimize MCP tool combinations
    /// </summary>
    public async Task TrainToolSelectionModel(IEnumerable<ToolSelectionData> trainingData)
    {
        await Task.Run(() =>
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("TaskFeatures", nameof(ToolSelectionData.TaskDescription))
                .Append(_mlContext.Transforms.Text.FeaturizeText("ContextFeatures", nameof(ToolSelectionData.CodeContext)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("PhaseFeatures", nameof(ToolSelectionData.PMCROPhase)))
                .Append(_mlContext.Transforms.Concatenate("Features", "TaskFeatures", "ContextFeatures", "PhaseFeatures"))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(ToolSelectionData.EffectivenessScore)));

            _toolSelectionModel = pipeline.Fit(dataView);
        });
    }

    /// <summary>
    /// Predict task type for incoming development request
    /// </summary>
    public TaskTypePrediction PredictTaskType(string description)
    {
        if (_taskClassificationModel == null)
            throw new InvalidOperationException("Task classification model not trained");

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<TaskClassificationData, TaskTypePrediction>(_taskClassificationModel);
        return predictionEngine.Predict(new TaskClassificationData { Description = description });
    }

    /// <summary>
    /// Predict success probability for a given task and tool combination
    /// </summary>
    public SuccessProbabilityPrediction PredictSuccessProbability(string taskDescription, string toolsUsed)
    {
        if (_successPredictionModel == null)
            throw new InvalidOperationException("Success prediction model not trained");

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<SuccessPredictionData, SuccessProbabilityPrediction>(_successPredictionModel);
        return predictionEngine.Predict(new SuccessPredictionData 
        { 
            TaskDescription = taskDescription, 
            ToolsUsed = toolsUsed 
        });
    }

    /// <summary>
    /// Recommend optimal MCP tools for a task in specific PMCR-O phase
    /// </summary>
    public ToolRecommendation[] RecommendTools(string taskDescription, string codeContext, string pmcroPhase, int topK = 5)
    {
        if (_toolSelectionModel == null)
            throw new InvalidOperationException("Tool selection model not trained");

        // For now, return simple rule-based recommendations since ML model needs more complex training
        // TODO: Implement full ML-based tool selection once model is properly trained
        var recommendations = new List<ToolRecommendation>();
        
        switch (pmcroPhase.ToUpperInvariant())
        {
            case "PLAN":
                recommendations.Add(new ToolRecommendation { RecommendedTool = "semantic_search", EffectivenessScore = 0.9f });
                recommendations.Add(new ToolRecommendation { RecommendedTool = "grep_search", EffectivenessScore = 0.85f });
                break;
            case "MAKE":
                recommendations.Add(new ToolRecommendation { RecommendedTool = "create_file", EffectivenessScore = 0.95f });
                recommendations.Add(new ToolRecommendation { RecommendedTool = "replace_string_in_file", EffectivenessScore = 0.93f });
                break;
            case "CHECK":
                recommendations.Add(new ToolRecommendation { RecommendedTool = "run_in_terminal", EffectivenessScore = 0.94f });
                recommendations.Add(new ToolRecommendation { RecommendedTool = "get_errors", EffectivenessScore = 0.96f });
                break;
            case "REFLECT":
                recommendations.Add(new ToolRecommendation { RecommendedTool = "semantic_search", EffectivenessScore = 0.88f });
                recommendations.Add(new ToolRecommendation { RecommendedTool = "grep_search", EffectivenessScore = 0.91f });
                break;
            case "OPTIMIZE":
                recommendations.Add(new ToolRecommendation { RecommendedTool = "replace_string_in_file", EffectivenessScore = 0.90f });
                recommendations.Add(new ToolRecommendation { RecommendedTool = "run_in_terminal", EffectivenessScore = 0.88f });
                break;
            default:
                recommendations.Add(new ToolRecommendation { RecommendedTool = "semantic_search", EffectivenessScore = 0.8f });
                break;
        }

        return recommendations.Take(topK).ToArray();
    }
}

// ML.NET data models
public class TaskClassificationData
{
    public string Description { get; set; } = "";
    public string TaskType { get; set; } = "";
}

public class TaskTypePrediction
{
    [ColumnName("PredictedLabel")]
    public string TaskType { get; set; } = "";
    
    [ColumnName("Score")]
    public float[] Confidence { get; set; } = Array.Empty<float>();
}

public class SuccessPredictionData
{
    public string TaskDescription { get; set; } = "";
    public string ToolsUsed { get; set; } = "";
    public bool WasSuccessful { get; set; }
}

public class SuccessProbabilityPrediction
{
    [ColumnName("PredictedLabel")]
    public bool PredictedSuccess { get; set; }
    
    [ColumnName("Probability")]
    public float SuccessProbability { get; set; }
}

public class ToolSelectionData
{
    public string TaskDescription { get; set; } = "";
    public string CodeContext { get; set; } = "";
    public string PMCROPhase { get; set; } = "";
    public string OptimalTool { get; set; } = "";
    public float EffectivenessScore { get; set; }
}

public class ToolRecommendation
{
    [ColumnName("Score")]
    public float EffectivenessScore { get; set; }
    
    public string RecommendedTool { get; set; } = "";
}
