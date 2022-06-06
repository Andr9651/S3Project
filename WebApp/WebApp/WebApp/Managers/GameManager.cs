using Microsoft.AspNetCore.SignalR.Client;
using ModelLibrary.Model;

namespace WebApp.Managers;
public class GameManager
{
    public int IncomePerSecond { get => CalculateIncomePerSecond(); }

    public event Action OnPong;
    public event Action OnStateChanged;

    private HubConnection _connection;
    private GameData _gameData;
    private Dictionary<int, Purchasable> _purchasables;


    public GameManager()
    {
        _gameData = new GameData();
        _purchasables = new Dictionary<int, Purchasable>();
    }

    public async Task ConnectToGame(string ip)
    {
        if (_connection is not null)
        {
            await CloseConnection();
        }

        // Configure HubConnectionBuilder
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("https://" + ip + "/GameHub");
        builder.WithAutomaticReconnect();
        _connection = builder.Build();

        // Map SignalR RPC Methods
        // The "On" method's type parameters specifies what parameters the RPC contains
        _connection.On("Pong", Pong);
        _connection.On<int>("BalanceUpdate", BalanceUpdate);
        _connection.On<int, int>("PurchaseUpdate", ReceivePurchaseUpdate);
        _connection.On<Dictionary<int, int>>("ReceivePurchases", ReceivePurchases);
        _connection.On<Dictionary<int, Purchasable>>("ReceivePurchasables", ReceivePurchasables);

        await _connection.StartAsync();
    }

    private int CalculateIncomePerSecond()
    {
        int incomePerSecond = 1;

        if (_gameData.Purchases is not null)
        {
            foreach (KeyValuePair<int, int> purchases in _gameData.Purchases)
            {
                incomePerSecond += _purchasables[purchases.Key].Income * purchases.Value;
            }
        }

        return incomePerSecond;
    }

    private void Pong()
    {
        OnPong.Invoke();
    }

    private void BalanceUpdate(int balance)
    {
        _gameData.Balance = balance;

        OnStateChanged?.Invoke();
    }

    private void ReceivePurchaseUpdate(int purchaseId, int amount)
    {
        _gameData.Purchases[purchaseId] = amount;
        OnStateChanged?.Invoke();
    }

    private void ReceivePurchases(Dictionary<int, int> purchases)
    {
        _gameData.Purchases = purchases;
        OnStateChanged?.Invoke();
    }

    private void ReceivePurchasables(Dictionary<int, Purchasable> purchasables)
    {
        _purchasables = purchasables;
        OnStateChanged?.Invoke();
    }

    public async Task CloseConnection()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    public void PingServer()
    {
        _connection.InvokeAsync("Ping");
    }

    public async Task<bool> TryBuyPurchasable(int purchasableId)
    {
        return await _connection.InvokeAsync<bool>("TryBuyPurchasable", purchasableId);
    }

    public async Task<int> BuyMaxPurchasable(int purchasableId)
    {
        return await _connection.InvokeAsync<int>("BuyMaxPurchasable", purchasableId);
    }

    public bool CanBuyPurchasable(int purchasableId)
    {
        bool canPurchase = false;

        if (_purchasables.ContainsKey(purchasableId))
        {
            Purchasable purchasable = _purchasables[purchasableId];
            canPurchase = purchasable.Price <= _gameData.Balance;
        }

        return canPurchase;
    }

    public int GetBalance()
    {
        return _gameData.Balance;
    }

    public ICollection<Purchasable> GetPurchasables()
    {
        return _purchasables.Values;
    }

    public int GetPurchaseAmount(int purchasableId)
    {
        int amount = 0;

        if (_gameData.Purchases.ContainsKey(purchasableId))
        {
            amount = _gameData.Purchases[purchasableId];
        }

        return amount;
    }

    public int GetPurchasableIncome(int purchasableId)
    {
        int income = 0;

        if (_purchasables.ContainsKey(purchasableId))
        {
            income = _purchasables[purchasableId].Income;
        }

        return income;
    }
    public int GetTotalPurchasableIncome(int purchasableId)
    {
        int income = 0;

        if (_gameData.Purchases.ContainsKey(purchasableId))
        {
            income = _purchasables[purchasableId].Income * _gameData.Purchases[purchasableId];
        }

        return income;
    }
}