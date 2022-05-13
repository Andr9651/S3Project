using Microsoft.AspNetCore.SignalR.Client;
using ModelLibrary.Model;

namespace WebApp.Managers;
public class GameManager
{
    private HubConnection _connection;
    public event Action PongEvent;

    private GameData _gameData;
    private List<Purchasable> _purchasables;

    public event Action StateHasChangedEvent;

    public GameManager()
    {
        _gameData = new GameData();
        _purchasables = new List<Purchasable>();
    }
    
    public async Task ConnectToGame(string ip)
    {
        if (_connection is not null)
        {
            await CloseConnection();
        }

        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://" + ip + "/GameHub");
        builder.WithAutomaticReconnect();
        _connection = builder.Build();

        _connection.On("Pong", Pong);
        _connection.On<int>("BalanceUpdate", BalanceUpdate);
        _connection.On<List<Purchasable>>("ReceivePurchasables", ReceivePurchasables);
        _connection.On<Dictionary<int, int>>("ReceivePurchases", ReceivePurchases);
        _connection.On<int, int>("PurchaseUpdate", ReceivePurchaseUpdate);
        
        await _connection.StartAsync();
    }

    private void ReceivePurchaseUpdate(int purchaseId, int amount)
    {
        _gameData.Purchases[purchaseId] = amount;
        StateHasChangedEvent?.Invoke();
    }

    private void ReceivePurchases(Dictionary<int, int> purchases)
    {
        _gameData.Purchases = purchases;
        StateHasChangedEvent?.Invoke();
    }

    private void ReceivePurchasables(List<Purchasable> purchasables)
    {
        _purchasables = purchasables.ToList();
        StateHasChangedEvent?.Invoke();
    }
    
    private void BalanceUpdate(int balance)
    {
        _gameData.Balance = balance;

        StateHasChangedEvent?.Invoke();
    }

    private void Pong()
    {
        PongEvent.Invoke();
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

    public bool CanBuyPurchasable(int purchasableId)
    {
        Purchasable purchasable = _purchasables.Find(p => p.Id == purchasableId);

        return purchasable.Price <= _gameData.Balance;
    }
    
    public int GetBalance()
    {
        return _gameData.Balance;
    }

    public IReadOnlyCollection<Purchasable> GetPurchasables()
    {
        return _purchasables;
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
}