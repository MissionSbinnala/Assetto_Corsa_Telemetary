using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace Telemetry.Data
{
    public static class FileTreeBuilder
    {
        public static FileNode BuildSeparated(string folderPath, int depth = 1, int maxDepth = 3, bool extend = true)
        {
            var result = new FileNode();

            // 当前目录下的文件
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (extend)
                    result.Files.Add(new FileNode
                    {
                        Name = Path.GetFileName(file),
                        DisplayName = Path.GetFileName(file)
                    });
                else
                    result.Files.Add(new FileNode
                    {
                        Name = Path.GetFileName(file),
                        DisplayName = Path.GetFileName(file).Split('.').First()
                    });
            }

            // 当前目录下的文件夹
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                var folderNode = new FileNode();

                if (depth < maxDepth)
                {
                    folderNode = BuildSeparated(dir, depth + 1, maxDepth, extend);
                }

                folderNode.Name = Path.GetFileName(dir);
                folderNode.DisplayName = folderNode.Name;

                result.Folders.Add(folderNode);
            }

            return result;
        }
    }

    public class FileNode
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public List<FileNode> Files { get; set; } = new();
        public List<FileNode> Folders { get; set; } = new();

        public IEnumerable<FileNode> GetAllFolders()
        {
            if (Folders.Count == 0) yield break;

            foreach (var child in Folders)
                yield return child;
        }

        public IEnumerable<FileNode> GetAllFiles(string? extends = null)
        {
            if (Files.Count == 0) yield break;

            if (extends is null)
                foreach (var child in Files)
                    yield return child;

            foreach (var child in Files.Where(file => file.Name.EndsWith(extends)))
                yield return child;
        }
    }
}
