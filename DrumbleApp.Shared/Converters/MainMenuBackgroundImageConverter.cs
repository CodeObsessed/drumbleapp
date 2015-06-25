using System;
using System.Windows.Data;

namespace DrumbleApp.Shared.Converters
{
    public class MainMenuBackgroundImageConverter : IValueConverter
    {
        string lightBlueImage = "/Images/Custom/LB/LineRight.png";
        string darkBlueImage = "/Images/Custom/DB/LineRight.png";
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        {
            return (int)value % 2 == 1 ? darkBlueImage : lightBlueImage; 
        }         
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        { 
            throw new NotImplementedException(); 
        }
    }
}
