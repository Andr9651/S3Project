namespace BackendAPI.Model.Dto;

public class GameInstance
{
    public int Id { get; set; }
    public int Balance { get; set; }
    public string Ip { get; set; }

    public GameInstance(GameInstanceDto gameInstanceDto,List<GamePurchaseDto>  gamePurchaseDtos)
    {
        Id = gameInstanceDto.Id;
        Balance = gameInstanceDto.Balance;
        Ip = gameInstanceDto.Ip;    
    }
}
