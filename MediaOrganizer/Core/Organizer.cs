using System;
using System.IO;
using MediaOrganizer.Core;

namespace MediaOrganizer.Core
{
    public class Organizer
    {
        private readonly string _sourceFolder;
        private readonly string _targetFolder;
        private readonly bool _copy;

        public Organizer(string sourceFolder, string targetFolder, bool copy)
        {
            _sourceFolder = sourceFolder;
            _targetFolder = targetFolder;
            _copy = copy;
        }

        public void Organize()
        {
            string[] files = Directory.GetFiles(_sourceFolder, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                var type = MediaHelper.GetMediaType(file);
                if (type == MediaType.Other)
                    continue;

                DateTime date = MediaHelper.GetCaptureOrCreationDate(file);

                string baseFolder = type == MediaType.Photo ? "Photos" : "Videos";
                string yearFolder = date.Year.ToString();
                string monthFolder = MediaHelper.GetMonthFolderName(date.Month);

                string destFolder = Path.Combine(_targetFolder, baseFolder, yearFolder, monthFolder);
                Directory.CreateDirectory(destFolder);

                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(destFolder, fileName);
                destPath = GetUniquePath(destPath);

                if (_copy)
                {
                    File.Copy(file, destPath);
                }
                else
                {
                    File.Move(file, destPath);
                }
            }
        }

        private static string GetUniquePath(string path)
        {
            if (!File.Exists(path)) return path;

            string dir = Path.GetDirectoryName(path)!;
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int i = 1;

            string newPath;
            do
            {
                newPath = Path.Combine(dir, $"{name} ({i}){ext}");
                i++;
            } while (File.Exists(newPath));

            return newPath;
        }
    }
}
