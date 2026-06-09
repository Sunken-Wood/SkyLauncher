// SkyLauncher/ViewModels/VersionManagementViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using SkyLauncher.Core.Models;
using SkyLauncher.Views;

namespace SkyLauncher.ViewModels;

public class VersionManagementViewModel : INotifyPropertyChanged
{
    private static VersionManagementViewModel? _instance;
    public static VersionManagementViewModel Instance => _instance ??= new VersionManagementViewModel();


    public VersionManagementViewModel()
    {
        RefreshCommand = new RelayCommand(ExecuteRefresh);
        ImportVersionCommand = new RelayCommand(ExecuteImportVersion);
        ImportFolderCommand = new RelayCommand(ExecuteImportFolder);
        DeleteInstanceCommand = new RelayCommand<SkyLauncher.Core.Models.MinecraftInstance>(ExecuteDeleteInstance);
    }

    public ObservableCollection<SkyLauncher.Core.Models.MinecraftInstance> InstanceList => MainViewModel.Instance.InstanceList;

    private SkyLauncher.Core.Models.MinecraftInstance? _selectedInstance;
    public SkyLauncher.Core.Models.MinecraftInstance? SelectedInstance
    {
        get => _selectedInstance;
        set
        {
            _selectedInstance = value;
            if (value != null) MainViewModel.Instance.SelectedInstance = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedInstanceInfo));
        }
    }

    private void ExecuteDeleteInstance(SkyLauncher.Core.Models.MinecraftInstance? instance)
    {
        if (instance == null) return;

        var result = HandyControl.Controls.MessageBox.Show(
            $"确定要删除实例 \"{instance.Name}\" 吗？\n\n此操作不会删除游戏文件。",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            MainViewModel.Instance.RemoveInstance(instance);

            // 如果删除的是当前选中的，清空选择
            if (SelectedInstance == instance)
                SelectedInstance = null;

            OnPropertyChanged(nameof(InstanceList));
        }
    }
    public string SelectedInstanceInfo
    {
        get
        {
            //var deleteButton = Application.Current.MainWindow?.FindName("DeleteInstanceButton") as Button;
            if (SelectedInstance == null) return "未选择实例";
            var inst = SelectedInstance;
            return $"实例：{inst.Name}\n版本id：{inst.VersionId}\n文件路径： {inst.GameDirectory}\n{(inst.LoaderName ?? "原版")}";
            //deleteButton.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand ImportVersionCommand { get; }
    public ICommand ImportFolderCommand { get; }
    public ICommand DeleteInstanceCommand { get; }


    private void ExecuteRefresh()
    {
        MainViewModel.Instance.ScanInstances();
        OnPropertyChanged(nameof(InstanceList));
    }

    private void ExecuteImportVersion()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择版本 JSON 文件",
            Filter = "Minecraft 版本 JSON|*.json|所有文件|*.*",
            InitialDirectory = MainViewModel.Instance.DefaultMinecraftFolder
        };

        var mainWindow = Application.Current.MainWindow;
        bool? result = mainWindow != null ? dialog.ShowDialog(mainWindow) : dialog.ShowDialog();

        if (result == true)
        {
            ImportVersionFromJson(dialog.FileName);
        }
    }

    private void ExecuteImportFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "选择版本文件夹",
            InitialDirectory = MainViewModel.Instance.DefaultMinecraftFolder
        };

        var mainWindow = Application.Current.MainWindow;
        bool? result = mainWindow != null
            ? dialog.ShowDialog(mainWindow)
            : dialog.ShowDialog();

        if (result == true)
        {
            var folderPath = dialog.FolderName;
            var folderName = Path.GetFileName(folderPath);
            var jsonPath = Path.Combine(folderPath, $"{folderName}.json");

            if (!File.Exists(jsonPath))
            {
                var jsonFiles = Directory.GetFiles(folderPath, "*.json");
                if (jsonFiles.Length == 0)
                {
                    HandyControl.Controls.MessageBox.Show("文件夹内没有找到版本 JSON 文件", "导入失败");
                    return;
                }
                jsonPath = jsonFiles[0];
            }

            ImportVersionFromJson(jsonPath);
        }
    }

    private void ImportVersionFromJson(string jsonPath)
    {
        var versionDir = Path.GetDirectoryName(jsonPath);
        var versionName = Path.GetFileNameWithoutExtension(jsonPath);

        if (versionName == "launcher_profiles")
        {
            HandyControl.Controls.MessageBox.Show("这是 PCL 配置文件，不是版本文件", "导入失败");
            return;
        }

        var isIsolated = Directory.Exists(Path.Combine(versionDir!, "mods"))
                      || Directory.Exists(Path.Combine(versionDir!, "config"));

        var rootPath = Path.GetDirectoryName(versionDir)!;

        var instance = new SkyLauncher.Core.Models.MinecraftInstance
        {
            Name = versionName,
            RootPath = rootPath,
            GameDirectory = isIsolated ? versionDir! : rootPath,
            VersionJsonPath = jsonPath,
            VersionId = versionName,
            Source = LauncherSource.SkyLauncher,
            IsManaged = true
        };

        MainViewModel.Instance.AddInstance(instance);
        SelectedInstance = instance;
        OnPropertyChanged(nameof(InstanceList));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}