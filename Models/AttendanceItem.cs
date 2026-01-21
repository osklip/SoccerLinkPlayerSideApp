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
        public string Title { get; set; } 
        public string DateDisplay { get; set; }
        public string Location { get; set; }

        public bool? IsPresent { get; set; }

        public Color YesColor => IsPresent == true
            ? Color.FromArgb("#2ECC71")
            : Color.FromArgb("#32404A");

        public Color YesTextColor => IsPresent == true
            ? Colors.White
            : Color.FromArgb("#86A9C8"); 

        public Color YesBorderColor => IsPresent == true
            ? Colors.Transparent
            : Color.FromArgb("#5A707A"); 

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
