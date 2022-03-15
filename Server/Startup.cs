using Stl.Fusion.Bridge;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Dipterv.Bll.Mappings;
using Dipterv.Bll.Services;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
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

namespace Dipterv.Server
{
    public class Startup
    {
        private IConfiguration Cfg { get; }
        private IWebHostEnvironment Env { get; }

        public Startup(IConfiguration cfg, IWebHostEnvironment environment)
        {
            Cfg = cfg;
            Env = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Publisher.Options() { Id = "P-zRElT9p6urTsqJe7" });

            services.AddRouting();

            var fusion = services.AddFusion();
            var fusionServer = fusion.AddWebServer();
            var fusionAuth = fusion.AddAuthentication()
                .AddServer(
                        signInControllerSettingsFactory: _ => SignInController.DefaultSettings with
                        {
                            DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                            SignInPropertiesBuilder = (_, properties) => {
                                properties.IsPersistent = true;
                            }
                        },
                        serverAuthHelperSettingsFactory: _ => ServerAuthHelper.DefaultSettings with
                        {
                            NameClaimKeys = Array.Empty<string>(),
                        })
                .AddAuthBackend<DbAuthService<FusionDbContext>>()
                .AddServer();

            fusion.AddComputeService<IProductService, ProductService>();
            fusion.AddComputeService<IProductReviewService, ProductReviewService>();
            fusion.AddComputeService<IProductInventoryService, ProductInventoryService>();
            fusion.AddComputeService<IDateService, DateService>();
            fusion.AddComputeService<ISpecialOfferService, SpecialOfferService>();
            fusion.AddComputeService<ICustomerService, CustomerService>();
            fusion.AddComputeService<IOrderService, OrderService>();

            services.AddScoped<IAccountService, AccountService>();

            services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<FusionDbContext>()
            .AddDefaultTokenProviders();

            services.AddDbContextFactory<FusionDbContext>(dbContext => {
                dbContext.EnableSensitiveDataLogging();
            });

            services.AddScoped<FusionDbContext>(p => p.GetRequiredService<IDbContextFactory<FusionDbContext>>().CreateDbContext());

            services.AddDbContextServices<FusionDbContext>(dbContext =>
            {
                dbContext.AddEntityResolver<int, Product>();
                dbContext.AddEntityResolver<int, ProductInventory>();
                dbContext.AddEntityResolver<int, ProductReview>();
                dbContext.AddEntityResolver<int, Location>();
                dbContext.AddEntityResolver<int, WorkOrder>();
                dbContext.AddEntityResolver<int, SpecialOffer>();

                dbContext.AddOperations((_, o) => {
                    o.UnconditionalWakeUpPeriod = TimeSpan.FromSeconds(1);
                });

                dbContext.AddAuthentication<DbSessionInfo<long>, DbUser<long>, long>();
            });

            services.AddAutoMapper(typeof(MappingProfile));

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fusion API", Version = "v1" });
            //    c.CustomSchemaIds(x => x.FullName);
            //});

            services.AddTransient<DateInvalidateService>();

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Data Source=localhost,3033;Initial Catalog=AdventureWorks2019;Integrated Security=False;User Id=sa;Password=Password123!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.FromHours(12),
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true, // Migration to Schema 7 is required
            PrepareSchemaIfNecessary = true
                    }));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
            });


            // ASP.NET Core authentication providers
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                //options.LoginPath = "/signIn";
                //options.LogoutPath = "/signOut";
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;

                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            services.AddHttpContextAccessor();

            services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            // Web
            services.AddRouting();
            services.AddMvc().AddApplicationPart(Assembly.GetExecutingAssembly());
            services.AddServerSideBlazor(o => o.DetailedErrors = true);
            fusionAuth.AddBlazor(o => { });
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> log)
        {
            // Configure the HTTP request pipeline.
            if (Env.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30),
            });
            app.UseFusionSession();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFusionWebSocketServer();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
