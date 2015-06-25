using DrumbleApp.Shared.Enums;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace DrumbleApp.Shared.Converters
{
    public class MainMenuBackgroundColourConverter : IValueConverter
    {
        SolidColorBrush lightBlueBrush = ThemeColourConverter.GetBrushFromTheme(ThemeColour.LightBlue);
        SolidColorBrush darkBlueBrush = ThemeColourConverter.GetBrushFromTheme(ThemeColour.DarkBlue); 
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        {
            return (int)value % 2 == 1 ? darkBlueBrush : lightBlueBrush; 
        }         
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        { 
            throw new NotImplementedException(); 
        }
    }
}
