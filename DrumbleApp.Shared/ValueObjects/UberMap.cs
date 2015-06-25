using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberMap
    {
        public Uri Href { get; private set; }

        public UberMap(Uri href)
        {
            this.Href = href;
        }
    }
}
