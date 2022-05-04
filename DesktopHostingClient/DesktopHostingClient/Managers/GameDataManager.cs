using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopHostingClient.Model;
using System.Threading;
using DesktopHostingClient.Service;

namespace DesktopHostingClient.Managers;

public class GameDataManager
{
    public bool HasGameData { get { return GameData is not null; } }
    private GameData? GameData { get; set; }
    private static GameDataManager _instance;
    public event Action<int> OnBalanceChanged;
    private Thread incrementBalanceThread;
    private volatile bool _isRunning;
    public Dictionary<int,Purchasable> Purchasables { get; private set; }
    public bool IsUpdateThreadRunning
    {
        get { return _isRunning; }
        private set { _isRunning = value; }
    }

   
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
        IsUpdateThreadRunning = true;
        incrementBalanceThread = new Thread(() =>
        {
            while (IsUpdateThreadRunning)
            {
                SetBalance(GetBalance() + 1);
                NotifyBalanceChanged();
                Thread.Sleep(1000);
            }
        });
        incrementBalanceThread.Start();
    }
    public void StopBalanceUpdateThread()
    {
        IsUpdateThreadRunning = false;
        incrementBalanceThread?.Join();
    }

    public void NotifyBalanceChanged()
    {
        OnBalanceChanged?.Invoke(GameData.Balance);
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
    public async Task HostingStartUp()
    {
        PurchasableService purchasableService = new PurchasableService();
        List<Purchasable> purchasables  = await purchasableService.GetPurchasables();

        Purchasables = purchasables.ToDictionary(
            keySelector: purchasable => purchasable.Id,
            elementSelector: purchasable => purchasable);

        CreateGameData();

    }
}
