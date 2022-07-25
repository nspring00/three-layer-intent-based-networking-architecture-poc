namespace Common.Web.AspNetCore;

public static class HostEnvironmentExtensions
{
    const string DockerEnvironmentName = "Docker";

    public static bool IsDocker(this IHostEnvironment environment)
    {
        return environment.IsEnvironment(DockerEnvironmentName);
    }
}
