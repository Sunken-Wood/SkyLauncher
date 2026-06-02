using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Themes; 
using SkyLauncher.Views;
using System;
using System.Collections.Generic;
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
using System.IO;
namespace SkyLauncher.Views
{
    /// <summary>
    /// ToolboxPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolboxPage : UserControl
    {
        public ToolboxPage()
        {
            InitializeComponent();

        }
        public string ThemeColor { get; set; }
        public System.Windows.Media.ImageSource BackgroundImagePath { get; set; }
        public static event Action<Color> ThemeColorChanged;
        public static event Action<String> BackgroundImagePathChanged;
        private void FlipClock_Show(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ContentArea.Content = new Views.FlipClock();
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("遇到严重错误，当前页面为 null", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ColorPicker_OnSelectedColorChanged(object sender, FunctionEventArgs<Color> e)
        {

            if (e.Info != null)
            {
                System.Windows.Media.Color currentColor = e.Info;

                Color selectedColor = e.Info;
                //ThemeManager.Current.UpdateAccentColor(newAccentColor);
                SolidColorBrush userBrush = new SolidColorBrush(selectedColor);

                ThemeColor = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                ThemeColorChanged?.Invoke(selectedColor);
            }
        }
        public void ConfirmSelectPic(object sender, RoutedEventArgs e)
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
            }
        }
    }
}
