﻿using CityAppREST.Data;
using CityAppREST.Data.Repositories;
using CityAppREST.Helpers;
using CityAppREST.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CityAppREST
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("CityAppDB"));

            // Add repository services
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Company>, CompanyRepository>();

            services.AddSingleton<TokenGenerator>();

            // Add data initializer
            services.AddTransient<CityAppDataInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CityAppDataInitializer cityAppDataInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

            cityAppDataInitializer.InitializeData();
        }
    }
}
