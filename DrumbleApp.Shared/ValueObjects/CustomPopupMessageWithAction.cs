using DrumbleApp.Shared.Enums;
using System;
using System.Windows.Controls;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class CustomPopupMessageWithAction : CustomPopupMessage
    {
        public Action LeftButtonAction { get; private set; }

        public Action RightButtonAction { get; private set; }

        public Action NoButtonAction { get; private set; }

        public CustomPopupMessageWithAction(CustomPopupMessageType messageType, string message, string leftButtonText, string rightButtonText, Action leftButtonAction, Action rightButtonAction, Action noButtonAction)
            : base(messageType, message, leftButtonText, rightButtonText)
        {
            this.LeftButtonAction = leftButtonAction;
            this.RightButtonAction = rightButtonAction;
            this.NoButtonAction = noButtonAction;
        }

        public CustomPopupMessageWithAction(CustomPopupMessageType messageType, string message, string leftButtonText, string rightButtonText, Action leftButtonAction, Action rightButtonAction, Action noButtonAction, HyperlinkButton hyperLink)
            : base(messageType, message, leftButtonText, rightButtonText, hyperLink)
        {
            this.LeftButtonAction = leftButtonAction;
            this.RightButtonAction = rightButtonAction;
            this.NoButtonAction = noButtonAction;
        }
    }
}
