using DrumbleApp.Shared.Resources;
using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class PredefinedDepartureTime
    {
        public int DepartureTimeInMinutes { get; private set; }

        public string DepartureTime { get; private set; }

        public PredefinedDepartureTime(int minutes)
        {
            DepartureTimeInMinutes = minutes;
            switch (minutes)
            {
                case 0:
                    DepartureTime = AppResources.PredefinedDepartureTimeNow;
                    break;
                case 15:
                    DepartureTime = AppResources.PredefinedDepartureTime15;
                    break;
                case 30:
                    DepartureTime = AppResources.PredefinedDepartureTime30;
                    break;
                case 45:
                    DepartureTime = AppResources.PredefinedDepartureTime45;
                    break;
                case 60:
                    DepartureTime = AppResources.PredefinedDepartureTime60;
                    break;
                case 90:
                    DepartureTime = AppResources.PredefinedDepartureTime90;
                    break;
                case 120:
                    DepartureTime = AppResources.PredefinedDepartureTime120;
                    break;
                case 240:
                    DepartureTime = AppResources.PredefinedDepartureTime240;
                    break;
                case 480:
                    DepartureTime = AppResources.PredefinedDepartureTime480;
                    break;
                case 960:
                    DepartureTime = AppResources.PredefinedDepartureTime960;
                    break;
                case 1440:
                    DepartureTime = AppResources.PredefinedDepartureTime1440;
                    break;
                default:
                    throw new Exception("Predefined time does not exist");
            }
        }

        public static PredefinedDepartureTime FromNow()
        {
            return new PredefinedDepartureTime(0);
        }
    }
}
