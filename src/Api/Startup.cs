using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {

            services.AddControllers ();

            services.AddAuthentication ("Bearer")
                .AddJwtBearer ("Bearer", options => {
                    options.Authority = "https://localhost:5001";

                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization (options => {
                options.AddPolicy ("ApiScope", policy => {
                    policy.RequireAuthenticatedUser ();
                    policy.RequireClaim ("scope", "api1.read");
                });
            });

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter> ();
                c.AddSecurityDefinition ("oauth2", new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows {
                            Password = new OpenApiOAuthFlow {
                                TokenUrl = new Uri ("https://localhost:5001/connect/token", UriKind.Absolute),
                                    Scopes = new Dictionary<string, string> { { "api1.read", "api1.read" } }
                            }
                        }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
                app.UseSwagger ();
                app.UseSwaggerUI (c => {
                    c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Api v1");
                    c.OAuthClientId ("mvc2");
                    c.OAuthClientSecret ("secret");
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant ();
                });
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ().RequireAuthorization ("ApiScope");
            });
        }
    }
}