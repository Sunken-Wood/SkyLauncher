using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLauncher.FluentCore
{
    public class ModFileInfo
    {
        public string FileName { get; set; } // 模组文件名
        public string FullPath { get; set; } // 模组文件全名
        public long FileSize { get; set; } // 模组文件大小
        public string FormattedSize => FormatSize(FileSize); // 模组文件大小


        // 文件大小格式化
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
