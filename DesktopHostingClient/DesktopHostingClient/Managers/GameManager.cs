using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using System.Threading;
using DesktopHostingClient.Service;

namespace DesktopHostingClient.Managers;

/// <summary>
/// The <c>GameManager</c> manages the gamestate.
/// <br/>
/// The Class is a singleton and instances must be aquired through GetInstance()
/// <br/>
/// This includes loading and saving GameData and managing game update loops.
/// </summary>
public class GameManager
{
    /// <value> True if a GameData instance is present </value>
    public bool HasGameData
    {
        get { return (GameData is not null); }
    }

    // Volatile was not needed since the field is only modified by the main thread.
    public bool IsUpdateThreadRunning { get; private set; }

    public int IncomePerSecond { get => CalculateIncomePerSecond(); }


    /// <summary> Key: Purchasable.Id <br/> Value: Purchasable </summary>
    public Dictionary<int, Purchasable> Purchasables { get; set; }

    /// <summary> Sends balance as parameter </summary>
    public event Action<int> OnBalanceChanged;

    /// <summary> Sends Purchasable.Id and bought amount as parameters </summary>
    public event Action<int, int> OnPurchase;
    private GameData GameData { get; set; }
    private Thread incrementBalanceThread;
    private static GameManager _instance;

    private GameManager()
    {
        Purchasables = new Dictionary<int, Purchasable>();
    }

    /// <returns> The only GameManager instance</returns>
    public static GameManager GetInstance()
    {
        if (_instance is null)
        {
            _instance = new GameManager();
        }

        return _instance;
    }

    public void StartBalanceUpdateThread()
    {
        if (IsUpdateThreadRunning)
        {
            throw new InvalidOperationException("UpdateBalanceThread is already Running");
        }

        IsUpdateThreadRunning = true;

        incrementBalanceThread = new Thread(() =>
        {
            while (IsUpdateThreadRunning)
            {
                lock (GameData)
                {
                    SetBalance(GetBalance() + IncomePerSecond);
                }

                NotifyBalanceChanged();
                Thread.Sleep(1000);
            }
        });

        incrementBalanceThread.Start();
    }

    private int CalculateIncomePerSecond()
    {
        int incomePerSecond = 1;

        if (GameData.Purchases is not null)
        {
            foreach (KeyValuePair<int, int> purchases in GameData.Purchases)
            {
                incomePerSecond += Purchasables[purchases.Key].Income * purchases.Value;
            }
        }

        return incomePerSecond;
    }

    public void StopBalanceUpdateThread()
    {
        IsUpdateThreadRunning = false;

        // Waits for the increment balance thread to finish
        incrementBalanceThread?.Join();
    }

    public void NotifyBalanceChanged()
    {
        OnBalanceChanged?.Invoke(GameData.Balance);
    }

    public void CreateGameData(GameData? loadedData = null)
    {
        if (loadedData == null)
        {
            GameData = new GameData();
        }
        else
        {
            GameData = loadedData;
        }
    }

    public async Task SetupGame(int? loadedGameId = null)
    {
        PurchasableService purchasableService = new PurchasableService();
        GameDataService gameDataService = new GameDataService();

        Task<Dictionary<int, Purchasable>> purchasablesTask = purchasableService.GetPurchasables();

        Task<GameData> gameDataTask;
        if (loadedGameId is null)
        {
            gameDataTask = gameDataService.CreateGameData();
        }
        else
        {
            gameDataTask = gameDataService.LoadGameData(loadedGameId.Value);
        }

        Purchasables = await purchasablesTask;
        GameData = await gameDataTask;

        StartBalanceUpdateThread();
    }

    public void ShutdownGame()
    {
        StopBalanceUpdateThread();
    }

    public int GetBalance()
    {
        return GameData.Balance;
    }

    private void SetBalance(int newBalance)
    {
        GameData.Balance = newBalance;
        if (OnBalanceChanged is not null)
        {
            OnBalanceChanged(newBalance);
        }
    }

    public bool TryBuyPurchasable(int purchasableId)
    {
        bool isSuccess = false;

        if (Purchasables.ContainsKey(purchasableId))
        {
            Purchasable purchasable = Purchasables[purchasableId];

            // Locks gamedata from being used outside this scope
            lock (GameData)
            {
                // Placing GetBalance inside the lock ensures Repeatable Read
                int balance = GetBalance();
                if (balance >= purchasable.Price)
                {
                    BuyPurchasable(purchasable, 1);
                    isSuccess = true;
                }
            }
        }

        return isSuccess;
    }

    public int BuyMaxPurchasables(int purchasableId)
    {
        int purchasedAmount = 0;

        if (Purchasables.ContainsKey(purchasableId))
        {
            Purchasable purchasable = Purchasables[purchasableId];

            if (purchasable.Price > 0)
            {


                lock (GameData)
                {
                    int balance = GetBalance();

                    // Integer division always rounds down (truncates)
                    purchasedAmount = balance / purchasable.Price;
                    if (purchasedAmount > 0)
                    {
                        BuyPurchasable(purchasable, purchasedAmount);
                    }
                }
            }
        }
        return purchasedAmount;
    }

    private void BuyPurchasable(Purchasable purchasable, int amount)
    {
        SetBalance(GetBalance() - purchasable.Price);

        int newPurchasedAmount = GetPurchasedAmount(purchasable.Id) + amount;
        GameData.Purchases[purchasable.Id] = newPurchasedAmount;

        if (OnPurchase is not null)
        {
            OnPurchase(purchasable.Id, newPurchasedAmount);
        }
    }

    public Dictionary<int, int> GetPurchases()
    {
        return GameData.Purchases;
    }

    public int GetPurchasedAmount(int purchasableId)
    {
        if (GameData.Purchases.ContainsKey(purchasableId))
        {
            return GameData.Purchases[purchasableId];
        }
        else
        {
            return 0;
        }
    }

    public int GetGameId()
    {
        return GameData.Id;
    }

    public async Task<bool> SaveGame()
    {
        bool isSuccess = false;
        try
        {
            GameDataService gameDataService = new GameDataService();
            isSuccess = await gameDataService.SaveGameData(GameData);
        }
        catch (Exception)
        {

        }

        return isSuccess;
    }
}
