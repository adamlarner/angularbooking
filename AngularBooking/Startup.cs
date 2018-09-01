using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngularBooking.Data;
using AngularBooking.Models;
using AngularBooking.Services.Email;
using AngularBooking.Services.JwtManager;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AngularBooking
{
    public class Startup
    {
        private SqliteConnection _connection;

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            if (HostingEnvironment.IsEnvironment("Development_WebAPI_Test"))
            {
                _connection = new SqliteConnection("DataSource=:memory:;cache=shared");
                _connection.Open();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // retrieve smtp config from environment variable (needed for configuring both identity email confirm and email service)
            string smtpConfig = Configuration.GetValue<string>("BOOKING_SMTP");
            // determine whether email confirmation should be configured (enable for testing)
            bool emailConfirmation = string.IsNullOrEmpty(smtpConfig) ? HostingEnvironment.IsEnvironment("Development_WebAPI_Test") ? true : false : true;

            // configure data protection rules explicitly (linux hosts require this for the persistence of keys)
            // note: for production environments, set up a protector to interface with an external HSM/Key management system
            services.AddDataProtection()
                .SetApplicationName("angularbooking")
                .PersistKeysToFileSystem(new DirectoryInfo("/keys"));

            // configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                    builder.
                    AllowAnyHeader().
                    AllowAnyOrigin().
                    AllowAnyMethod().
                    AllowCredentials()
                );
            });

            // configure CSRF
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            // configure MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddSpaStaticFiles(conf =>
            {
                conf.RootPath = "ClientApp/dist";
            });

            // configure global CORS policy
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("Default"));
            });

            // add EF
            services.AddEntityFrameworkSqlite()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    if (HostingEnvironment.IsEnvironment("Development_WebAPI_Test"))
                    {
                        // configure in-memory WebAPI test environment
                        options.UseSqlite(_connection);
                    }
                    else
                    {
                        options.UseSqlite("DataSource=local.db");
                    }
                });

            // add Identity
            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = emailConfirmation;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // add JWT auth
            // clear default claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // specify jwt key
            string jwtKey = Configuration["JWT_KEY"];
            // configure default authentication scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JWT_ISSUER"],
                    ValidAudience = Configuration["JWT_AUDIENCE"],
                    // todo - use injected key from docker/kubernetes secret manager
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero,

                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = (content) =>
                    {
                        // get service for jwt manager
                        IJwtManager jwtManager = content.HttpContext.RequestServices.GetService<IJwtManager>();
                        JwtSecurityToken token = (JwtSecurityToken)content.SecurityToken;

                        // check against revocation list, and fail auth if on list
                        if (jwtManager.IsRevoked(token))
                        {
                            content.Fail("Refused: token has been revoked");
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddOData();

            // add service for sending basic emails (BasicEmailService for local testing only, NOT for production use)
            string emailPickupDirectory = HostingEnvironment.IsEnvironment("Development_WebAPI_Test") ?
                Path.Combine(Environment.CurrentDirectory, "test_email") : null;
            services.AddTransient<IEmailService, BasicEmailService>(f => new BasicEmailService(emailPickupDirectory, smtpConfig));

            // add service for accessing application data layer
            services.AddScoped<IUnitOfWork, DbUnitOfWork>();

            // service for managing JWT creation and revocation
            services.AddSingleton<IJwtManager, InMemoryJwtManager>(f => new InMemoryJwtManager(Configuration, jwtKey));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // register shutdown clean-up in application lifetime service
            lifetime.ApplicationStopped.Register(() =>
            {
                // clean up sqlite in-memory connection
                if (_connection != null)
                    _connection.Dispose();
            });

            // create migration for database, and create if it doesn't already exist
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    // if 'demo' mode, seed data after db deletion/creation
                    if (Configuration.GetValue<string>("BOOKING_DEMO") != "false" && !env.IsEnvironment("Development_WebAPI_Test"))
                    {
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();

                        // seed data
                        var seedSection = Configuration.GetSection("BOOKING_DEMO_SEED");

                        SeedDataProvider.Generate
                        (
                            context,
                            seedSection.GetValue<int>("DAYS"),
                            seedSection.GetValue<int>("ROOMS"),
                            seedSection.GetValue<int>("MIN_ROOM_COLUMNS"),
                            seedSection.GetValue<int>("MAX_ROOM_COLUMNS"),
                            seedSection.GetValue<int>("MIN_ROOM_ROWS"),
                            seedSection.GetValue<int>("MAX_ROOM_ROWS")
                        );

                        string username = Configuration.GetValue<string>("BOOKING_DEMO_USER");
                        string password = Configuration.GetValue<string>("BOOKING_DEMO_PASSWORD");

                        // if demo user and password are specified, create admin user
                        if (username != null && password != null)
                        {
                            // get required services
                            using (UserManager<User> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>())
                            {
                                using (RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>())
                                {
                                    User adminUser = new User
                                    {
                                        UserName = username,
                                        Email = password,
                                        EmailConfirmed = true
                                    };

                                    var createResult = userManager.CreateAsync(adminUser, password).Result;
                                    if (createResult.Succeeded)
                                    {
                                        context.Customers.Add(new Customer
                                        {
                                            UserId = adminUser.Id,
                                            ContactEmail = username,
                                            FirstName = "DEMO",
                                            LastName = "DEMO"
                                        });
                                        context.SaveChanges();
                                        roleManager.CreateAsync(new IdentityRole { Name = "user" });
                                        roleManager.CreateAsync(new IdentityRole { Name = "admin" });
                                        userManager.AddToRoleAsync(adminUser, "user");
                                        userManager.AddToRoleAsync(adminUser, "admin");
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        context.Database.Migrate();
                    }
                }
            }

            /*
             * Configure request pipeline
             */

            // inline 'middleware' for extracting jwt from auth cookie and adding to context header
            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;

                if (path != null && path.Contains("/api"))
                {
                    // read authorization cookie, and add token to header
                    var authCookie = context.Request.Cookies["Authorization"];
                    if (authCookie != null)
                    {
                        context.Request.Headers.Append("Authorization", authCookie);
                    }
                }

                await next();
            });

            app.UseCors();

            app.UseAuthentication();

            app.UseMvc(config =>
            {
                config.MapRoute("default", "api/{controller}/{id}");
                // required for OData filter support
                config.EnableDependencyInjection();
                config.Expand().Filter().OrderBy().Select().MaxTop(null);
            });

            // exclude SPA configuration for test/debug purposes
            if (!env.IsEnvironment("Development_WebAPI_Test"))
            {
                if (env.IsProduction())
                {
                    app.UseSpaStaticFiles();
                }

                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.Options.StartupTimeout = TimeSpan.FromMinutes(2);
                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });

            }

        }

    }
}
