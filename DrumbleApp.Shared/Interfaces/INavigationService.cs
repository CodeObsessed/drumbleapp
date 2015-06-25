using System;

namespace DrumbleApp.Shared.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo(string url);
        void NavigateTo(Uri uri);
        void GoBack();
        void GoForward();
        void RemoveBackEntries();
        void RemoveBackEntry();
        string CurrentPage();
    }
}
