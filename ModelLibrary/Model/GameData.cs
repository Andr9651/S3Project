using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Model;
public class GameData
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public Dictionary<int, int> Purchases { get; set; }
    public int Balance { get; set; }

    public GameData()
    {
        Purchases = new Dictionary<int, int>();
        Balance = 0;
    }
}

