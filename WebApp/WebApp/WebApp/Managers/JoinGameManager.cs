using System.Text.RegularExpressions;

namespace WebApp.Managers;

public class JoinGameManager
{

    public static bool ValidateIP(string ip)
    {
        Regex regex = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$");

        return regex.IsMatch(ip);
    }

    public static string CreateJoinGameURI(string ip)
    {
        return $"/Game/{ip}"; 
    }

}


