using System;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace MediaOrganizer.Core
{
    public enum MediaType
    {
        Photo,
        Video,
        Other
    }

    public static class MediaHelper
    {
        private static readonly string[] PhotoExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".heic" };

        private static readonly string[] VideoExtensions =
            { ".mp4", ".mov", ".avi", ".mkv", ".wmv" };

        public static MediaType GetMediaType(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            if (PhotoExtensions.Contains(ext)) return MediaType.Photo;
            if (VideoExtensions.Contains(ext)) return MediaType.Video;
            return MediaType.Other;
        }

        public static DateTime GetCaptureOrCreationDate(string path)
        {
            var type = GetMediaType(path);
            if (type == MediaType.Photo)
            {
                var exifDate = GetExifDate(path);
                if (exifDate.HasValue)
                    return exifDate.Value;
            }

            DateTime created = File.GetCreationTime(path);
            DateTime modified = File.GetLastWriteTime(path);
            return created < modified ? created : modified;
        }

        private static DateTime? GetExifDate(string path)
        {
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(path);
                var subIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (subIfd != null &&
                    subIfd.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime date))
                {
                    return date;
                }
            }
            catch
            {
                // ignore and fall back
            }
            return null;
        }

        public static string GetMonthFolderName(int month)
        {
            string name = new DateTime(2000, month, 1).ToString("MMMM");
            return month.ToString("00") + " - " + name;
        }
    }
}
