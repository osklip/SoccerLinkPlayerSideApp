using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class Trening
    {
        public int TreningID { get; set; }
        public string Typ { get; set; }
        public int ListaObecnosciID { get; set; }

        public DateTime Data { get; set; }
        public DateTime GodzinaRozpoczecia { get; set; }
        public DateTime GodzinaZakonczenia { get; set; }
        public string Miejsce { get; set; }
        public int TrenerID { get; set; }
    }
}
