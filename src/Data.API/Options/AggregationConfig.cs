namespace Data.API.Options;

public class AggregationConfig
{
    public int UpdateInterval { get; set; } = 2500;
    public int AfterKnowledgeUpdateTimeout { get; set; } = 0;
}
