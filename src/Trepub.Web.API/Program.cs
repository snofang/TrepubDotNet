using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;


namespace Trepub.Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args);
            host.ConfigureLogging((webHostBuilderContext, loggingBuilder) =>{
                loggingBuilder.ClearProviders();
                loggingBuilder.AddFile(webHostBuilderContext.Configuration.GetSection("Logging"));
            });
            host.UseStartup<Startup>();
            host.Build().Run();
            //AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Mvc.EnableRangeProcessing", tr//ue);

        }

    }
}
