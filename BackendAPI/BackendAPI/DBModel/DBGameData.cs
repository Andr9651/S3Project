namespace BackendAPI.DBModel;

// Matches the GameData table in the database.
public class DBGameData
{
    public int Id { get; set; }
    public int Balance { get; set; }
    public string HostIp { get; set; }
}
