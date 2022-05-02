using Microsoft.AspNetCore.SignalR.Client;
 
namespace WebApp.Managers;
public class GameManager
{
    private HubConnection _connection;
    public event Action PongEvent;
    public event Action<int> BalanceUpdateEvent;

    public async Task ConnectToGame(string ip)
    {
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://" + ip + "/GameHub");
        _connection = builder.Build();
        _connection.On("Pong", Pong);
        _connection.On<int>("BalanceUpdate", BalanceUpdate);
        await _connection.StartAsync();
        
    }

    private void BalanceUpdate(int balance)
    {
        BalanceUpdateEvent(balance);
    }

    public void CloseConnection()
    {
        _connection.StopAsync().Wait();
    }

    public void PingServer()
    {
        _connection.InvokeAsync("Ping");
    }

    private void Pong()
    {
        PongEvent.Invoke();
    }

}
