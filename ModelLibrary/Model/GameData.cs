using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Model;
public class GameData
{
    public int Id { get; init; }
    public Dictionary<int, int> Purchases { get; set; }
    public int Balance
    {
        get { return _balance; }
        set { _balance = value; }
    }
    private volatile int _balance;


    public GameData()
    {
        Purchases = new Dictionary<int, int>();
    }
}

