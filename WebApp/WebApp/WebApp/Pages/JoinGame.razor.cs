using Microsoft.AspNetCore.Components;
using WebApp.Managers;
namespace WebApp.Pages;

public partial class JoinGame : ComponentBase
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    private string ip;
    private string error;
    private bool _useValidation = true;

    public void ButtonJoinGame()
    {

        if (!_useValidation || JoinGameManager.ValidateIP(ip))
        {
            string gameURI = JoinGameManager.CreateJoinGameURI(ip);

            NavManager.NavigateTo(gameURI);
        }
        else
        {
            error = " Incorrect IP";
            StateHasChanged();
        }
    }
    public void ButtonJoinLocalGame()
    {
        NavManager.NavigateTo("/game/localhost:5100");
    }
}