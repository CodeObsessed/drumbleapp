using DrumbleApp.Shared.Resources;
using System;
using System.Text.RegularExpressions;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class Email
    {
        public string EmailAddress { get; private set; }

        public Email()
        {
            EmailAddress = String.Empty;
        }

        public Email(string emailAddress)
        {
            ParseEmailAddress(emailAddress);
        }

        private void ParseEmailAddress(string emailAddress)
        {
            if (Email.IsValidEmail(emailAddress))
            {
                EmailAddress = emailAddress;
            }
            else
            {
                EmailAddress = String.Empty;

                throw new Exception(AppResources.InvalidEmailAlert);
            }
        }

        private static bool IsValidEmail(string emailAddress)
        {
            if (String.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
        }

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}
