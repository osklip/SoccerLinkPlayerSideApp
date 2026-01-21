using System.Globalization;

namespace SoccerLinkPlayerSideApp.Models
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Colors.Orange; 
            }
            return Color.FromArgb("#2A5670");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}