using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopHostingClient.Model
{
    public class GameData
    {

        public int Id { get; set; }
       
        private  int _balance;

        public int Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        public string IP { get; set; }
        public Dictionary<int,int> Purchases { get; set; }


        public GameData()
        {
            Purchases = new Dictionary<int, int>();
            Balance = 0;    
        }
    }
}
