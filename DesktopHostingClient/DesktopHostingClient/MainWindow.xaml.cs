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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopHostingClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public HostingController HostingController { get; set; }
    public GameDataController GameDataController { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        GameDataController = new GameDataController();
        HostingController = new HostingController();
    }

    private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
    {
        GameDataController.CreateGameData();
    }
}