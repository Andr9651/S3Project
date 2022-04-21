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
    private static GameDataController _instance;

    private GameDataController()
    {

    }

    public static GameDataController GetInstance()
    {
        if (_instance is null)
        {
            _instance = new GameDataController();
        }

        return _instance;
    }

    public void CreateGameData()
    {
        GameData = new GameData();
    }

}
