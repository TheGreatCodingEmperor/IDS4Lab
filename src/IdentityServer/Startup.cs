﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace IdentityServer {
    public class Startup {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup (IWebHostEnvironment environment, IConfiguration configuration) {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices (IServiceCollection services) {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            string migrationsAssembly = typeof (Startup).GetTypeInfo ().Assembly.GetName ().Name; //UltraApp

            // uncomment, if you want to add an MVC-based UI
            //services.AddControllersWithViews();

            var builder = services.AddIdentityServer ()
                .AddTestUsers (TestUsers.Users)
                .AddConfigurationStore (options => {
                    options.ConfigureDbContext = b => b.UseSqlite (connectionString,
                        sql => sql.MigrationsAssembly (migrationsAssembly));
                })
                .AddOperationalStore (options => {
                    options.ConfigureDbContext = b => b.UseSqlite (connectionString,
                        sql => sql.MigrationsAssembly (migrationsAssembly));
                });

            // var builder = services.AddIdentityServer(options =>
            // {
            //     // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            //     options.EmitStaticAudienceClaim = true;
            // })
            //     .AddInMemoryIdentityResources(Config.IdentityResources)
            //     .AddInMemoryApiScopes(Config.ApiScopes)
            //     .AddInMemoryClients(Config.Clients);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential ();
        }

        public void Configure (IApplicationBuilder app) {
            InitializeDatabase (app);
            if (Environment.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            // uncomment if you want to add MVC
            //app.UseStaticFiles();
            //app.UseRouting();

            app.UseIdentityServer ();

            // uncomment, if you want to add MVC
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});
        }

        private void InitializeDatabase (IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory> ().CreateScope ()) {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext> ().Database.Migrate ();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext> ();
                context.Database.Migrate ();
                if (!context.Clients.Any ()) {
                    foreach (var client in Config.Clients) {
                        context.Clients.Add (client.ToEntity ());
                    }
                    context.SaveChanges ();
                }

                if (!context.IdentityResources.Any ()) {
                    foreach (var resource in Config.IdentityResources) {
                        context.IdentityResources.Add (resource.ToEntity ());
                    }
                    context.SaveChanges ();
                }

                if (!context.ApiScopes.Any ()) {
                    foreach (var resource in Config.ApiScopes) {
                        context.ApiScopes.Add (resource.ToEntity ());
                    }
                    context.SaveChanges ();
                }
            }
        }
    }
}