using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AngularSPAWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                        //.UseStartup<Startup>()
                        //.UseUrls("http://*:80;") // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x
                        //.Build();
                        .UseStartup<Startup>()
                        .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); o.Limits.MaxRequestBodySize = null; })
                        .UseIIS()
                        .ConfigureKestrel((context, serverOptions) =>
                        {
                            var kestrelConfig = context.Configuration.GetSection("Kestrel");
                            var certPath = kestrelConfig["CertificatePath"] ?? Environment.GetEnvironmentVariable("KESTREL_CERT_PATH");
                            var keyPath = kestrelConfig["KeyPath"] ?? Environment.GetEnvironmentVariable("KESTREL_KEY_PATH");
                            var portValue = kestrelConfig["Port"] ?? Environment.GetEnvironmentVariable("KESTREL_PORT");

                            if (!string.IsNullOrEmpty(portValue) && int.TryParse(portValue, out int port))
                            {
                                serverOptions.Listen(IPAddress.Any, port, listenOptions =>
                                {
                                    if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath) &&
                                        File.Exists(certPath) && File.Exists(keyPath))
                                    {
                                        var cert = X509Certificate2.CreateFromPemFile(certPath, keyPath);
                                        cert = new X509Certificate2(cert.Export(X509ContentType.Pfx));
                                        listenOptions.UseHttps(cert);
                                    }
                                });
                            }
                        })
                        //.UseHttpSys(options => { options.MaxRequestBodySize = 100_000_000;})
                        //.UseUrls("api_url")
                        .Build();
    }
}
