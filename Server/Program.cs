using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Dipterv.Bll.Mappings;
using Dipterv.Bll.Services;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Server;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Fusion.Bridge;
using Stl.Fusion.EntityFramework;
using Stl.Fusion.EntityFramework.Authentication;
using Stl.Fusion.Server;
using Stl.Fusion.Server.Authentication;
using Stl.Fusion.Server.Controllers;
using System.Reflection;
using System.Text.RegularExpressions;

var host = Host.CreateDefaultBuilder()
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

await host.RunAsync();