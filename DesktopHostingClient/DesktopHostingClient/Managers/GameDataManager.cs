using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopHostingClient.Model;

namespace DesktopHostingClient.Managers;

public class GameDataManager
{
    public GameData? GameData { get; private set; }
    private static GameDataManager _instance;

    private GameDataManager()
    {

    }

    public static GameDataManager GetInstance()
    {
        if (_instance is null)
        {
            _instance = new GameDataManager();
        }

        return _instance;
    }

    public void CreateGameData()
    {
        GameData = new GameData();
    }

}
