using DrumbleApp.Shared.Entities;
using System.Windows;

namespace DrumbleApp.Shared.Models
{
    public class OperatorModeModel
    {
        public PublicTransportOperator PublicTransportOperator { get; set; }
        public OperatorSetting OperatorSetting { get; set; }
        public Visibility IsPrivate { get; set; }
        public Visibility IsTourist { get; set; }

        public OperatorModeModel(PublicTransportOperator publicTransportOperator, OperatorSetting operatorSetting)
        {
            this.PublicTransportOperator = publicTransportOperator;
            this.OperatorSetting = operatorSetting;
            this.IsPrivate = (publicTransportOperator.IsPublic) ? Visibility.Collapsed : Visibility.Visible;
            this.IsTourist = (publicTransportOperator.Category == "Tourism") ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Enable()
        {
            OperatorSetting.IsEnabled = true;
        }

        public void Disable()
        {
            OperatorSetting.IsEnabled = false;
        }
    }
}
