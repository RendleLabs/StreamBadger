using Microsoft.Extensions.Hosting;

namespace StreamBadgerOverlay
{
    public static class Stopdown
    {
        internal static IHostApplicationLifetime Lifetime { get; set; }

        public static void Stop()
        {
            if (Lifetime is not null)
            {
                Lifetime.StopApplication();
            }
        }
    }
}
