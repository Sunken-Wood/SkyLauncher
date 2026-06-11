using Nrk.FluentCore.Environment;
using SkyLauncher.Core.Models;
using SkyLauncher.ViewModels;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using SkyLauncher.Models;
using System.Diagnostics;
namespace SkyLauncher.Views;

public partial class ConfigPage : UserControl
{
    private readonly ConfigPageViewModel _viewModel;
    private LauncherSettings _settings;
    private MainWindow Window;

    public ConfigPage()
    {
        
        InitializeComponent();
        _viewModel = new ConfigPageViewModel();
        DataContext = _viewModel;
        _settings = LauncherSettings.Load();
        Loaded += ConfigPage_Loaded;
    }

    private void ConfigPage_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadData();
    }

    /*private void AddNewJava(object sender, RoutedEventArgs e)
    {
        _viewModel.AddJavaCommand.Execute(null);
    }*/
    private void OpenScreenshotGallery(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            mainWindow.ContentArea.Content = new Views.ScreenshotGallery();
        }
        else
        {
            HandyControl.Controls.MessageBox.Show("遇到严重错误，当前页面为 null", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void OpenResourcePackManager(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            mainWindow.ContentArea.Content = new Views.ResourcePackManager();
        }
        else
        {
            HandyControl.Controls.MessageBox.Show("遇到严重错误，当前页面为 null", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void GoToMinecraftSettingPage(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            var selectedInstance = MainViewModel.Instance.SelectedInstance;

            mainWindow.ContentArea.Content = new Views.MinecraftSettingPage(selectedInstance);
        }
        else
        {
            HandyControl.Controls.MessageBox.Show("遇到严重错误，当前页面为 null", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}