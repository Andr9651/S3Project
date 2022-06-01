namespace BackendAPI.DBModel;

// Matches the GamePurchase table in the database.
public class DBGamePurchase
{
    public int GameDataId { get; set; }
    public int PurchasableId { get; set; }
    public int Amount { get; set; }
}
