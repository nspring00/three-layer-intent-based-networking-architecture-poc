using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Common.Web.AspNetCore;

public static class WebApplicationBuilderExtensions
{
    private const string AppSettingsEnforceHttp2Flag = "ASPNETCORE_Kestrel:Enforce_HTTP2";

    public static WebApplicationBuilder CheckEnforceHttp2(this WebApplicationBuilder builder)
    {
        if (builder.Configuration[AppSettingsEnforceHttp2Flag] is not null &&
            builder.Configuration[AppSettingsEnforceHttp2Flag].Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Enforcing HTTP2 on all endpoints");
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
            });
        }

        return builder;
    }

    public static WebApplicationBuilder ConfigurePortsForRestAndGrpcNoTls(this WebApplicationBuilder builder, int http1Port = 80, int http2Port = 8080)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(http1Port, listenOptions =>
                listenOptions.Protocols = HttpProtocols.Http1);
            options.ListenAnyIP(http2Port, listenOptions =>
                listenOptions.Protocols = HttpProtocols.Http2);
        });

        return builder;
    }
}
