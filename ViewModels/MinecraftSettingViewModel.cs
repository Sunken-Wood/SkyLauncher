using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nrk.FluentCore.GameManagement.Mods;
using SkyLauncher.Core.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;

namespace SkyLauncher.ViewModels
{
    public partial class MinecraftSettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MinecraftMod> _mods = new ObservableCollection<MinecraftMod>();

        private MinecraftInstance _instance;
        private readonly string modsFolder;

        private string _minecraftName = "未知";
        public string MinecraftName
        {
            get => _minecraftName;
            set => SetProperty(ref _minecraftName, value);
        }

        public MinecraftSettingViewModel(MinecraftInstance instance)
        {
            _instance = instance;
            modsFolder = _instance.ModsPath;
            MinecraftName = _instance.Name;
            LoadMods();
        }

        [RelayCommand]
        private async void LoadMods()
        {
            Mods.Clear();

            if (!Directory.Exists(modsFolder))
            {
                return;
            }

            // 使用 FluentCore 的 ModManager 枚举模组
            await foreach (var mod in ModManager.EnumerateModsAsync(modsFolder))
            {
                // 如果 DisplayName 为空，使用文件名
                if (string.IsNullOrEmpty(mod.DisplayName))
                {
                    mod.DisplayName = Path.GetFileNameWithoutExtension(mod.AbsolutePath);
                }
                Mods.Add(mod);
            }
        }

        [RelayCommand]
        public void ToggleModEnabled(MinecraftMod mod)
        {
            try
            {
                // 使用 FluentCore 的扩展方法切换模组状态
                mod.Switch(!mod.IsEnabled);

                // 刷新集合中的项（触发 UI 更新）
                var index = Mods.IndexOf(mod);
                if (index >= 0)
                {
                    Mods[index] = mod;
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(
                    $"切换模组状态失败：{ex.Message}\n请检查文件权限或是否被其他程序占用。",
                    "错误",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void DeleteMod(MinecraftMod mod)
        {
            var result = HandyControl.Controls.MessageBox.Show(
                $"确定要删除模组 \"{mod.DisplayName}\" 吗？此操作不可恢复。",
                "确认删除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    // 使用 FluentCore 的扩展方法删除模组
                    mod.Delete();
                    Mods.Remove(mod);
                }
                catch (Exception ex)
                {
                    HandyControl.Controls.MessageBox.Show(
                        $"删除模组失败：{ex.Message}",
                        "错误",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void OpenModsFolder()
        {
            if (Directory.Exists(modsFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", modsFolder);
            }
            else
            {
                HandyControl.Controls.MessageBox.Show(
                    "模组文件夹不存在！",
                    "错误",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }
}