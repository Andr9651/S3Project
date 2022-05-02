using Microsoft.AspNetCore.SignalR.Client;
 
namespace WebApp.Managers;
public class GameManager
{
    private HubConnection _connection;
    public event Action PongEvent;

    public void ConnectToGame(string ip)
    {
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://" + ip + "/GameHub");
        _connection = builder.Build();
        _connection.On("Pong", Pong);
        _connection.StartAsync().Wait();
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
