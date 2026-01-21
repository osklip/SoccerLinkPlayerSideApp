using System.Globalization;

namespace SoccerLinkPlayerSideApp.Models
{
    public class CalendarItem
    {
        public string Title { get; set; }   
        public string Description { get; set; } 
        public DateTime Date { get; set; }    
        public string Type { get; set; }       
        public string Color { get; set; }    
        public string Icon { get; set; }   

        public string DisplayDate => Date.ToString("dd MMMM, dddd, HH:mm", new CultureInfo("pl-PL"));
    }
}