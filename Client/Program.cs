using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Dipterv.Client;
using Stl.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

StartupHelper.ConfigureServices(builder.Services, builder);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var host = builder.Build();
host.Services.HostedServices().Start();
await host.RunAsync();
