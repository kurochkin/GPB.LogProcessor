using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogProcessor
{
    public class Startup
    {
        private readonly string _apiKey;

        public Startup()
        {
            _apiKey = "4ED6DB8C6CA444AEA9296E73174C20F2";
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer(new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                StaticFileOptions = { ServeUnknownFileTypes = true }
            });

            app.Run(async (context) =>
            {
                if (context.Request.Path.ToString().TrimEnd('/') == "/q")
                {
                    if (!context.Request.Query.TryGetValue("apikey", out var apikey))
                    {
                        throw new UnauthorizedAccessException("No apikey");
                    }

                    var apikey0 = apikey.First();
                    if (apikey0 != _apiKey)
                    {
                        throw new UnauthorizedAccessException("Invalid apikey");
                    }

                    if (!context.Request.Query.TryGetValue("requestId", out var requestId))
                    {
                        throw new ArgumentException("No requestId");
                    }

                    var requestId0 = requestId.First();
                    if (string.IsNullOrWhiteSpace(requestId0))
                    {
                        throw new ArgumentException("Empty requestId");
                    }

                    if (!context.Request.Query.TryGetValue("component", out var component))
                    {
                        throw new ArgumentException("No component");
                    }

                    var component0 = component.First();
                    if (string.IsNullOrWhiteSpace(component0))
                    {
                        throw new ArgumentException("Empty component");
                    }

                    string cmdlet = Path.Combine(env.ContentRootPath, "GetLogByRequestId.ps1");
                    string outFileName = requestId0 + ".log";
                    string outFileNameFull = Path.Combine(env.WebRootPath, outFileName);
                    string cmdArgs = $"-File \"{cmdlet}\" -Text \"{requestId0}\" -Component \"{component0}\" -OutFile \"{outFileNameFull}\"";

                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync($"<b>Run args:</b>{cmdArgs} ....<br/>");
                    await context.Response.Body.FlushAsync();

                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "powershell",
                        Arguments = cmdArgs,
                        CreateNoWindow = true,
                        WorkingDirectory = env.WebRootPath
                    }).WaitForExit((int)(TimeSpan.FromMinutes(5).TotalMilliseconds));

                    await context.Response.WriteAsync($"<br/><b>Result:</b><a href='../{outFileName}'>{outFileName}</a>");
                    await context.Response.WriteAsync("</body></html>");
                }
            });
        }
    }
}
