using System;
using System.Linq;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using CBAS.Extensions;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using Serilog;
using Serilog.Exceptions;

namespace AngularSPAWebAPI
{
    public class Startup
    {
        private readonly IHostingEnvironment currentEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {

            Configuration = configuration;

            currentEnvironment = env;
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // By setting EnableEndpointRouting to false, you can continue using the traditional MVC routing setup
            services.AddMvc(options => options.EnableEndpointRouting = false);
            //// SQLite & Identity.
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("DEF_CONN")));

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("PubScreenConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                // Lockout settings.
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
            });

            // Role based Authorization: policy based role checks.
            services.AddAuthorization(options =>
            {
                // Policy for dashboard: only administrator role.
                options.AddPolicy("Manage Accounts", policy => policy.RequireRole("administrator"));
                // Policy for resources: user or administrator roles. 
                options.AddPolicy("Access Resources", policy => policy.RequireRole("administrator", "user"));
            });

            // Adds application services.
            services.AddTransient<IEmailSender, EmailSender>();
            //services.Add(ServiceDescriptor.Transient<IElasticClient, EmailSender>());
            services.AddElasticSearch(Configuration);

            //Adds serilog to 
            services.AddSerilog();

            // Uncomment this line for publuishing
            //services.AddIdentityServer(options =>
            //         options.PublicOrigin = "https://mousebytes.ca")
            services.AddIdentityServer()
                // The AddDeveloperSigningCredential extension creates temporary key material for signing tokens.
                // This might be useful to get started, but needs to be replaced by some persistent key material for production scenarios.
                // See http://docs.identityserver.io/en/release/topics/crypto.html#refcrypto for more information.
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                // To configure IdentityServer to use EntityFramework (EF) as the storage mechanism for configuration data (rather than using the in-memory implementations),
                // see https://identityserver4.readthedocs.io/en/release/quickstarts/8_entity_framework.html
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>(); // IdentityServer4.AspNetIdentity.

            if (currentEnvironment.IsProduction())
            {
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5000/";
                        options.RequireHttpsMetadata = false;

                        options.ApiName = "WebAPI";
                    });
            }
            else
            {
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000/";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "WebAPI";
                });
            }


            services.AddCors(options =>
            {
                options.AddPolicy("LocalCorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

                options.AddPolicy("ProductionCorsPolicy", builder =>
                {
                    builder.WithOrigins("https://mousebytes.ca") // Production origins
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            services.Configure<FormOptions>(x => x.ValueCountLimit = 2048);

            services.AddMvc();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSerilogRequestLogging(); // To enable Serilog request logging
            if (env.IsDevelopment())
            {
                app.UseCors("LocalCorsPolicy");
                app.UseDeveloperExceptionPage();
                // Starts "npm start" command using Shell extension.
                app.Shell("ng serve");
            }
            else
            {
                app.UseCors("ProductionCorsPolicy");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
           

            app.UseHttpsRedirection();
            app.UseRouting();


            // Router on the server must match the router on the client (see app.routing.module.ts) to use PathLocationStrategy.
            var appRoutes = new[] {
                 "/home",
                 "/account/signin",
                 "/account/signup",
                 "/account/forgot",
                 "/resources",
                 "/dashboard",
                 "/taskAnalysis",
                 "/upload",
                 "/animal-info",
                 "/experiment",
                 "/data-extraction",
                 "/data-link",
                 "/search-experiment",
                 "/manage-user",
                 "/profile",
                 "/imaging",
                 "/genomics",
                 "/guideline",
                 "/data-visualization",
                 "/video-tutorial",
                 "/forms",
                 "/terms",
                 "/contact-us",
                 "/account/reset",
                 "/download-ds",
                 "/mb-dashboard",
                 "/pubScreen-dashboard",
                 "/pubScreen",
                 "/pubScreen-search",
                 "/comp",
                 "/comp-search",
                 "/pubScreen-queue",
                 "/pubScreen-edit",
                 "/comp-edit",
            };

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && appRoutes.Contains(context.Request.Path.Value))
                {
                    context.Request.Path = new PathString("/");
                }

                await next();
            });

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Microsoft.AspNetCore.StaticFiles: API for starting the application from wwwroot.
            // Uses default files as index.html.
            app.UseDefaultFiles();
            // this should always be the last middleware
            app.UseStaticFiles();


        }
    }
}
