using DrumbleApp.Domain.Models.Enums;
using DrumbleApp.Domain.Models.Resources;
using System;
using System.Windows.Controls;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public class PopupMessage
    {
        public string Caption { get; private set; }

        public string Message { get; private set; }
        public string LeftButtonText { get; private set; }
        public string RightButtonText { get; private set; }

        public HyperlinkButton HyperLink { get; set; }

        public PopupMessage(PopupMessageType messageType, string message, string leftButtonText, string rightButtonText)
        {
            if (String.IsNullOrEmpty(message))
                throw new Exception("Message cannot be empty");

            this.Message = message;
            this.Caption = GetMesage(messageType);
            this.LeftButtonText = leftButtonText;
            this.RightButtonText = rightButtonText;
        }

        public PopupMessage(PopupMessageType messageType, string message, string leftButtonText, string rightButtonText, HyperlinkButton hyperLink)
            : this(messageType, message, leftButtonText, rightButtonText)
        {
            this.HyperLink = hyperLink;
        }

        private string GetMesage(PopupMessageType messageType)
        {
            switch (messageType)
            {
                case PopupMessageType.Error:
                    return AppResources.PopupMessageTypeError;
                case PopupMessageType.Information:
                    return AppResources.PopupMessageTypeInformation;
                case PopupMessageType.Warning:
                    return AppResources.PopupMessageTypeWarning;
                case PopupMessageType.Sucess:
                    return AppResources.PopupMessageTypeSuccess;
                default:
                    return String.Empty;
            }
        }
    }
}
