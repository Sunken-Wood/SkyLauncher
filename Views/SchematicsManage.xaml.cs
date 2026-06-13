using SkyLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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


namespace SkyLauncher.Views
{
    /// <summary>
    /// ShaderPackManage.xaml 的交互逻辑
    /// </summary>
    public partial class SchematicsManage : UserControl
    {
        //private readonly string _shaderPacksDir;
        public ObservableCollection<ResourcePackItem> DataList { get; } = new();
        public SchematicsManage()
        {
            InitializeComponent();
            LoadShaderPacks();
            this.DataContext = this;
        }
        private string SchematicsFilesDir
        {
            get
            {
                var instance = MainViewModel.Instance.SelectedInstance;
                return instance != null
                    ? System.IO.Path.Combine(instance.GameDirectory, "schematics")
                    : string.Empty;
            }
        }
        public void OpenSchematicsFilesFolder(object sender, RoutedEventArgs e)
        {
            var instance = MainViewModel.Instance.SelectedInstance;
            if (instance == null)
            {
                MessageBox.Show("没有选中的实例", "错误");
                return;
            }
            var shaderPacksDir = System.IO.Path.Combine(instance.GameDirectory, "schematics");
            if (!System.IO.Directory.Exists(shaderPacksDir))
            {
                System.IO.Directory.CreateDirectory(shaderPacksDir);
            }
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = shaderPacksDir,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开文件夹: {ex.Message}", "错误");
            }
        }
        public void LoadShaderPacks()
        {
            DataList.Clear();

            if (!Directory.Exists(SchematicsFilesDir))
                Directory.CreateDirectory(SchematicsFilesDir);

            // 读取已启用的资源包

            // 遍历所有 zip
            foreach (var file in Directory.GetFiles(SchematicsFilesDir, "*.litematic"))
            {
                DataList.Add(new ResourcePackItem
                {
                    FileName = System.IO.Path.GetFileName(file),

                });
            }
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.NavigateToPage(() => new Views.ConfigPage());
            }
        }
    }
}
