using HandyControl.Controls;
using Nrk.FluentCore.Environment;
using System;
using System.IO;
using System.Text.Json;

namespace SkyLauncher.Core.Models;

public class LauncherSettings
{
    private static readonly string ConfigFolder =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            "SkyLauncher");

    private static readonly string ConfigPath =
        Path.Combine(ConfigFolder, "settings.json");

    // ===== Java =====

    public string JavaExecutablePath { get; set; } = string.Empty;

    public int MinMemoryMB { get; set; } = 1024;

    public int MaxMemoryMB { get; set; } = (int)((MemoryUtils.GetWindowsMetrics().Free)*0.8);

    // ===== User =====

    public string PlayerName { get; set; } = "Steve";

    // ===== Minecraft =====

    public string DefaultMinecraftFolder { get; set; } =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            ".minecraft");

    // ===== UI =====

    public bool AutoScanInstances { get; set; } = true;

    public bool ShowSnapshots { get; set; } = true;

    // ===== Load / Save =====

    public static LauncherSettings Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);

                return JsonSerializer.Deserialize<LauncherSettings>(json)
                    ?? new LauncherSettings();
            }
        }
        catch
        {
            HandyControl.Controls.MessageBox.Show(
                "无法加载设置，缺省的设置将会被使用\n遇到此问题请联系zydrawer",
                "错误",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error
                );
        }

        return new LauncherSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(ConfigFolder);

            var json = JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(ConfigPath, json);
        }
        catch
        {
        }
    }

    public string? LastSelectedInstanceJsonPath { get; set; }
}