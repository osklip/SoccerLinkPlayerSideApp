using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class MeczDostepnosc
    {
        public int DostepnoscID { get; set; }
        public int MeczID { get; set; }
        public int ZawodnikID { get; set; }
        public int Status { get; set; }
        public DateTime DataZgloszenia { get; set; }
    }
}
