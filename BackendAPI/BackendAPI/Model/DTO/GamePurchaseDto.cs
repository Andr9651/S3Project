namespace BackendAPI.Model.DTO
{
    public class GamePurchaseDto
    {
        public int GameInstanceId { get; set; }
        public int PurchasableId { get; set; }
        public int Amount { get; set; }
    }
}
