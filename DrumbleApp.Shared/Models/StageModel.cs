using GalaSoft.MvvmLight;
using System.Windows;

namespace DrumbleApp.Shared.Models
{
    public class StageModel : ViewModelBase
    {
        public string Colour { get; private set; }
        public int Width { get; private set; }

        private Visibility longWaitVisibility;
        public Visibility LongWaitVisibility
        {
            get { return longWaitVisibility; }
            set
            {
                longWaitVisibility = value;
                RaisePropertyChanged("LongWaitVisibility");
            }
        }

        public StageModel(string colour, int width, Visibility longWaitVisibility)
            : this (colour, width)
        {
            this.LongWaitVisibility = longWaitVisibility;
        }

        public StageModel(string colour, int width)
        {
            this.LongWaitVisibility = Visibility.Collapsed;
            this.Colour = colour;
            this.Width = width;
        }

        public static StageModel Waiting(int width, Visibility longWaitVisibility)
        {
            return new StageModel("#222222", width, longWaitVisibility);
        }

        public static StageModel Waiting(int width)
        {
            return new StageModel("#222222", width);
        }
    }
}
