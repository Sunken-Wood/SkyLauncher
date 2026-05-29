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

    private bool _isMansualCollocation = true;
    public bool IsMansualCollocation
    {
        get => _isMansualCollocation;
        set { 
            _isMansualCollocation = value;
            OnPropertyChanged();
        }
    }

    private bool _isAutoCollocation;
    public bool IsAutoCollocation
    {
        get => _isAutoCollocation;
        set
        {
            if(value)
            {
                MainViewModel.Instance.MaxMemoryMB = (int)(MemoryUtils.GetWindowsMetrics().Free * 0.8);
            }
            _isAutoCollocation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MaxMemory));
        }
    }

    public ConfigPageViewModel()
    {
        AddJavaCommand = new RelayCommand(ExecuteAddJava);
        AutoDetectJavaCommand = new RelayCommand(ExecuteAutoDetectJava);
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
        else MessageBox.Show("未找到已安装的 Java");
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