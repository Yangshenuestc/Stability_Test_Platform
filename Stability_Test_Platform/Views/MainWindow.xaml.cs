using Stability_Test_Platform.ViewModels;
using System.Windows;

namespace Stability_Test_Platform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 将 DataContext 设置为 MainViewModel
            this.DataContext = new MainViewModel();
        }
    }
}