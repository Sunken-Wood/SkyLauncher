using SkyLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkyLauncher.Views
{
    /// <summary>
    /// MinecraftSettingPage.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MinecraftSettingPage : UserControl
    {
        private readonly MinecraftSettingViewModel _viewModel;
        public MinecraftSettingPage(SkyLauncher.Core.Models.MinecraftInstance instance)
        {
            InitializeComponent();
            _viewModel = new MinecraftSettingViewModel(instance);
            DataContext = _viewModel;
            
        }

    }
}
