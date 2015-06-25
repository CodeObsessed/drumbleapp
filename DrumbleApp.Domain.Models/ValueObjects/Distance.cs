using DrumbleApp.Domain.Models.Resources;
using System;
using System.Globalization;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class Distance
    {
        private bool metric;
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
        }

        public double Value { get; private set; }

        public Distance(double meters, bool metric)
        {
            this.metric = metric;
            SetValue(meters, metric);
            SetText();
        }

        public override string ToString()
        {
            return text;
        }

        private void SetValue(double meters, bool metric)
        {
            if (metric)
            {
                this.Value = meters;
                return;
            }

            double yards = meters * 1.09361;

            this.Value = yards;
        }

        private void SetText()
        {
            if (this.Value <= 0)
                text = String.Empty;

            if (this.metric)
            {
                if (this.Value < 1000)
                    text = this.Value.ToString(CultureInfo.InvariantCulture) + AppResources.MetersAbbr;
                else
                    text = Math.Round(this.Value / 1000, 1).ToString(CultureInfo.InvariantCulture) + AppResources.KilometersAbbr;
            }
            else
            {
                if (this.Value < 1760)
                    text = Math.Round(this.Value, 0).ToString(CultureInfo.InvariantCulture) + AppResources.YardsAbbr;
                else
                    text = Math.Round(this.Value / 1760, 1).ToString(CultureInfo.InvariantCulture) + AppResources.MilesAbbr;
            }
        }
    }
}
