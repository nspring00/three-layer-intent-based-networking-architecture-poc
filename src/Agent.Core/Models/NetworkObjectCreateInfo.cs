namespace Agent.Core.Models;

public record NetworkObjectCreateInfo(int Id, string Application)
{
    public List<string> Groups = new();
}
