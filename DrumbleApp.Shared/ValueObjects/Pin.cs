using DrumbleApp.Shared.Resources;
using System;
using System.Globalization;
using System.Linq;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class Pin
    {
        private const int expectedPinLength = 5;

        private string pinNumber;

        public string PinNumber
        {
            get
            {
                return pinNumber;
            }
            private set
            {
                if (!IsValid(value))
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, AppResources.InvalidOneTimePinAlert, expectedPinLength));
                }

                pinNumber = value;
            }
        }

        public static Pin CreateFrom(string pinNumber)
        {
            return new Pin(pinNumber);
        }

        public static bool IsValid(string pinNumber)
        {
            if (pinNumber == null)
                return false;

            return !(pinNumber.Length != expectedPinLength || pinNumber.Any(x => !Char.IsDigit(x)));
        }

        private Pin(string pinNumber)
        {
            PinNumber = pinNumber;
        }

        public override string ToString()
        {
            return PinNumber;
        }
    }
}
