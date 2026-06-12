using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using SkyLauncher.Core.Models;
using SkyLauncher.Service;
using SkyLauncher.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// ToolboxPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolboxPage : UserControl, INotifyPropertyChanged
    {
        private LauncherSettings _settings;
        private readonly string _token;
        public event EventHandler<double> OpacityChanged;
        public ToolboxPage()
        {
            InitializeComponent();
            _settings = LauncherSettings.Load(); // 先加载设置
            //_token = token;
            if (_settings != null)
            {
                OpacitySlider.Value = _settings.LauncherOpacity;
                Debug.WriteLine(_settings.LauncherOpacity);
            }
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private bool isFirstLoad = true;

        /*private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isFirstLoad)
            {
                isFirstLoad = false;
                return;
            }
            if (MainTabControl.SelectedIndex == 0)
            {
                var currentContent = ThemeTransition.Content;
                ThemeTransition.Content = null;
                ThemeTransition.Content = currentContent;
            }
        }*/
        public string ThemeColor
        {
            get => _settings?.ThemeColorSetting;
            set
            {
                if (_settings != null)
                {
                    _settings.ThemeColorSetting = value;
                    _settings.Save();
                    OnPropertyChanged();
                }
            }
        }

        private void SwitchToLight(object sender, RoutedEventArgs e)
        {
            /*var dicts = Application.Current.Resources.MergedDictionaries;
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                var source = dicts[i].Source?.ToString();
                if (source != null && source.Contains("Skin"))
                {
                    dicts.RemoveAt(i);
                }
            }
            Application.Current.Resources.MergedDictionaries.Add(
                ResourceHelper.GetSkin(SkinType.Default));
            var brush = Application.Current.FindResource("RegionBrush") as SolidColorBrush;
            Debug.WriteLine(brush.Color);
            dicts.Add(ResourceHelper.GetSkin(SkinType.Default));

            */
        }


        private void SwitchToDark(object sender, RoutedEventArgs e)
        {
            /*var dicts = Application.Current.Resources.MergedDictionaries;
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                var source = dicts[i].Source?.ToString();
                if (source != null && source.Contains("Skin"))
                {
                    dicts.RemoveAt(i);
                }
            }
            Application.Current.Resources.MergedDictionaries.Add(
                ResourceHelper.GetSkin(SkinType.Dark));
            var brush = Application.Current.FindResource("RegionBrush") as SolidColorBrush;
            Debug.WriteLine(brush.Color);
            dicts.Add(ResourceHelper.GetSkin(SkinType.Dark));

            */
        }


        public System.Windows.Media.ImageSource BackgroundImagePath { get; set; }

        public static event Action<Color> ThemeColorChanged;
        public static event Action<String> BackgroundImagePathChanged;

        private void FlipClock_Show(object sender = null, RoutedEventArgs e = null)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.NavigateToPage(() => new Views.FlipClock());
            }
        }



        private void ColorPicker_OnSelectedColorChanged(object sender, FunctionEventArgs<Color> e)
        {
            if (e.Info != null)
            {
                Color selectedColor = e.Info;
                ThemeColor = $"#{selectedColor.A:X2}{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                //System.Diagnostics.Debug.WriteLine($"OnSelectedColorChanged - A:{selectedColor.A} R:{selectedColor.R} G:{selectedColor.G} B:{selectedColor.B}");
                ThemeColorChanged?.Invoke(selectedColor);
            }
        }

        private void ConfirmSelectPic(object sender, RoutedEventArgs e)
        {
            string imagePath = ImageSelector.Uri?.OriginalString;

            if (imagePath == null)
            {
                HandyControl.Controls.MessageBox.Show("请选择一张图片！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                BackgroundImagePathChanged?.Invoke(imagePath);
                ImageSource imageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                BackgroundImagePath = imageSource;

                // 同时保存图片路径到设置
                if (_settings != null)
                {
                    _settings.PictureBackgroundPath = imagePath;
                    _settings.Save();
                }
            }
        }

        private void ColorPicker_ColorConfirmed(object sender, FunctionEventArgs<Color> e)
        {
            if (e.Info != null)
            {
                //Color c = e.Info;
                //System.Diagnostics.Debug.WriteLine($"ColorConfirmed - A:{c.A} R:{c.R} G:{c.G} B:{c.B}");
                Color selectedColor = e.Info;
                ThemeColor = $"#{selectedColor.A:X2}{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                HandyControl.Controls.MessageBox.Show($"已选择颜色: {ThemeColor}", "颜色选择", MessageBoxButton.OK, MessageBoxImage.Information);
                //public RelayCommand InfoCmd => new(() => Growl.Info(Properties.Langs.Lang.GrowlInfo, _token));
            }
        }

        private void ConfirmOpacity(object sender, RoutedEventArgs e)
        {
            double opacity = OpacitySlider.Value;

            if (_settings != null)
            {
                _settings.LauncherOpacity = (float)opacity;
                _settings.Save();
                OpacityChanged?.Invoke(this, opacity);
                Debug.WriteLine(_settings.LauncherOpacity);
                Debug.WriteLine(_settings.LauncherOpacity.GetType());
                HandyControl.Controls.MessageBox.Show(
                    $"已设置启动器不透明度为: {opacity:F2}",
                    "设置成功",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
               
            }
        }


    }
}