using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;

namespace VeloPortal.WebApi.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            string HostName = Dns.GetHostName();
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "VeloPortal API v1",
                    Description = "API documentation for VeloPortal version 1, Recommended Base URL is: " + HostName + "/api/V1/",
                    Contact = new OpenApiContact
                    {
                        Name = "iTech Velocity",
                        Email = "info@itechvelocity.com"
                    }
                });
                // for differecnt document
                //c.SwaggerDoc("Accounts-and-Finance-v1", new OpenApiInfo
                //{
                //    Version = "v1",
                //    Title = "Accounts & Finance API v1",
                //    Description = "API documentation for Accounts & Finanace Velocity version 1, Recommended Base URL is: " + HostName + "/api/V1/",
                //    Contact = new OpenApiContact
                //    {
                //        Name = "iTech Velocity",
                //        Email = "info@itechvelocity.com"
                //    }
                //});
                // start for version 2
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "VeloPortal API v2",
                    Description = "API documentation for VeloPortal version 2",
                    Contact = new OpenApiContact
                    {
                        Name = "iTech Velocity",
                        Email = "info@itechvelocity.com"

                    }
                });
                c.ResolveConflictingActions(apiDescriptions =>
                {
                    // Your conflict resolution strategy here
                    // Example: Choose the first action
                    return apiDescriptions.First();
                });

                //Add JWT Authentication support in Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });

                // Enable annotations
                c.EnableAnnotations();

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            #region Api Version 
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-API-Version"),
                    new MediaTypeApiVersionReader("api-version")
                );

            }).AddMvc(options => {
                options.Conventions.Add(new VersionByNamespaceConvention());
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            #endregion
            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment env,
            IConfiguration configuration)
        {
            // Read from config instead of hardcoding per environment
            // var pathBase = env.IsProduction() ? "/clarraapi" : string.Empty;
            var pathBase = env.IsProduction() ? configuration["Swagger:PathBase"] ?? string.Empty : string.Empty;
            app.UseSwagger(c =>
            {
                // Dynamically set the swagger.json route prefix
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = $"{pathBase}" }
            };
                });
            });

            app.UseSwaggerUI(c =>
            {
                // Single dynamic endpoint — no more copy-paste per environment
                c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "Velocity API v1");

                // Uncomment when you add more versions/modules:
                // c.SwaggerEndpoint($"{pathBase}/swagger/Accounts-Finance-v1/swagger.json", "Accounts & Finance API v1");
                // c.SwaggerEndpoint($"{pathBase}/swagger/v2/swagger.json", "Main API v2");

                c.DocumentTitle = "VeloPortal  API Documentation";
                c.DisplayRequestDuration();
                c.EnableFilter();
                c.EnableDeepLinking();
                c.DefaultModelsExpandDepth(-1);
                c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
            });

            return app;


           
        }
    }
}
