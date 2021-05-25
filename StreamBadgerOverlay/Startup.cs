using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamBadger.Endpoints;
using StreamBadger.Shared;
using StreamBadgerOverlay.Data;

namespace StreamBadgerOverlay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ImageStore>();
            services.AddSingleton<SoundStore>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            Stopdown.Lifetime = lifetime;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapGet("/css/bootstrap/bootstrap.min.css", async context =>
                {
                    var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("StreamBadgerOverlay.wwwroot.css.bootstrap.bootstrap.min.css");
                    if (resource is not null)
                    {
                        context.Response.StatusCode = 200;
                        await resource.CopyToAsync(context.Response.Body);
                    }
                });
                endpoints.MapGet("/images/{name}", ImageEndpoint.Get);
                endpoints.MapGet("/sounds/{name}", SoundEndpoint.Get);

                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
