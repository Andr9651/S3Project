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
}
