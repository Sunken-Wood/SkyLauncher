using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyLauncher.FluentCore;
using System.Collections.ObjectModel;
using System.IO;

namespace SkyLauncher.ViewModels
{
    public partial class MinecraftSettingViewModel : ObservableObject
    {
        // 序列化Mods集合（自动生成 Mods集合）
        [ObservableProperty]
        private ObservableCollection<ModFileInfo> _mods = new ObservableCollection<ModFileInfo>();

        // 初始化变量
        private SkyLauncher.Core.Models.MinecraftInstance _instance;
        private readonly string modsFolder;

        private string _minecraftName = "未知";
        public string MinecraftName
        {
            get { return _minecraftName; }
            set { _minecraftName = value; }
        }

        public MinecraftSettingViewModel(SkyLauncher.Core.Models.MinecraftInstance instance)
        {
            // 获取模组文件夹
            _instance = instance;
            modsFolder = _instance.ModsPath;

            // 获取游戏版本名称
            MinecraftName = _instance.Name;

            // 加载模组列表
            LoadMods();
        }

        [RelayCommand]
        private void LoadMods()
        {
            // 清空Mods集合
            Mods.Clear();

            if (!Directory.Exists(modsFolder))
            {
                return;
            }

            // 获取所有后缀名为jar的文件
            var files = Directory.GetFiles(modsFolder, "*.jar")
                .Select(fullPath => new ModFileInfo
                {
                    FullPath = fullPath,
                    FileName = Path.GetFileName(fullPath),
                    FileSize = new FileInfo(fullPath).Length
                });

            // 将文件添加到Mods集合
            foreach (var file in files)
            {
                Mods.Add(file);
            }
        }
    }
}
