using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Models
{
    public sealed class WhereToModel
    {
        public string Text { get; private set; }
        public Coordinate Point { get; private set; }
        public SearchType SearchType { get; private set; }

        public WhereToModel(string text, Coordinate point, SearchType searchType)
        {
            this.Text = text;
            this.Point = point;
            this.SearchType = searchType;
        }
    }
}
