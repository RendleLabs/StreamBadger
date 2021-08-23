using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.LifecycleEvents;
using MudBlazor.Services;
using StreamBadger.Shared;
using StreamBadger.Shared.Clients;

[assembly: XamlCompilationAttribute(XamlCompilationOptions.Compile)]

namespace StreamBadger
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .RegisterBlazorMauiWebView()
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices(ConfigureServices);

#if(WINDOWS)
			appBuilder
                .ConfigureLifecycleEvents(builder =>
                {
                    builder.AddWindows(lifeCycleBuilder =>
                    {
                        lifeCycleBuilder.OnLaunched((_, _) =>
                        {
                            StartOverlay();
                        });
                        lifeCycleBuilder.OnClosed((_, _) =>
                        {
                            StopOverlay();
                        });

                    });
                });
#endif
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazorWebView();

            services.AddMudServices();

            services.AddSingleton<ImageStore>();
            services.AddSingleton<SoundStore>();
            services.AddSingleton<TwitchAuth>();
            services.AddSingleton<SoundTemp>();
            services.AddSingleton<SettingsStore>();

#if (WINDOWS)
            services.AddHttpClient<IServerClient, ServerClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:25293");
            });

            services.AddHttpClient<LoginClient>(client =>
            {
                client.BaseAddress = new Uri("https://streambadger.com");
            });
#endif
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
            try
            {
                _overlay?.Dispose();
            }
            catch
            {
                // Ignore
            }
        }

        private IHost _overlay;
        private Task _overlayTask;
    }
}