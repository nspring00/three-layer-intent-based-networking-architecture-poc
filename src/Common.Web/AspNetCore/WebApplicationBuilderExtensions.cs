using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Common.Web.AspNetCore;

public static class WebApplicationBuilderExtensions
{
    private const string AppSettingsEnforceHttp2Flag = "ASPNETCORE_Kestrel:Enforce_HTTP2";
    private const string AppSettingsHttp1Port = "ASPNETCORE_Kestrel:HTTP1_Port";
    private const string AppSettingsHttp2Port = "ASPNETCORE_Kestrel:HTTP2_Port";

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

    public static WebApplicationBuilder ConfigurePortsForRestAndGrpcNoTls(this WebApplicationBuilder builder, int? http1Port = null, int? http2Port = null)
    {
        http1Port ??= builder.Configuration[AppSettingsHttp1Port] is not null
            ? int.Parse(builder.Configuration[AppSettingsHttp1Port])
            : 80;        
        http2Port ??= builder.Configuration[AppSettingsHttp2Port] is not null
            ? int.Parse(builder.Configuration[AppSettingsHttp2Port])
            : 8080;
            
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(http1Port.Value, listenOptions =>
                listenOptions.Protocols = HttpProtocols.Http1);
            options.ListenAnyIP(http2Port.Value, listenOptions =>
                listenOptions.Protocols = HttpProtocols.Http2);
        });

        return builder;
    }
}
