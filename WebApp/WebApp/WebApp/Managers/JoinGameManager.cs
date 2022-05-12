using System.Text.RegularExpressions;

namespace WebApp.Managers;

public class JoinGameManager
{

    public static bool ValidateIP(string ip)
    {
        bool isValidIp = false;

        Regex regex = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}:\\d{1,5}$");
        Match match = regex.Match(ip);

        isValidIp = match.Success;

        if (isValidIp)
        {
            string[] splits = ip.Split('.', ':');

            for (int i = 0; i < 4; i++)
            {
                bool valid = int.Parse(splits[i]) <= 255;

                if (!valid)
                {
                    isValidIp = false;
                }
            }

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


