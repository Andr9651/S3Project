using ModelLibrary.Model;

namespace BackendAPI.DBModel;
public class ModelConverter
{
    public static GameData ToGameData(DBGameData dbGameData, List<DBGamePurchase> dbGamePurchases = null)
    {
        GameData gameData = new GameData()
        {
            Id = dbGameData.Id,
            Balance = dbGameData.Balance,
            Ip = dbGameData.HostIp,
            Purchases = new Dictionary<int, int>()
        };

        if (dbGamePurchases is not null)
        {
            foreach (DBGamePurchase purchase in dbGamePurchases)
            {
                gameData.Purchases[purchase.PurchasableId] = purchase.Amount;
            }
        }

        return gameData;
    }

    public static DBGameData ToDBGameData(GameData gameData)
    {
        DBGameData dbGameData = new DBGameData()
        {
            Id = gameData.Id,
            Balance = gameData.Balance,
            HostIp = gameData.Ip,
        };

        return dbGameData;
    }

    public static List<DBGamePurchase> ToDBGamePurchases(GameData gameData)
    {
        List<DBGamePurchase> dbGamePurchases = new List<DBGamePurchase>();

        foreach (KeyValuePair<int, int> purchasableIdAmount in gameData.Purchases)
        {
            DBGamePurchase dbGamePurchase = new DBGamePurchase()
            {
                GameDataId = gameData.Id,
                PurchasableId = purchasableIdAmount.Key,
                Amount = purchasableIdAmount.Value
            };

            dbGamePurchases.Add(dbGamePurchase);
        }

        return dbGamePurchases;
    }
}
