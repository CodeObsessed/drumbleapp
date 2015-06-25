using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Resources;
using System;
using System.Windows.Controls;

namespace DrumbleApp.Shared.ValueObjects
{
    public class CustomPopupMessage
    {
        public string Caption { get; private set; }

        public string Message { get; private set; }
        public string LeftButtonText { get; private set; }
        public string RightButtonText { get; private set; }

        public HyperlinkButton HyperLink { get; set; }

        public CustomPopupMessage(CustomPopupMessageType messageType, string message, string leftButtonText, string rightButtonText)
        {
            if (String.IsNullOrEmpty(message))
                throw new Exception("Message cannot be empty");

            this.Message = message;
            this.Caption = GetMesage(messageType);
            this.LeftButtonText = leftButtonText;
            this.RightButtonText = rightButtonText;
        }

        public CustomPopupMessage(CustomPopupMessageType messageType, string message, string leftButtonText, string rightButtonText, HyperlinkButton hyperLink)
            : this(messageType, message, leftButtonText, rightButtonText)
        {
            this.HyperLink = hyperLink;
        }

        private string GetMesage(CustomPopupMessageType messageType)
        {
            switch (messageType)
            {
                case CustomPopupMessageType.Error:
                    return AppResources.CustomPopupMessageTypeError;
                case CustomPopupMessageType.Information:
                    return AppResources.CustomPopupMessageTypeInformation;
                case CustomPopupMessageType.Warning:
                    return AppResources.CustomPopupMessageTypeWarning;
                case CustomPopupMessageType.Sucess:
                    return AppResources.CustomPopupMessageTypeSuccess;
                default:
                    return String.Empty;
            }
        }
    }
}
