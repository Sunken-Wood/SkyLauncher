using SkyLauncher.ViewModels;
using System;
using System.Windows.Controls;

namespace SkyLauncher.Views
{
    public partial class MinecraftSettingPage : UserControl
    {
        public MinecraftSettingPage()
        {
            InitializeComponent();
        }

        public MinecraftSettingPage(SkyLauncher.Core.Models.MinecraftInstance instance) : this()
        {
            // 创建 ViewModel 并设置 DataContext
            DataContext = new MinecraftSettingViewModel(instance);

            // 加载完成后订阅事件
            Loaded += MinecraftSettingPage_Loaded;
        }

        private void MinecraftSettingPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= MinecraftSettingPage_Loaded; // 取消订阅，避免重复

            // 为所有已存在的模组订阅属性变化事件
            if (DataContext is MinecraftSettingViewModel viewModel)
            {
                foreach (var mod in viewModel.Mods)
                {
                    mod.PropertyChanged += Mod_PropertyChanged;
                }

                // 订阅集合变化，以便后续添加的模组也能响应
                viewModel.Mods.CollectionChanged += (s, args) =>
                {
                    if (args.NewItems != null)
                    {
                        foreach (SkyLauncher.FluentCore.ModFileInfo item in args.NewItems)
                        {
                            item.PropertyChanged += Mod_PropertyChanged;
                        }
                    }

                    if (args.OldItems != null)
                    {
                        foreach (SkyLauncher.FluentCore.ModFileInfo item in args.OldItems)
                        {
                            item.PropertyChanged -= Mod_PropertyChanged;
                        }
                    }
                };
            }
        }

        private void Mod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SkyLauncher.FluentCore.ModFileInfo.IsEnabled)
                && sender is SkyLauncher.FluentCore.ModFileInfo modInfo
                && DataContext is MinecraftSettingViewModel viewModel)
            {
                viewModel.ToggleModEnabled(modInfo, modInfo.IsEnabled);
            }
        }
    }
}