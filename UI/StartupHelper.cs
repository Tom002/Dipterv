using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stl.Fusion.Client;
using Stl.Fusion.Blazor;
using Stl.Fusion.Extensions;
using Stl.Fusion.UI;
using Stl.Fusion;
using System;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.Clients;
using Dipterv.Shared.Interfaces.ComputeServices;
using Templates.TodoApp.UI.Mapping;

namespace Templates.TodoApp.UI;

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
        fusionClient.AddReplicaService<IShoppingCartService, IShoppingCartClientDef>();
        fusionClient.AddReplicaService<IShoppingCartDetailsService, IShoppingCartDetailsClientDef>();
        fusionClient.AddReplicaService<IProductDetailsService, IProductDetailsClientDef>();
        fusionClient.AddReplicaService<IProductSearchService, IProductSearchClientDef>();

        services.AddAutoMapper(typeof(MappingProfile));

        ConfigureSharedServices(services);
    }

    public static void ConfigureSharedServices(IServiceCollection services)
    {
        // Blazorise
        services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

        // Other UI-related services
        var fusion = services.AddFusion();
        fusion.AddFusionTime();
        fusion.AddBackendStatus();

        // Default update delay is 0.5s
        services.AddTransient<IUpdateDelayer>(c => new UpdateDelayer(c.UICommandTracker(), 0.5));
    }
}
