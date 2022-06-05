using DesktopHostingClient.Managers;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopHostingClient.Windows;
public partial class HostingWindow : Window
{
    private HostingManager HostingManager { get; set; }
    private GameManager GameManager { get; set; }
    private int? _loadGameId { get; set; }
    public HostingWindow(int? loadGameId = null)
    {
        InitializeComponent();

        _loadGameId = loadGameId;

        GameManager = GameManager.GetInstance();
        HostingManager = new HostingManager();
    }

    // Gets called when the window opens
    private async void OnLoad(object sender, RoutedEventArgs e)
    {
        Task<string> IpTask = HostingManager.GetPublicIp();

        // Start Game and Hosting
        HostingManager.SetupSignalRHost();
        await HostingManager.StartHosting();
        await GameManager.SetupGame(_loadGameId);

        // Set content of labels
        LabelIpAddress.Content = await IpTask;
        GameId.Content = GameManager.GetGameId();
        LabelPort.Content = HostingManager.Port;
    }

    // Gets called when the window closes
    private async void OnClose(object sender, EventArgs e)
    {
        GameManager.ShutdownGame();
        HostingManager.DisposeHost();
        await AskToSave();
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

        MessageBoxResult messageBoxResult = MessageBox.Show(
            "Do you want to save before closing?",
            "Save Game?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Exclamation
        );

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
            MessageBox.Show(
                $"Congratulations: The Game was successfully saved.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        else
        {
            MessageBox.Show(
                $"ERROR: The Game failed to save.{Environment.NewLine}{Environment.NewLine}Please try again.",
                "ERROR!",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}
