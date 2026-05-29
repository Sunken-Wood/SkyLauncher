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
using SkyLauncher.Views;

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
    }
}
