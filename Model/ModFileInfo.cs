using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLauncher.FluentCore
{
    public class ModFileInfo
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public long FileSize { get; set; }
        public string FormattedSize => FormatSize(FileSize);

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}
