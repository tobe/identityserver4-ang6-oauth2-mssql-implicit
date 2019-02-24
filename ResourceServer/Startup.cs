using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ResourceServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
            /*.AddOAuth2Introspection(options =>
            {
                options.Authority = Configuration.GetSection("AuthService")["Authority"];
                options.ClientId = Configuration.GetSection("AuthService")["ClientId"];
                options.ClientSecret = Configuration.GetSection("AuthService")["ClientSecret"];
            });*/
            .AddJwtBearer(options => {
                // base-address of your identityserver
                options.Authority = Configuration.GetSection("AuthService")["Authority"];

                // name of the API resource
                options.Audience = "api1";

                options.RequireHttpsMetadata = false;

                // Mapira iz JWT-a tj ovog endpointa^ gori (valjda?)
                // ime korisnika iz JWT-a u identity i za rolu
                // Dakle, u JWT-u ima claim koji se zove "role" i to se mapira u ASP.NET Identity Rolu
                // Pa se onda moze koristit [Authorize("Administrator")] i tako te pizdarije
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
            /*.AddOpenIdConnect("oidc", options => {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.ClientId = Configuration.GetSection("AuthService")["ClientId"];
                options.ClientSecret = Configuration.GetSection("AuthService")["ClientSecret"];

                options.GetClaimsFromUserInfoEndpoint = true;
                //options.SaveTokens = true;
                options.ResponseType = "id_token"; // Zanima me samo IDENTITY TOKEN

                // Mapira iz JWT-a tj ovog endpointa^ gori (valjda?)
                // ime korisnika iz JWT-a u identity i za rolu
                // Dakle, u JWT-u ima claim koji se zove "role" i to se mapira u ASP.NET Identity Rolu
                // Pa se onda moze koristit [Authorize("Administrator")] i tako te pizdarije
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });*/

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // CLAIMS yooo
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MoraMocOboje", policyAdmin =>
                {
                    policyAdmin.RequireClaim("CanRead");
                    policyAdmin.RequireClaim("CanWrite");
                });
                options.AddPolicy("MozeBrisat", policyAdmin => {
                    policyAdmin.RequireClaim("CanDelete");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.WithOrigins(Configuration.GetValue<string>("WebApp"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
