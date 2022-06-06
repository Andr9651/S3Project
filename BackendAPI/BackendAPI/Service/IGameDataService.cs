using BackendAPI.DBModel;
using ModelLibrary.Model;

namespace BackendAPI.Service
{
    public interface IGameDataService
    {
        GameData CreateGameData();
        GameData GetGameData(int id);
        Dictionary<int, Purchasable> GetPurchasables();
        bool SaveGameData(GameData gameData);
    }
}