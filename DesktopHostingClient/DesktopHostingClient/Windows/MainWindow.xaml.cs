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
using DesktopHostingClient.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DesktopHostingClient.Windows;

namespace DesktopHostingClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
    {
        HostingWindow hostingWindow = new HostingWindow();
        hostingWindow.Show();

        this.Close();
    }

    private void Load_Game_Click(object sender, RoutedEventArgs e)
    {
        string loadGameIdText = LoadGameTextBox.Text;

        // "out" passes the parameter as a reference into the function
        // and allows variable to keep changes made inside the function
        if (int.TryParse(loadGameIdText, out int loadGameId))
        {
            HostingWindow hostingWindow = new HostingWindow(loadGameId);
            hostingWindow.Show();

            this.Close();
        } else
        {
            MessageBox.Show($"{loadGameIdText} is not a valid id", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
