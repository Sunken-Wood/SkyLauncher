using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyLauncher.FluentCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HandyControl.Controls;

namespace SkyLauncher.ViewModels
{
    public partial class MinecraftSettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ModFileInfo> _mods = new ObservableCollection<ModFileInfo>();

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
            _instance = instance;
            modsFolder = _instance.ModsPath;
            MinecraftName = _instance.Name;
            LoadMods();
        }

        [RelayCommand]
        private void LoadMods()
        {
            Mods.Clear();

            if (!Directory.Exists(modsFolder))
            {
                return;
            }

            var jarFiles = Directory.GetFiles(modsFolder, "*.jar")
                .Select(fullPath => new ModFileInfo
                {
                    FullPath = fullPath,
                    FileName = Path.GetFileName(fullPath),
                    FileSize = new FileInfo(fullPath).Length,
                    IsEnabled = true
                });

            var disabledFiles = Directory.GetFiles(modsFolder, "*.jar.disabled")
                .Select(fullPath => new ModFileInfo
                {
                    FullPath = fullPath,
                    FileName = Path.GetFileName(fullPath),
                    FileSize = new FileInfo(fullPath).Length,
                    IsEnabled = false
                });

            foreach (var file in jarFiles.Concat(disabledFiles))
            {
                Mods.Add(file);
            }
        }

        public void ToggleModEnabled(ModFileInfo mod, bool isEnabled)
        {
            try
            {
                string currentPath = mod.FullPath;
                string directory = Path.GetDirectoryName(currentPath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentPath);

                if (isEnabled)
                {
                    // 从 .disabled 改为 .jar
                    string newPath = Path.Combine(directory, fileNameWithoutExtension);
                    if (File.Exists(currentPath))
                    {
                        File.Move(currentPath, newPath);
                        mod.FullPath = newPath;
                        mod.FileName = Path.GetFileName(newPath);
                    }
                }
                else
                {
                    // 从 .jar 改为 .disabled
                    string newPath = Path.Combine(directory, fileNameWithoutExtension + ".jar.disabled");
                    if (File.Exists(currentPath))
                    {
                        File.Move(currentPath, newPath);
                        mod.FullPath = newPath;
                        mod.FileName = Path.GetFileName(newPath);
                    }
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show("切换模组状态失败，请检查文件权限或是否被其他程序占用。", "错误",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
            }
        }
    }
}