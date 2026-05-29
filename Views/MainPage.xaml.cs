using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using SkyLauncher.ViewModels;
using System.Windows.Controls;
using System.Linq;

namespace SkyLauncher.Views;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = new MainPageViewModel();

    }

    
}