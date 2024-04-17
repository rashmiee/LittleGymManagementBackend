using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Extensions.DependencyInjection;

namespace LittleGymManagementBackend
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                builder => builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
                    });
            services.AddControllers();
        }
        // This method gets called by the runtime.Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // In production, use custom error handling.
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Configure middleware
            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseRouting();
            app.UseAuthorization();

            // Enable CORS for requests from http://localhost:3000
            app.UseCors("AllowLocalhost3000");
            //app.UseCors(options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader() );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
