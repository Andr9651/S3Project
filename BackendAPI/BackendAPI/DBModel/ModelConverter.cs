﻿using ModelLibrary.Model;

namespace BackendAPI.DBModel;
public class ModelConverter
{
    public static GameData ToGameData(DBGameData dbGameData, List<DBGamePurchase> dbGamePurchases = null)
    {
        GameData gameData = new GameData()
        {
            Id = dbGameData.Id,
            Balance = dbGameData.Balance,
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
            Balance = gameData.Balance
        };

        return dbGameData;
    }

    public static List<DBGamePurchase> ToDBGamePurchases(GameData gameData)
    {
        List<DBGamePurchase> dbGamePurchases = new List<DBGamePurchase>();

        // gameData.Purchases is a dictionary and contains
        // KeyValuePairs<Purchasable.id, amount>
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

    public static Purchasable ToPurchasable(DBPurchasable dBPurchasable)
    {
        Purchasable purchasable = new Purchasable()
        {
            Id = dBPurchasable.Id,
            Name = dBPurchasable.Name,
            Price = dBPurchasable.Price,
            Income = dBPurchasable.Income
        };

        return purchasable;
    }
}
