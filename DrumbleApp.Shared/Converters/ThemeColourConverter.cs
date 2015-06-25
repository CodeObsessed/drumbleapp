using DrumbleApp.Shared.Enums;
using System;
using System.Windows.Media;

namespace DrumbleApp.Shared.Converters
{
    public class ThemeColourConverter
    {
        public static string Black = "#00000000";

        public static SolidColorBrush GetBrushFromTheme(ThemeColour colour)
        {
            return new SolidColorBrush(GetColourFromTheme(colour));
        }

        public static SolidColorBrush GetBrushFromHex(string colour)
        {
            //strip out any # if they exist
            colour = colour.Replace("#", string.Empty);

            byte r = (byte)(Convert.ToUInt32(colour.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(colour.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(colour.Substring(4, 2), 16));

            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        public static Color GetColourFromTheme(ThemeColour colour)
        {
            switch (colour)
            {
                case ThemeColour.Clickable:
                    return Color.FromArgb(255, 47, 62, 77);
                case ThemeColour.DarkBlue:
                    return Color.FromArgb(255, 41, 128, 185);
                case ThemeColour.LightBlue:
                    return Color.FromArgb(255, 52, 152, 219);
                case ThemeColour.DrumbleBlue:
                    return Color.FromArgb(255, 0, 174, 255);
                case ThemeColour.Black:
                    return Color.FromArgb(255, 0, 0, 0);
                case ThemeColour.White:
                    return Color.FromArgb(255, 255, 255, 255);
                default:
                    return Color.FromArgb(255, 255, 255, 255);
            }
        }
    }
}
