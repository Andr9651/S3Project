﻿using Microsoft.AspNetCore.SignalR.Client;
using static WebApp.Model.Purchasables;

namespace WebApp.Managers;
public class GameManager
{
    private HubConnection _connection;
    public event Action PongEvent;
    public event Action<int> BalanceUpdateEvent;
    public event Action<List<Purchasable>> ReceivePurchasablesEvent;
    public event Action<Dictionary<int, int>> ReceivePurchasesEvent;
    public event Action<int,int> ReceivePurchaseUpdateEvent;

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
        ReceivePurchaseUpdateEvent(purchaseId, amount);
    }

    private void ReceivePurchases(Dictionary<int, int> purchases)
    {
        ReceivePurchasesEvent(purchases);
    }

    public async Task<bool> TryBuyPurchasable(int purchasableId)
    {
        return await _connection.InvokeAsync<bool>("TryBuyPurchasable", purchasableId);
    }
    
    private void BalanceUpdate(int balance)
    {
        BalanceUpdateEvent(balance);
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

    private void Pong()
    {
        PongEvent.Invoke();
    }
    private void ReceivePurchasables(List<Purchasable> purchasables)
    {
        ReceivePurchasablesEvent(purchasables);
    }
}
