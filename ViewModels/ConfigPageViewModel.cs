using Microsoft.Win32;
using Nrk.FluentCore.Environment;
using SkyLauncher.Core.Models;
using SkyLauncher.Core.Services;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SkyLauncher.ViewModels;

public class ConfigPageViewModel : INotifyPropertyChanged
{
    public static ConfigPageViewModel? _instance;
    public static ConfigPageViewModel Instance => _instance ??= new ConfigPageViewModel();
    private LauncherSettings _settings;

    public string _isMansualCollocation ;
    //_settings = LauncherSettings.Load();

    public bool IsManualCollocation
    {
        get => (bool)_settings.MansualCollocation;  // 注意：如果你的settings里也是MansualCollocation，保持一致
        set
        {
            if ((bool)_settings.MansualCollocation != value)
            {
                _settings.MansualCollocation = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAutoCollocation));
                OnPropertyChanged(nameof(IsManualModeEnabled)); // 添加这个通知
            }
        }
    }

    public bool IsAutoCollocation
    {
        get => !IsManualCollocation;  // 直接使用IsManualCollocation，保证逻辑一致
        set
        {
            if (value)
            {
                IsManualCollocation = false;
                // IsManualCollocation的setter已经包含了所有通知，这里不需要重复
            }
            // 添加：如果设为false（即用户点击自动分配但当前已是自动）
            else if (!IsManualCollocation)
            {
                // 手动设为手动模式
                IsManualCollocation = true;
            }
        }
    }

    // 添加这个属性用于控制Slider的启用状态
    public bool IsManualModeEnabled => IsManualCollocation;
    public ConfigPageViewModel()
    {
        AddJavaCommand = new RelayCommand(ExecuteAddJava);
        AutoDetectJavaCommand = new RelayCommand(ExecuteAutoDetectJava);
        _settings = LauncherSettings.Load();
    }

    public double MaxMemory
    {
        get => MainViewModel.Instance.MaxMemoryMB;
        set { 
            MainViewModel.Instance.MaxMemoryMB = (int)value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MinMemory));
            OnPropertyChanged(nameof(MemoryDisplayText));
        }
    }

    public double FreeMemory
    {
        get => (int)(MemoryUtils.GetWindowsMetrics().Free*0.8);
    }

    public double MinMemory
    {
        get => MainViewModel.Instance.MinMemoryMB;
        set { MainViewModel.Instance.MinMemoryMB = (int)value; OnPropertyChanged(); OnPropertyChanged(nameof(MemoryDisplayText)); }
    }

    public string MemoryDisplayText =>
        $"当前: {MainViewModel.Instance.MinMemoryMB}MB ~ {MainViewModel.Instance.MaxMemoryMB}MB ({MainViewModel.Instance.MaxMemoryMB / 1024}GB)";

    public System.Collections.ObjectModel.ObservableCollection<JavaRuntime> JavaList => MainViewModel.Instance.JavaList;

    private JavaRuntime? _selectedJava;
    public JavaRuntime? SelectedJava
    {
        get => _selectedJava;
        set
        {
            _selectedJava = value;
            if (value != null)
            {
                MainViewModel.Instance.JavaExecutablePath = value.ExecutablePath;
                MainViewModel.Instance.JavaVersion = value.Version;
            }
            OnPropertyChanged();
        }
    }

    public ICommand AddJavaCommand { get; }
    public ICommand AutoDetectJavaCommand { get; }

    private void ExecuteAddJava()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择 Java 可执行文件",
            Filter = "Java 运行时|javaw.exe;java.exe|所有文件|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            var java = JavaRuntimeService.ImportJava(dialog.FileName);
            if (java != null)
            {
                if (!JavaList.Any(j => j.ExecutablePath == java.ExecutablePath))
                    JavaList.Add(java);
                SelectedJava = java;
            }
        }
    }
 
    private void ExecuteAutoDetectJava()
    {
        var javaList = JavaRuntimeService.ScanInstalledJava();
        JavaList.Clear();
        foreach (var java in javaList) JavaList.Add(java);
        if (JavaList.Count > 0) SelectedJava = JavaList[0];
        else HandyControl.Controls.MessageBox.Show("未找到已安装的 Java");
    }

    public void LoadData()
    {
        var saved = MainViewModel.Instance.JavaExecutablePath;
        if (!string.IsNullOrEmpty(saved))
        {
            var existing = JavaList.FirstOrDefault(j => j.ExecutablePath == saved);
            if (existing != null) SelectedJava = existing;
        }
        OnPropertyChanged(nameof(MaxMemory));
        OnPropertyChanged(nameof(MinMemory));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}