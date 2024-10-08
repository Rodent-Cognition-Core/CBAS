using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                        //.UseHttpSys(options => { options.MaxRequestBodySize = 100_000_000;})
                        //.UseUrls("http://localhost:5000")
                        .Build();
    }
}
