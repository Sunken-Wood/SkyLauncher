using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyLauncher.FluentCore;
using System.Collections.ObjectModel;
using System.IO;

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

            var files = Directory.GetFiles(modsFolder, "*.jar")
                .Select(fullPath => new ModFileInfo
                {
                    FullPath = fullPath,
                    FileName = Path.GetFileName(fullPath),
                    FileSize = new FileInfo(fullPath).Length
                });
            foreach (var file in files)
            {
                Mods.Add(file);
            }
        }
    }
}
