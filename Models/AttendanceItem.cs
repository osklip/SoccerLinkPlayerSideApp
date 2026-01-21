using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Models
{
    public class AttendanceItem
    {
        public int Id { get; set; }
        public string Title { get; set; }       // np. "Mecz: Legia Warszawa"
        public string DateDisplay { get; set; } // np. "20.01 18:00"
        public string Location { get; set; }

        // Status: null = brak decyzji, true = będę, false = nie będę
        public bool? IsPresent { get; set; }

        // KOLORY UI - Dopasowane do motywu (Colors.xaml)
        // Zielony (#2ECC71) jeśli obecny, inaczej szary (#BFBFBF)
        public Color YesColor => IsPresent == true
            ? Color.FromArgb("#2ECC71")
            : Color.FromArgb("#32404A"); // Ciemny, zlewający się z tłem lub InputColor

        public Color YesTextColor => IsPresent == true
            ? Colors.White
            : Color.FromArgb("#86A9C8"); // Muted

        public Color YesBorderColor => IsPresent == true
            ? Colors.Transparent
            : Color.FromArgb("#5A707A"); // CardBorder

        // Czerwony (#E74C3C) jeśli nieobecny, inaczej szary
        public Color NoColor => IsPresent == false
            ? Color.FromArgb("#E74C3C")
            : Color.FromArgb("#32404A");

        public Color NoTextColor => IsPresent == false
            ? Colors.White
            : Color.FromArgb("#86A9C8");

        public Color NoBorderColor => IsPresent == false
            ? Colors.Transparent
            : Color.FromArgb("#5A707A");
    }
}
