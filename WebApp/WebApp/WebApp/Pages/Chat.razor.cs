using Microsoft.AspNetCore.Components;
using WebApp.Managers;

namespace WebApp.Pages
{
    public partial class Chat : ComponentBase
    {
        [Parameter]
        public GameManager GameManager { get; init; }



    }
}
