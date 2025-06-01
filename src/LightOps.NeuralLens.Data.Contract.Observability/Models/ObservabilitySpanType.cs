namespace LightOps.NeuralLens.Data.Contract.Observability.Models;

/// <summary>
/// Represents the type of span in the observability system.
/// </summary>
public enum ObservabilitySpanType
{
    /// <summary>
    /// Specific span type not specified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Represents a client call to Generative AI model or service that generates a response or requests a tool call based on the input prompt.
    /// </summary>
    GenAiInference = 1,

    /// <summary>
    /// Represents a client call to Generative AI model or service that generates embeddings based on the input text.
    /// </summary>
    GenAiEmbeddings = 2,

    /// <summary>
    /// Represents a tool execution in the context of Generative AI, where the tool is invoked to perform a specific action or retrieve information based on the input provided.
    /// </summary>
    GenAiExecuteTool = 3,

    /// <summary>
    /// Represents an invocation of a Generative AI agent, which may involve multiple steps or interactions with the model to achieve a specific task or goal.
    /// </summary>
    GenAiInvokeAgent = 4,
}