using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Resources;
using System.Windows;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class WaitingStage
    {
        public bool IsLongWait { get; private set; }

        public string Name 
        {
            get
            {
                return AppResources.WaitingStageName;
            }
        }

        private int waitTime = 0;
        public string WaitTimeDisplay 
        {
            get
            {
                return "(" + TimeConverter.MinutesToText(waitTime) + ")";
            }
        }

        public WaitingStage(int waitTimeInMinutes)
        {
            this.waitTime = waitTimeInMinutes;
            if (waitTimeInMinutes > 20)
                IsLongWait = true;
        }

        public Visibility LongWaitingTimeVisibility
        {
            get
            {
                if (IsLongWait)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
    }
}
