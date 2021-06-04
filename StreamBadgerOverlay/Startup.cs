using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamBadger;
using StreamBadger.Endpoints;
using StreamBadger.Shared;
using StreamBadgerOverlay.Data;
using StreamBadgerOverlay.Endpoints;
using StreamBadgerOverlay.Services;

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
            services.AddSingleton<ControlBus>();
            services.AddSingleton<ImageStore>();
            services.AddSingleton<SoundStore>();
            services.AddSingleton<SoundTemp>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHostedService<TwitchBot>();
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
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapGet("/css/bootstrap/bootstrap.min.css",
                    context => ServeCss(context, "bootstrap.bootstrap.min.css"));
                endpoints.MapGet("/css/site.css",
                    context => ServeCss(context, "site.css"));
                endpoints.MapGet("/images/{name}", ImageEndpoint.Get);
                endpoints.MapGet("/sounds/{name}", SoundEndpoint.Get);
                endpoints.MapGet("/show/{image}", ShowEndpoint.Show);
                endpoints.MapGet("/play/{sound}", PlayEndpoint.Play);
                endpoints.MapGet("/temp/sounds/{sound}", TempSounds.Play);
                endpoints.MapGet("/clear", ClearEndpoint.Clear);

                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static async Task ServeCss(HttpContext context, string fileName)
        {
            var resource = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"StreamBadgerOverlay.wwwroot.css.{fileName}");
            if (resource is not null)
            {
                context.Response.StatusCode = 200;
                await resource.CopyToAsync(context.Response.Body);
            }
        }
    }
}