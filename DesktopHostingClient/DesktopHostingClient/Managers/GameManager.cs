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
    /// <value>
    /// True if a GameData instance is present
    /// </value>
    public bool HasGameData
    {
        get { return (GameData is not null); }
    }

    /// <summary>
    /// Dictionary of available purchasable objects
    /// <br/>
    /// Key: Purchasable.Id
    /// <br/>
    /// Value: Purchasable
    /// </summary>
    public Dictionary<int, Purchasable> Purchasables { get; set; }

    public bool IsUpdateThreadRunning
    {
        get { return _isRunning; }
        private set { _isRunning = value; }
    }

    /// <summary>
    /// This event is triggered when the GameDataBalance is updated
    /// <br/>
    /// Returns the updated balance when invoked
    /// </summary>
    public event Action<int> OnBalanceChanged;
    public event Action<int, int> OnPurchase;
    private GameData? GameData { get; set; }
    private static GameManager _instance;
    private Thread incrementBalanceThread;
    private volatile bool _isRunning;

    private GameManager()
    {
        Purchasables = new Dictionary<int, Purchasable>();

    }

    /// <summary>
    /// Creates a GameManager if it has not been instatiated
    /// </summary>
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
        Purchasables = await purchasableService.GetPurchasables();
        GameDataService gameDataService = new GameDataService();

        if (loadedGameId is null)
        {
            GameData = await gameDataService.CreateGameData();
        }
        else
        {
            GameData = await gameDataService.LoadGameData(loadedGameId.Value);
        }

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

        //This fixes Race condition
        lock (GameData)
        {
            if (Purchasables.ContainsKey(purchasableId))
            {
                Purchasable purchasable = Purchasables[purchasableId];

                int balance = GetBalance();
                if (balance >= purchasable.Price)
                {
                    SetBalance(balance - purchasable.Price);
                    BuyPurchasable(purchasableId);
                    isSuccess = true;
                }
            }
        }

        return isSuccess;

    }

    public IReadOnlyDictionary<int, int> GetPurchases()
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

    private void BuyPurchasable(int purchasableId)
    {
        int newPurchasedAmount = GetPurchasedAmount(purchasableId) + 1;
        GameData.Purchases[purchasableId] = newPurchasedAmount;

        if (OnPurchase is not null)
        {
            OnPurchase(purchasableId, newPurchasedAmount);
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
        catch (Exception ex)
        {

        }

        return isSuccess;
    }
}
