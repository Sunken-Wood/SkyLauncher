using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SkyLauncher.Core.Services;

namespace SkyLauncher.ViewModels;

public class DownloadViewModel : INotifyPropertyChanged
{
    private string _statusText = "等待选择整合包...";
    private double _progress;
    private bool _isInstalling;
    private CancellationTokenSource? _cts;

    private readonly ModpackInstallService _installService;

    public DownloadViewModel()
    {
        PickPackageCommand = new RelayCommand(ExecutePickPackage);
        CancelCommand = new RelayCommand(ExecuteCancel);

        _installService = new ModpackInstallService();
        _installService.StatusChanged += (s) =>
        {
            Application.Current.Dispatcher.Invoke(() => StatusText = s);
        };
        _installService.ProgressChanged += (p) =>
        {
            Application.Current.Dispatcher.Invoke(() => Progress = p);
        };
    }

    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }

    public double Progress
    {
        get => _progress;
        set { _progress = value; OnPropertyChanged(); }
    }

    public bool IsInstalling
    {
        get => _isInstalling;
        set { _isInstalling = value; OnPropertyChanged(); }
    }

    public ICommand PickPackageCommand { get; }
    public ICommand CancelCommand { get; }

    private async void ExecutePickPackage()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择整合包文件",
            Filter = "整合包文件|*.zip;*.mrpack|所有文件|*.*"
        };

        if (dialog.ShowDialog() != true) return;

        IsInstalling = true;
        Progress = 0;
        _cts = new CancellationTokenSource();

        try
        {
            var filePath = dialog.FileName;
            var minecraftFolder = MainViewModel.Instance.GetActualMinecraftFolder();
            var javaPath = MainViewModel.Instance.JavaExecutablePath;

            if (string.IsNullOrEmpty(javaPath))
            {
                MessageBox.Show("请先在设置中配置 Java 运行时", "错误");
                return;
            }

            var ext = Path.GetExtension(filePath).ToLower();

            if (ext == ".mrpack")
            {
                await _installService.InstallModrinthAsync(filePath, minecraftFolder, javaPath, _cts.Token);
            }
            else if (ext == ".zip")
            {
                await _installService.InstallCurseForgeAsync(filePath, minecraftFolder, javaPath, _cts.Token);
            }

            // 刷新实例列表
            MainViewModel.Instance.ScanInstances(minecraftFolder);

            StatusText = "安装完成！请到版本管理页面查看新实例。";
            MessageBox.Show("整合包安装完成！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (OperationCanceledException)
        {
            StatusText = "已取消安装";
        }
        catch (Exception ex)
        {
            StatusText = "安装失败";
            MessageBox.Show($"安装失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsInstalling = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void ExecuteCancel()
    {
        _cts?.Cancel();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}