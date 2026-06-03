using MahApps.Metro.IconPacks;
using SkyLauncher.Core.Models;
using SkyLauncher.Views;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkyLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 用一个字段记录当前显示的页面
        private Page _currentPage;
        private LauncherSettings _settings;

        public MainWindow()
        {
            InitializeComponent();

            // 加载设置
            _settings = LauncherSettings.Load();

            // 窗口加载时默认显示主页并应用保存的设置
            Loaded += (s, e) =>
            {
                GoToMainPage();
                ApplySavedSettings();
            };

            // 订阅事件
            ToolboxPage.ThemeColorChanged += OnThemeColorChanged;
            ToolboxPage.BackgroundImagePathChanged += OnBackgroundImagePathChanged;
        }

        /// <summary>
        /// 应用保存的设置（颜色和背景图片）
        /// </summary>
        private void ApplySavedSettings()
        {
            // 应用保存的主题颜色
            if (!string.IsNullOrEmpty(_settings.ThemeColorSetting))
            {
                try
                {
                    // 解析保存的颜色字符串（格式：#RRGGBB）
                    string colorHex = _settings.ThemeColorSetting.TrimStart('#');
                    byte r = Convert.ToByte(colorHex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(colorHex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(colorHex.Substring(4, 2), 16);

                    Color savedColor = Color.FromRgb(r, g, b);
                    mainGrid.Background = new SolidColorBrush(savedColor);
                }
                catch
                {
                    // 如果颜色解析失败，使用默认颜色
                }
            }

            // 应用保存的背景图片
            if (!string.IsNullOrEmpty(_settings.PictureBackgroundPath) &&
                File.Exists(_settings.PictureBackgroundPath))
            {
                try
                {
                    ImageShow.Source = new BitmapImage(
                        new Uri(_settings.PictureBackgroundPath, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载保存的背景图片失败：{ex.Message}", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnThemeColorChanged(Color newColor)
        {
            // 主窗口响应颜色变化
            Dispatcher.Invoke(() =>
            {
                mainGrid.Background = new SolidColorBrush(newColor);
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            // 取消订阅，防止内存泄漏
            ToolboxPage.ThemeColorChanged -= OnThemeColorChanged;
            ToolboxPage.BackgroundImagePathChanged -= OnBackgroundImagePathChanged;
            base.OnClosed(e);
        }

        private void OnBackgroundImagePathChanged(string newPath)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    ImageShow.Source = new BitmapImage(new Uri(newPath, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载图片失败：{ex.Message}", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        // 注意：这个属性有问题，应该直接使用 ContentArea
        private Grid ContentGrid => mainGrid.Children.OfType<Grid>().FirstOrDefault()
                                    ?? (Grid)mainGrid.ColumnDefinitions[1]?.FindName("ContentArea");

        // 导航方法
        private void NavigateToPage(Page page, Button activeButton = null)
        {
            ContentArea.Content = page;
            _currentPage = page;
        }

        // 更新按钮样式（如果你想实现菜单高亮效果）
        private void UpdateButtonStyle(Button activeButton)
        {
            var stackPanel = mainGrid.Children.OfType<StackPanel>().FirstOrDefault();
            if (stackPanel == null) return;

            foreach (var child in stackPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.Style = this.FindResource("ButtonDefault") as Style;
                }
            }

            if (activeButton != null)
            {
                activeButton.Style = this.FindResource("ButtonPrimary") as Style;
            }
        }

        // 各个导航方法
        private void GoToMainPage(object sender = null, RoutedEventArgs e = null)
        {
            ContentArea.Content = new Views.MainPage();
        }

        private void GoToConfigPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Views.ConfigPage();
        }

        private void GoToAboutPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Views.AboutPage();
        }

        private void GoToToolboxPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Views.ToolboxPage();
        }

        private void GoToVersionManagementPage(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Views.VersionManagementPage();
        }
    }
}