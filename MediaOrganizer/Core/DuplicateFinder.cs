using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MediaOrganizer.Core
{
    public class DuplicateFileGroup
    {
        public string Hash { get; set; } = "";
        public List<string> Paths { get; set; } = new();
    }

    public class DuplicateFinder
    {
        private readonly string _rootFolder;

        public DuplicateFinder(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public List<DuplicateFileGroup> FindDuplicates()
        {
            // Stage 1: group by size to avoid hashing unique sizes
            var allFiles = Directory.GetFiles(_rootFolder, "*.*", SearchOption.AllDirectories);
            var sizeGroups = allFiles
                .Select(path => new FileInfo(path))
                .GroupBy(fi => fi.Length)
                .Where(g => g.Count() > 1);

            var duplicates = new List<DuplicateFileGroup>();

            foreach (var sizeGroup in sizeGroups)
            {
                var hashGroups = sizeGroup
                    .Select(fi => new
                    {
                        Path = fi.FullName,
                        Hash = ComputeHash(fi.FullName)
                    })
                    .GroupBy(x => x.Hash)
                    .Where(g => g.Count() > 1);

                foreach (var hashGroup in hashGroups)
                {
                    duplicates.Add(new DuplicateFileGroup
                    {
                        Hash = hashGroup.Key,
                        Paths = hashGroup.Select(x => x.Path).ToList()
                    });
                }
            }

            return duplicates;
        }

        private static string ComputeHash(string path)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            byte[] hash = sha.ComputeHash(stream);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
