using System.Text.RegularExpressions;

namespace WebApp.Managers;
public class JoinGameManager
{
    public static bool ValidateIP(string ip)
    {
        bool isValidIp = false;

        if (string.IsNullOrEmpty(ip))
        {
            return false;
        }

        // Checks if the string is a valid ip
        // that follows this format (ddd.ddd.ddd.ddd:ddddd) where "d" is a digit.
        // This also stops negative numbers
        Regex regex = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$");
        Match match = regex.Match(ip);

        isValidIp = match.Success;

        if (isValidIp)
        {
            // Splits the ip into substrings on "." and ":".
            string[] splits = ip.Split('.', ':');

            // Checks if the ip's numbers are 255 or under.
            for (int i = 0; i < 4; i++)
            {
                bool valid = int.Parse(splits[i]) <= 255;

                if (!valid)
                {
                    isValidIp = false;
                }
            }

            // Checks if the port is less than 65535.
            if (int.Parse(splits[4]) > 65535)
            {
                isValidIp = false;
            }
        }

        return isValidIp;
    }

    public static string CreateJoinGameURI(string ip)
    {
        return $"/Game/{ip}";
    }
}