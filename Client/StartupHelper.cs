using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Dipterv.Client.Interfaces;
using Dipterv.Client.Services;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.Clients;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Fusion.Client;
using Stl.Fusion.UI;

namespace Dipterv.Client
{
    public static class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, WebAssemblyHostBuilder builder)
        {
            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");

            // Fusion
            var fusion = services.AddFusion();
            var fusionClient = fusion.AddRestEaseClient(
                (c, o) =>
                {
                    o.BaseUri = baseUri;
                    o.IsLoggingEnabled = true;
                    o.IsMessageLoggingEnabled = false;
                }).ConfigureHttpClientFactory(
                (c, name, o) =>
                {
                    var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
                    var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
                    o.HttpClientActions.Add(client => client.BaseAddress = clientBaseUri);
                });

            fusion.AddAuthentication().AddRestEaseClient().AddBlazor();

            fusionClient.AddReplicaService<IProductService, IProductClientDef>();
            fusionClient.AddReplicaService<IProductReviewService, IProductReviewClientDef>();
            fusionClient.AddReplicaService<IProductInventoryService, IProductInventoryClientDef>();
            fusionClient.AddReplicaService<ISpecialOfferService, ISpecialOfferClientDef>();
            fusionClient.AddReplicaService<IOrderService, IOrderClientDef>();
            fusionClient.AddReplicaService<IAccountService, IAccountClientDef>();

            fusion.AddComputeService<IProductDetailsService, ProductDetailsService>();

            services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();
            services.AddTransient<IUpdateDelayer>(c => new UpdateDelayer(c.UICommandTracker(), 0.1));
        }
    }
}
