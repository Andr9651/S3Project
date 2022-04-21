using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopHostingClient.Model;

namespace DesktopHostingClient.Controller;

public class GameDataController
{
    public GameData? GameData { get; private set; }
    public void CreateGameData()
    {
        GameData = new GameData();
    }
    
}
