using System.Globalization;

namespace SoccerLinkPlayerSideApp.Models // lub .Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Colors.Orange; // Kolor aktywnej zakładki
            }
            return Color.FromArgb("#2A5670"); // Kolor nieaktywnej zakładki (ciemny niebieski)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}