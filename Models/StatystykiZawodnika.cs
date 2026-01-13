using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class StatystykiZawodnika
    {
        public int StatZawodnikaID { get; set; }
        public int MeczID { get; set; }
        public int ZawodnikID { get; set; }
        public int TrenerID { get; set; }
        public int Gole { get; set; }
        public int Strzaly { get; set; }
        public int StrzalyCelne { get; set; }
        public int StrzalyNiecelne { get; set; }
        public int PodaniaCelne { get; set; }
        public int Faule { get; set; }
        public int ZolteKartki { get; set; }
        public int CzerwoneKartki { get; set; }
        public int CzysteKonta { get; set; }
    }
}
