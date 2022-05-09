namespace BackendAPI.Model.DTO;

public class GameInstance
{
    public int Id { get; set; }
    public int Balance { get; set; }
    public string Ip { get; set; }
    public Dictionary<int, int> Purchases { get; set; }

    public GameInstance(GameInstanceDto gameInstanceDto, List<GamePurchaseDto> gamePurchaseDtos)
    {
        Id = gameInstanceDto.Id;
        Balance = gameInstanceDto.Balance;
        Ip = gameInstanceDto.Ip;
        Purchases = new Dictionary<int, int>();

        foreach (GamePurchaseDto purchase in gamePurchaseDtos)
        {
            Purchases[purchase.PurchasableId] = purchase.Amount;
        }
    }
}
