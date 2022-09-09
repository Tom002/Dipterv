using System.Collections.Generic;
using System.Linq;
using Dipterv.Bll.Services;
using Dipterv.Dal.DbContext;
using Dipterv.Shared.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Templates.TodoApp.Host;

var host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(cfg => {
        // Looks like there is no better way to set _default_ URL
        cfg.Sources.Insert(0, new MemoryConfigurationSource() {
            InitialData = new Dictionary<string, string>() {
                {WebHostDefaults.ServerUrlsKey, "http://localhost:5000"},
            }
        });
    })
    .ConfigureWebHostDefaults(webHost => webHost
        .UseDefaultServiceProvider((ctx, options) => {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            }
        })
        .UseStartup<Startup>())
    .Build();

//var appPartManager = (ApplicationPartManager)host.Services.GetService(typeof(ApplicationPartManager));

//var controllerFeature = new ControllerFeature();
//appPartManager.PopulateFeature(controllerFeature);

//// Get the names of all of the controllers
//var controllers = controllerFeature.Controllers.Select(x => x.Name);

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FusionDbContext>();

    context.Database.EnsureCreated();
}



await host.RunAsync();
