using Diplom2.Data.Analyzer;
using Diplom2.Data.DBO;
using Diplom2.Data.Interfaces;
using Diplom2.Data.Interfaces.S3;
using Diplom2.Data.S3;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Diplom2
{
    public class Program
    {
        private static WebApplicationBuilder builder;
        private static WebApplication app;
        public static void Main(string[] args)
        {
            builder = WebApplication.CreateBuilder(args);
            var cors = new Dictionary<string, List<string>>
            {
                { "frontend-cors", ["http://my-site.com:3000"] }
            };
            EnableCors(cors);
            BasicDependencies();
            SetAuthentication_Keycloak();
            SetDB_Mysql();
            SetDB_Qdrant();
            SetS3_GCPObjectStorage();
            Set_Qdrant();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthorization();
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.None;
            });
            app = builder.Build();
            var forwardingOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardingOptions.KnownNetworks.Clear();  // удалим loopback по умолчанию
            forwardingOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardingOptions);
            
            if (app.Environment.IsDevelopment())
            {
                //IdentityModelEventSource.ShowPII = true;
                app.MapOpenApi();
            }

            SetLogout_Keycloak();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(cors.Keys.First());

            app.MapControllers();

            app.Run();
        }

        private static void BasicDependencies()
        {
            builder.Services.AddSingleton<IRecomender, Recomender>();
            builder.Services.AddScoped<ICart, CartRepository>();
            builder.Services.AddScoped<ICategory, CategoryRepository>();
            builder.Services.AddScoped<ILocalGeoService>(p => new LocalGeoService("/app/GeoLite2-Country.mmdb"));
            builder.Services.AddScoped<IProduct, ProductRepository>();
            builder.Services.AddScoped<IUser, UserRepository>();
            builder.Services.AddScoped<IFilter, FilterRepository>();
            builder.Services.AddScoped<INiceness, NicenessRepository>();
        }

        private static void EnableCors(Dictionary<string, List<string>> cors) {
            foreach (var item in cors)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(name: item.Key,
                        policy =>
                            {
                                policy.WithOrigins(item.Value.ToArray());

                        });
                });
            }
        }

        private static void SetAuthentication_Keycloak()
        {
            var kc = builder.Configuration.GetSection("Keycloak");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOpts =>
            {
                cookieOpts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = kc["Authority"];
                options.ClientId = kc["ClientId"];
                options.ClientSecret = kc["ClientSecret"];
                options.ResponseType = kc["ResponseType"]; // "code"
                options.SaveTokens = bool.Parse(kc["SaveTokens"]);
                options.Scope.Clear();
                foreach (var scope in kc["Scopes"].Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    options.Scope.Add(scope);
                }
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = true; // production = true
                options.CallbackPath = "/api/signin-oidc";
                options.SignedOutCallbackPath = "/api/signout-callback-oidc";
            });
        }

        private static void SetDB_Mysql()
        {
            var connectionString = builder.Configuration.GetConnectionString("DBConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(
                    connectionString
                )
            );
        }

        private static void SetS3_GCPObjectStorage()
        {
            builder.Services.AddSingleton(StorageClient.Create());
            builder.Services.AddScoped<IS3Images, GcsService>();
        }

        private static void SetLogout_Keycloak()
        {
            app.MapGet("/api/user/login", (HttpContext ctx) =>
                Results.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    new[] { OpenIdConnectDefaults.AuthenticationScheme }
                )
            );

            app.MapGet("/api/user/logout", async ctx =>
            {
                await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
                    new AuthenticationProperties { RedirectUri = "/" });
            });
        }
        private static async Task SetDB_Qdrant() {
        }
        private static void Set_Qdrant() {
            
        }
    }
}
