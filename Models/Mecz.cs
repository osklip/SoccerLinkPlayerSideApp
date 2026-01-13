using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class Mecz
    {
        public int MeczID { get; set; }
        public int SkladMeczowyID { get; set; }
        public string Przeciwnik { get; set; }
        public DateTime Data { get; set; }
        public DateTime Godzina { get; set; }
        public string Miejsce { get; set; }
        public int TrenerID { get; set; }
    }
}
