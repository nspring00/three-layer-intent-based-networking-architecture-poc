using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Common.Web.AspNetCore;

public static class WebApplicationBuilderExtensions
{
    private const string AppSettingsEnforceHttp2Flag = "ASPNETCORE_Kestrel:Enforce_HTTP2";

    public static WebApplicationBuilder CheckEnforceHttp2(this WebApplicationBuilder builder)
    {
        Console.WriteLine($"ASPNETCORE_Kestrel:Enforce_HTTP2={builder.Configuration[AppSettingsEnforceHttp2Flag]}");
        if (builder.Configuration[AppSettingsEnforceHttp2Flag] is not null &&
            builder.Configuration[AppSettingsEnforceHttp2Flag].Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
            });
        }

        return builder;
    }
}
