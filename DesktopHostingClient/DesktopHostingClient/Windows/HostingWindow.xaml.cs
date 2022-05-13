using DesktopHostingClient.Managers;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopHostingClient.Windows;
public partial class HostingWindow : Window
{
    public HostingManager HostingManager { get; set; }
    public GameManager GameManager { get; set; }
    private int? _loadGameId { get; set; }
    public HostingWindow(int? loadGameId = null)
    {
        InitializeComponent();
        GameManager = GameManager.GetInstance();
        HostingManager = new HostingManager();
        _loadGameId = loadGameId;
    }

    private async void OnLoad(object sender, RoutedEventArgs e)
    {
        LabelIpAddress.Content = await HostingManager.GetPublicIp();

        HostingManager.SetupSignalRHost();

        await HostingManager.StartHosting();

        await GameManager.SetupGame(_loadGameId);

        GameId.Content = GameManager.GetGameId();

        LabelPort.Content = HostingManager.Port;
    }

    private void OnClose(object sender, EventArgs e)
    {
        GameManager.ShutdownGame();
        HostingManager.DisposeHost();
    }

    private void Stop_Game_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();

        this.Close();
    }

    private async Task<bool> AskToSave()
    {
        bool savedSuccessfully = false;

        MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save before closing?", "Save Game?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

        if (messageBoxResult == MessageBoxResult.Yes)
        {
           savedSuccessfully = await GameManager.SaveGame();
        }

        return savedSuccessfully;
    }

    private async void Save_Game_Click(object sender, RoutedEventArgs e)
    {
        bool result = await GameManager.SaveGame();

        if (result)
        {
            MessageBox.Show($"Congratulations: The Game was successfully saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        else
        {
            MessageBox.Show($"ERROR: The Game failed to save.{Environment.NewLine}{Environment.NewLine}Please try again.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
