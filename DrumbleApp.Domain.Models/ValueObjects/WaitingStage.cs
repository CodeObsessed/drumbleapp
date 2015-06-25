using DrumbleApp.Domain.Models.Resources;
using System.Windows;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class WaitingStage
    {
        private Time time;

        public bool IsLongWait { get; private set; }
        public Visibility LongWaitingTimeVisibility { get; private set; }

        public string Name 
        {
            get
            {
                return AppResources.WaitingStageName;
            }
        }

        public string WaitTimeDisplay 
        {
            get
            {
                return "(" + time.Text + ")";
            }
        }

        public WaitingStage(int waitTimeInMinutes)
        {
            this.time = new Time(new System.TimeSpan(0, waitTimeInMinutes, 0));
            if (waitTimeInMinutes > 20)
            {
                this.IsLongWait = true;
                this.LongWaitingTimeVisibility = Visibility.Visible;
            }
            else
            {
                this.IsLongWait = false;
                this.LongWaitingTimeVisibility = Visibility.Collapsed;
            }
        }
    }
}
