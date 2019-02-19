using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.Models;
using IdentityServer4;
using Trepub.Web.API.Identity;
using Swashbuckle.AspNetCore.Swagger;
using System.ServiceModel;
using SoapCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Trepub.Common.Facade;
using Trepub.IFS;
using Microsoft.AspNetCore.Mvc;

namespace Trepub.Web.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var ifsFacade = new IfsFacade();

            ((IfsFacade)FacadeProvider.IfsFacade).SetConfigurationRoot(Configuration);

            //Authorization's policy requirement
            services.AddScoped<IAuthorizationHandler, RoleResourceRequirementHandler>();

            //CORS
            services.AddCors();

            //Memory Cache 
            services.AddMemoryCache();

            // Angular's default header name for sending the XSRF token.
            //services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            //MVC
            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(TxActionFilter));
                var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).
                                RequireAuthenticatedUser().AddRequirements(new RoleResourceRequirement()).Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new ValidationFilter());
            }).AddJsonOptions(options => options.SerializerSettings
                .ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            //IdentityServer & Authorization
            services.AddIdentityServer(options =>
            {
                //options.RequireSsl = false;
                options.IssuerUri = Configuration.GetSection("LocalSettings")["TokenIssuerUri"]; ;
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients())
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddSecretValidator<Trepub.Web.API.Identity.SecretValidator>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("LocalSettings")["BaseUrl"];
                    options.RequireHttpsMetadata = false;
                    //options.ApiName = "api1";
                });


            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Trepub.api", Version = "v1" });
                c.SchemaFilter<SwaggerSchemaFilter>();
                c.OperationFilter<SwaggerOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            bool testing = false;
            bool.TryParse(Configuration.GetSection("LocalSettings")["TestEnvironment"], out testing);
            if (testing)
            {
                // Enable Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                //c.EnabledValidator();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trepub.api");
                });
            }

            //TODO: move to appsetting
            //loggerFactory.AddFile("../../../TrepubLogs/Trepub-{Date}.txt", LogLevel.Information, null, false, 1073741824, null);

            ((IfsFacade)FacadeProvider.IfsFacade).SetILoggerFactory(app.ApplicationServices.GetService<ILoggerFactory>());

            app.UseCors(cb => cb.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            //IdentityServer
            app.UseIdentityServer();
            app.UseMiddleware<ErrorHandlerMW>();

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.ApplicationServices.GetService<ILoggerFactory>().CreateLogger<Startup>().LogInformation("Trepub Initialized.");
        }

        #region IdentityServer Configuration Methods

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new List<string> { "role" })

            };
        }

        public IEnumerable<Client> GetClients()
        {
            //var partyConfigBSO = BusinessFacade.GetBSO<PartyConfigBSO>();
            return new List<Client>
            {
                new Client
                {
                    ClientId = "roClient",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new IdentityServer4.Models.Secret("trepubSecrect".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        "roles"
                    }
                },

                //Azartel Mobile App Server client
                //TODO: to read the clients from db and make it dynamic.
                new Client
                {
                    //ClientId = partyConfigBSO.GetPartyConfig(EntityConstants.BUSINESSUNITPARTY_IDS_AZARTEL_MOBILEAPPSERVER, 
                    //EntityConstants.PARTY_CONFIGKEY_APPSERVERCLIENT_ID),
                    ClientId = "AzartelMASClient",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new IdentityServer4.Models.Secret("AzartelMASClient12345".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        "roles"
                    }
                }

            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1" ) {
                    UserClaims = { "role" }
                }
            };
        }

        #endregion


        #region Swagger Filter

        public class SwaggerOperationFilter : IOperationFilter
        {

            ILogger<Startup> logger;
            public SwaggerOperationFilter(ILogger<Startup> logger)
            {
                this.logger = logger;
            }

            public void Apply(Operation operation, OperationFilterContext context)
            {
                logger.LogInformation($"------ {operation.OperationId} -----");
            }
        }


        public class SwaggerSchemaFilter : ISchemaFilter
        {

            ILogger<Startup> logger;
            public SwaggerSchemaFilter(ILogger<Startup> logger)
            {
                this.logger = logger;
            }

            public void Apply(Schema model, SchemaFilterContext context)
            {
                var typeInfo = context.SystemType.GetTypeInfo();

                if (typeInfo.Namespace.StartsWith("Trepub"))
                {
                    if (!typeInfo.Name.EndsWith("View"))
                    {
                        logger.LogWarning("SWAGGWR ERROR >>>>>>>>>>>> Invalid type in API: " + typeInfo.FullName);
                    }
                    else
                    {
                        logger.LogInformation("SWAGGER SCHEMA - " + typeInfo.Name);

                    }
                }

            }
        }

        #endregion 

    }
}
