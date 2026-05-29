using MahApps.Metro.IconPacks;
using SkyLauncher.Views;
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

        public MainWindow()
        {
            InitializeComponent();

            // 窗口加载时默认显示主页
            Loaded += (s, e) => GoToMainPage();
        }

        private Grid ContentGrid => mainGrid.Children.OfType<Grid>().FirstOrDefault()
                                    ?? (Grid)mainGrid.ColumnDefinitions[1]?.FindName("ContentArea");


        // 导航方法
        private void NavigateToPage(Page page, Button activeButton = null)
        {
            // 切换内容
            ContentArea.Content = page;
            _currentPage = page;

            //高亮当前选中的按钮
            //UpdateButtonStyle(activeButton);
        }

        // 更新按钮样式（如果你想实现菜单高亮效果）
        private void UpdateButtonStyle(Button activeButton)
        {
            // 获取所有侧边栏按钮
            var stackPanel = mainGrid.Children.OfType<StackPanel>().FirstOrDefault();
            if (stackPanel == null) return;

            foreach (var child in stackPanel.Children)
            {
                if (child is Button btn)
                {
                    // 重置为默认样式
                    btn.Style = this.FindResource("ButtonDefault") as Style;

                    /*
                    var dockPanel = btn.Content as DockPanel;
                    if (dockPanel?.Children[0] is iconPacks:PackIconBase icon)
                    {
                        icon.Foreground = this.FindResource("SecondaryTextBrush") as System.Windows.Media.Brush;
                    }*/
                }
            }

            // 高亮当前按钮
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