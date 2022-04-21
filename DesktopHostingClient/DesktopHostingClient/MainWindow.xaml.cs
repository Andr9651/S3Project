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
using DesktopHostingClient.Controller;

namespace DesktopHostingClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public GameDataController gameDataController { get; set; }
    
    public MainWindow()
    {
        InitializeComponent();
        gameDataController = new GameDataController();
    }
    // ¯\_(ツ)_/¯
    public void ButtonNewGame_Click(object sender, RoutedEventArgs e)
    {
        gameDataController.CreateGameData();
    }
}
