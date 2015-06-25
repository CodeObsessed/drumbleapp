using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Models
{
    public sealed class SearchTypeModel
    {
        public SearchType SearchType { get; private set; }

        public SearchTypeModel(SearchType searchType)
        {
            this.SearchType = searchType;
        }
    }
}
