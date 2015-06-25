using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Globalization;

namespace DrumbleApp.Shared.Converters
{
    public static class DistanceConverter
    {
        public static string MetersToText(int meters)
        {
            bool isMetric = SimpleIoc.Default.GetInstance<IInMemoryApplicationSettingModel>().GetSetting(ApplicationSetting.UseMetric).Value;
           
            if (isMetric)
            {
                if (meters < 1000)
                    return meters.ToString(CultureInfo.InvariantCulture) + "m";
                else
                    return Math.Round((double)meters / 1000, 1).ToString(CultureInfo.InvariantCulture) + "km";
            }
            else
            {
                double yards = meters * 1.09361;

                if (yards < 1760)
                    return Math.Round(yards, 0).ToString(CultureInfo.InvariantCulture) + "yds";
                else
                    return Math.Round(yards / 1760, 1).ToString(CultureInfo.InvariantCulture) + "mi";
            }
        }

        public static double MilesToMeters(double miles)
        {
            return miles * 1609.34;
        }
    }
}
