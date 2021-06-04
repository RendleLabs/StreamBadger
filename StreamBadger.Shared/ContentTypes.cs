using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StreamBadger.Shared
{
    public static class ContentTypes
    {
        private static readonly Dictionary<string, string> ContentTypeToFileExtension = new(StringComparer.OrdinalIgnoreCase)
        {
            ["audio/mpeg"] = "mp3",
            ["audio/ogg"] = "ogg",
            ["audio/wav"] = "wav",
        };

        private static readonly Dictionary<string, string> FileExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
        {
            ["mp3"] = "audio/mpeg",
            ["ogg"] = "audio/ogg",
            ["wav"] = "audio/wav",
        };

        public static bool TryGetFileExtension(string contentType, [NotNullWhen(true)] out string? extension) =>
            ContentTypeToFileExtension.TryGetValue(contentType, out extension);

        public static bool TryGetContentType(string fileExtension, [NotNullWhen(true)] out string? contentType) =>
            FileExtensionToContentType.TryGetValue(fileExtension.TrimStart('.'), out contentType);
    }
}