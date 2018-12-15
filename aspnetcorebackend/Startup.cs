using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Models;
using aspnetcorebackend.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using Serilog;

namespace aspnetcorebackend {
    public class Startup {
        public Startup (IConfiguration configuration) {
            // Logger
            Log.Logger = new LoggerConfiguration ()
                .ReadFrom
                .Configuration (configuration)
                .CreateLogger ();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            var appSettingsSection = Configuration.GetSection ("AppSettings");
            services.Configure<AppSettings> (appSettingsSection);

            services.AddCors ();

            var key = Encoding.ASCII.GetBytes (Configuration.GetSection ("AppSettings:Secret").Value);

            // IN-MEMORY PROVIDER
            // TODO: Comment out the line below and the GetRequiredService inside Configure() to swap with a real database in production
            services.AddDbContext<ApplicationDbContext> (option => option.UseInMemoryDatabase ("TestData"));

            // Real database SQL Server
            // TODO: Uncomment the line below to switch on the real database
            // services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("")));

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "http://localhost:5000",
                    ValidAudience = "http://localhost:5000",
                    IssuerSigningKey = new SymmetricSecurityKey (key)
                    };
                });

            services.AddAutoMapper ();
            services.AddScoped<IUserRepository, UserRepository> ();
            services.AddScoped<IDepartmentRepository, DepartmentRepository> ();
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {

            // Serilog
            loggerFactory.AddSerilog ();

            //NSwag
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, swaggerSettings =>
            {
                swaggerSettings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("Authentication"));

                swaggerSettings.GeneratorSettings.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("Authentication", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "The language of the response",
                        In = SwaggerSecurityApiKeyLocation.Header
                    }));
                
                swaggerSettings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Office App API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new SwaggerContact
                    {
                        Name = "Devlin Duldulao",
                        Email = string.Empty,
                        Url = "https://devlinduldulao.pro"
                    };
                    document.Info.License = new SwaggerLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://devlinduldulao.pro/license"
                    };
                };
            });

            
            app.UseCors (b => b.AllowAnyHeader ().AllowCredentials ().AllowAnyMethod ().AllowAnyOrigin ());
            app.UseAuthentication ();

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
                // In-Memory seeding of data
                // TODO: Comment out the two lines below when switching to real database
                var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext> ();
                TestData.AddTestData (context);
            } else {
                app.UseExceptionHandler ("/Error");
                app.UseHsts ();
            }
            app.UseHttpsRedirection ();
            app.UseHealthChecks("/ready",
                new HealthCheckOptions {
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonConvert.SerializeObject(
                            new {
                                status = report.Status.ToString(),
                                errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
            app.UseMvc ();
        }
    }
}

public static class TestData {
    public static void AddTestData (ApplicationDbContext context) {
        context.SaveChanges ();
    }
}