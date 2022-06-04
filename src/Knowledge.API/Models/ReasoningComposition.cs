namespace Knowledge.API.Models;

/// <summary>
/// Result of reasoning process
/// </summary>
public record ReasoningComposition(bool ActionRequired, AgentAction? Action = null);

public record AgentAction(int Scale);
