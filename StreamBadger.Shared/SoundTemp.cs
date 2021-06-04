using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using StreamBadger.Shared;

namespace StreamBadger
{
    public class SoundTemp : IDisposable
    {
        public async Task<string> StoreAsync(Stream stream, string contentType)
        {
            if (!ContentTypes.TryGetFileExtension(contentType, out var extension))
            {
                extension = "mp3";
            }

            var fileName = $"sb_temp_sound.{extension}";
            var tempFile = Path.Combine(Path.GetTempPath(), fileName);
            await using var target = File.Create(tempFile);
            await stream.CopyToAsync(target);
            return fileName;
        }

        public bool TryGetPath(string name, [NotNullWhen(true)] out string? path, [NotNullWhen(true)] out string? contentType)
        {
            path = Path.Combine(Path.GetTempPath(), name);
            if (!File.Exists(path))
            {
                contentType = null;
                return false;
            }
            var extension = Path.GetExtension(name).TrimStart('.');
            if (!ContentTypes.TryGetContentType(extension, out contentType))
            {
                contentType = "audio/mpeg";
            }

            return true;
        }

        public void Dispose()
        {
            var tempFiles = Path.Combine(Path.GetTempPath(), "sb_temp_sound.*");
            foreach (var file in Directory.EnumerateFiles(Path.GetTempPath(), "sb_sound_temp.*"))
            {
                File.Delete(file);
            }
        }
    }
}