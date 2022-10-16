namespace Common.Web.AspNetCore;

public static class HostEnvironmentExtensions
{
    const string DockerEnvironmentName = "Docker";
    const string EcsEnvironmentName = "ECS";

    public static bool IsDocker(this IHostEnvironment environment)
    {
        return environment.IsEnvironment(DockerEnvironmentName);
    }

    public static bool IsDockerSimulation(this IHostEnvironment environment)
    {
        return environment.IsEnvironment(DockerEnvironmentName); // TODO change this
    }

    public static bool IsEcs(this IHostEnvironment environment)
    {
        return environment.IsEnvironment(EcsEnvironmentName);
    }
}
