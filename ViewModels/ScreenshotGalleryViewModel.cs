using SkyLauncher.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SkyLauncher.ViewModels;

public class ScreenshotGalleryViewModel : INotifyPropertyChanged
{
    private readonly string _screenshotFolder;

    public ScreenshotGalleryViewModel()
    {
        _screenshotFolder = MainViewModel.Instance.GetScreenshotFolder();
        Screenshots = new ObservableCollection<ScreenshotItem>();
        LoadScreenshots();
    }

    public ObservableCollection<ScreenshotItem> Screenshots { get; }

    public string ScreenshotFolderPath => _screenshotFolder;

    public int ScreenshotCount => Screenshots.Count;

    public bool HasScreenshots => Screenshots.Count > 0;

    public string EmptyMessage => $"截图文件夹为空\n{_screenshotFolder}";

    public void LoadScreenshots()
    {
        Screenshots.Clear();

        if (!Directory.Exists(_screenshotFolder))
        {
            OnPropertyChanged(nameof(HasScreenshots));
            return;
        }
        
        var imageFiles = Directory.GetFiles(_screenshotFolder)
            .Where(IsImageFile)
            .OrderByDescending(File.GetLastWriteTime);
            //.Take(30); // 非常重要，别全加载

        foreach (var file in imageFiles)
        {
            try
            {
                var bitmap = new BitmapImage();

                bitmap.BeginInit();

                bitmap.UriSource = new Uri(file);

                // 关键：
                bitmap.DecodePixelWidth = 1000;

                // 避免锁文件
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();

                bitmap.Freeze();

                Screenshots.Add(new ScreenshotItem
                {
                    FilePath = file,
                    CreateTime = File.GetLastWriteTime(file),
                    Thumbnail = bitmap
                });
            }
            catch
            {
                HandyControl.Controls.MessageBox.Show("无法获取文件","确认",MessageBoxButton.YesNo,MessageBoxImage.Error);
            }
        }

        OnPropertyChanged(nameof(ScreenshotCount));
        OnPropertyChanged(nameof(HasScreenshots));
    }

    public void OpenScreenshotFolder()
    {
        if (Directory.Exists(_screenshotFolder))
        {
            System.Diagnostics.Process.Start("explorer.exe", _screenshotFolder);
        }
    }

    private static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLower();

        return ext is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}