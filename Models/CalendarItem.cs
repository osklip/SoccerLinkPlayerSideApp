using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class CalendarItem
    {
        public string Title { get; set; }       // Np. "Mecz z Legią" lub "Trening siłowy"
        public string Description { get; set; } // Np. "Wyjazd, zbiórka 10:00"
        public DateTime Date { get; set; }      // Data zdarzenia
        public string Type { get; set; }        // "Mecz", "Trening", "Wydarzenie"
        public string Color { get; set; }       // Kolor kafelka (np. Czerwony dla meczu)
        public string Icon { get; set; }        // Ikonka (np. ⚽, 🏋️, 📅)
    }
}

