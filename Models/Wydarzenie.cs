using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class Wydarzenie
    {
        public int WydarzenieID { get; set; }
        public string Nazwa { get; set; }
        public string Miejsce { get; set; }
        public DateTime Data { get; set; }
        public DateTime GodzinaStart { get; set; }
        public DateTime GodzinaKoniec { get; set; }  
        public string Opis { get; set; }
        public int TrenerID { get; set; }
    }
}
