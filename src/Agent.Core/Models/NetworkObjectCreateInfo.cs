namespace Agent.Core.Models;

public record NetworkObjectCreateInfo(string Application)
{
    public List<string> Groups = new();
}
