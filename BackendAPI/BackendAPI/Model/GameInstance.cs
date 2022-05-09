namespace BackendAPI.Model.DTO;

public class GameInstance
{
    public int Id { get; set; }
    public int Balance { get; set; }
    public string HostIp { get; set; }
    public Dictionary<int, int> Purchases { get; set; }


    public GameInstance()
    {

    }

    public GameInstance(GameInstanceDto gameInstanceDto, List<GamePurchaseDto> gamePurchaseDtos = null)
    {
        Id = gameInstanceDto.Id;
        Balance = gameInstanceDto.Balance;
        HostIp = gameInstanceDto.HostIp;
        Purchases = new Dictionary<int, int>();

        if (gamePurchaseDtos is not null)
        {
            foreach (GamePurchaseDto purchase in gamePurchaseDtos)
            {
                Purchases[purchase.PurchasableId] = purchase.Amount;
            }
        }
    }

    public GameInstanceDto GetGameInstanceDto()
    {
        GameInstanceDto gameInstanceDto = new GameInstanceDto()
        {
            Id = this.Id,
            Balance = this.Balance,
            HostIp = this.HostIp,
        };

        return gameInstanceDto;
    }

    public List<GamePurchaseDto> GetGamePurchaseDtos()
    {
        List<GamePurchaseDto> gamePurchaseDtos = new List<GamePurchaseDto>();

        foreach (KeyValuePair<int, int> purchasableIdAmount in Purchases)
        {
            GamePurchaseDto gamePurchaseDto = new GamePurchaseDto()
            {
                GameInstanceId = this.Id,
                PurchasableId = purchasableIdAmount.Key,
                Amount = purchasableIdAmount.Value
            };

            gamePurchaseDtos.Add(gamePurchaseDto);
        }

        return gamePurchaseDtos;
    }
}
