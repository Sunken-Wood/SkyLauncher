using System.Windows;
using System.Windows.Controls;
using SkyLauncher.ViewModels;

namespace SkyLauncher.Views;

public partial class VersionManagementPage : UserControl
{
    public VersionManagementPage()
    {
        InitializeComponent();
        DataContext = VersionManagementViewModel.Instance;
        Loaded += VersionManagementPage_Loaded;
    }

    private void VersionManagementPage_Loaded(object sender, RoutedEventArgs e)
    {
        // 页面加载时自动扫描（如果还没扫过）
        if (VersionManagementViewModel.Instance.InstanceList.Count == 0)
        {
            VersionManagementViewModel.Instance.RefreshCommand.Execute(null);
        }
    }
    private void DeleteInstance_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is SkyLauncher.Core.Models.MinecraftInstance instance)
        {
            VersionManagementViewModel.Instance.DeleteInstanceCommand.Execute(instance);
        }
    }
}