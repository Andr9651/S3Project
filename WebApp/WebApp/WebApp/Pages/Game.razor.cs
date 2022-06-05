using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using WebApp.Managers;

namespace WebApp.Pages;

public partial class Game : ComponentBase
{
    [Parameter]
    public string Ip { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set;}

    private long pingTime = 0;
    private Stopwatch pingTimer;
    private bool _doDelay;
    private bool _showDebug;
    private int _delayLength = 5000;
    private GameManager gameManager = new GameManager();

    private void Ping()
    {
        gameManager.PingServer();
        pingTimer.Start();
    }

    private async void TryBuyPurchasable(int purchasableId)
    {
        if (gameManager.CanBuyPurchasable(purchasableId))
        {
            if (_doDelay)
            {
                await Task.Delay(_delayLength);
            }

            bool success = await gameManager.TryBuyPurchasable(purchasableId);

            if (!success)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Somebody bought something before you!");
            }
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("alert", "Insufficient funds!");
        }
    }

    protected override async void OnInitialized()
    {
        pingTimer = new Stopwatch();
        base.OnInitialized();

        // StateHasChanged updates the UI
        gameManager.OnStateChanged += StateHasChanged;

        gameManager.OnPong += () =>
        {
            pingTimer.Stop();
            pingTime = pingTimer.ElapsedMilliseconds;
            pingTimer.Reset();
            StateHasChanged();
        };

        await gameManager.ConnectToGame(Ip);
    }
}

