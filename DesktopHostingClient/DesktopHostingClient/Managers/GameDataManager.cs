using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopHostingClient.Model;
using System.Threading;

namespace DesktopHostingClient.Managers;

public class GameDataManager
{
    public bool HasGameData { get { return GameData is not null; } }
    private GameData? GameData { get; set; }
    private static GameDataManager _instance;
    public event Action<int> OnBalanceChanged;
    private Thread incrementBalanceThread;

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

    public void StartBalanceUpdateThread()
    {
        incrementBalanceThread = new Thread(() =>
        {
            while (true)
            {
                SetBalance(GetBalance() + 1);
                NotifyBalanceChanged();
                Thread.Sleep(1000);
            }
        });
    }

    public void NotifyBalanceChanged()
    {
        OnBalanceChanged.Invoke(GameData.Balance);
    }

    public void CreateGameData()
    {
        GameData = new GameData();
    }

    public int GetBalance()
    {
        lock (GameData)
        {
            return GameData.Balance;
        }

    }

    private void SetBalance(int newBalance)
    {
        lock (GameData)
        {
            GameData.Balance = newBalance;
        }

    }

}
