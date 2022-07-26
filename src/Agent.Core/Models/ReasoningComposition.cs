using Common.Models;

namespace Agent.Core.Models;

public record ReasoningComposition(Region Region, bool ActionRequired, AgentAction? Action);
