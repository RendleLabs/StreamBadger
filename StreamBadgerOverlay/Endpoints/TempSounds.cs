using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StreamBadger;
using StreamBadgerOverlay.Services;

namespace StreamBadgerOverlay.Endpoints
{
    public class TempSounds
    {
        public static async Task Play(HttpContext context)
        {
            var soundTemp = context.RequestServices.GetRequiredService<SoundTemp>();
            if (!context.Request.RouteValues.TryGetValue("sound", out var sound) || sound is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (soundTemp.TryGetPath(sound!.ToString(), out var path, out var contentType))
            {
                context.Response.ContentType = contentType;
                await context.Response.SendFileAsync(path);
            }
        }
    }
}