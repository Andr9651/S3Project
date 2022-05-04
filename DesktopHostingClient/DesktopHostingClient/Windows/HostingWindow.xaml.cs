using DesktopHostingClient.Managers;
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
using System.Windows.Shapes;

namespace DesktopHostingClient.Windows;

public partial class HostingWindow : Window
{
    public HostingManager HostingManager { get; set; }
    public GameDataManager GameDataManager { get; set; }

    public HostingWindow()
    {
        InitializeComponent();
        GameDataManager = GameDataManager.GetInstance();
        HostingManager = new HostingManager();
    }

    private async void OnLoad(object sender, RoutedEventArgs e)
    {
        await GameDataManager.HostingStartUp();
        LabelIPAdress.Content = await HostingManager.GetPublicIP();
        HostingManager.SetupSignalRHost();
        await HostingManager.StartHosting();
        LabelPort.Content = HostingManager.Port;
       
    }

    private void OnClose(object sender, EventArgs e)
    {
        HostingManager.DisposeHost();
    }

    private void Stop_Game_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();  
        this.Close();
    }
}
