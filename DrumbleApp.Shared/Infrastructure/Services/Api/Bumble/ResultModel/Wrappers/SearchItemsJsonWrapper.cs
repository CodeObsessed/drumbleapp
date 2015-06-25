using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers
{
    public sealed class SearchItemsJsonWrapper
    {
        public IEnumerable<SearchItemResultModel> SearchItems { get; set; }
    }
}
