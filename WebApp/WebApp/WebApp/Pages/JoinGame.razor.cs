using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using WebApp.Managers;
namespace WebApp.Pages;

public partial class JoinGame : ComponentBase
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    private string ip;
    private string error;

    public void ButtonJoinGame()
    {
        if (JoinGameManager.ValidateIP(ip))
        {
            string gameURI = JoinGameManager.CreateJoinGameURI(ip);

            NavManager.NavigateTo(gameURI);
        }
        else
        {
            error = " Incorrect IP";
        }
    }
    public void ButtonJoinLocalGame()
    {
        NavManager.NavigateTo("/game/localhost:5100");
    }
}