using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using StreamBadger.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Threading;
using StreamBadger.Services;
using StreamBadger.Clients;
using StreamBadger.Shared;

namespace StreamBadger
{
    public class Startup : IStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazorWebView();
            services.AddSingleton<WeatherForecastService>();

            services.AddSingleton<ImageStore>();
            services.AddSingleton<SoundStore>();
            services.AddSingleton<TwitchAuth>();
            //services.AddHostedService<TwitchBot>();
            
            services.AddHttpClient<ServerClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:25293");
            });

            services.AddHttpClient<LoginClient>(client =>
            {
                client.BaseAddress = new Uri("https://streambadger.com");
            });

        }

        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UseFormsCompatibility()
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureLifecycleEvents(builder => {
                    builder.AddWindows(lifeCycleBuilder =>
                    {
                        lifeCycleBuilder.OnLaunched((windows, args) =>
                        {
                            StartOverlay();
                        });
                        lifeCycleBuilder.OnClosed((windows, args) =>
                        {
                            StopOverlay();
                        });

                    });
                });


        }

        private void StartOverlay()
        {
            var hostBuilder = StreamBadgerOverlay.Program
                .CreateHostBuilder(Array.Empty<string>());
            _overlay = hostBuilder.Build();
            _overlayTask = _overlay.StartAsync();
        }

        private void StopOverlay()
        {
            StreamBadgerOverlay.Stopdown.Stop();
        }

        private IHost _overlay;
        private Task _overlayTask;
    }
}