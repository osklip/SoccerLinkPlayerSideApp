using System.Globalization;

namespace SoccerLinkPlayerSideApp.Models
{
    public class CalendarItem
    {
        public string Title { get; set; }       // Np. "Mecz: Legia"
        public string Description { get; set; } // Np. "Stadion Miejski"
        public DateTime Date { get; set; }      // Pełna data i godzina
        public string Type { get; set; }        // "Mecz", "Trening", "Wydarzenie"
        public string Color { get; set; }       // Kolor kafelka
        public string Icon { get; set; }        // Ikona

        // NOWE: Sformatowana data specjalnie dla Dashboardu (np. "18 stycznia, niedziela, 16:00")
        // Używamy CultureInfo("pl-PL"), aby zawsze mieć polskie nazwy
        public string DisplayDate => Date.ToString("dd MMMM, dddd, HH:mm", new CultureInfo("pl-PL"));
    }
}