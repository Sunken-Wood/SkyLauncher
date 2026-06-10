using SkyLauncher.ViewModels;
using System;
using System.Collections.Generic;
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
    /// ResourcePackManager.xaml 的交互逻辑
    /// </summary>
    public partial class ResourcePackManager : UserControl
    {
        public ResourcePackManager()
        {
            InitializeComponent();
            try
            {
                DataContext = new ResourcePackManagerViewModel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }
    }
}
