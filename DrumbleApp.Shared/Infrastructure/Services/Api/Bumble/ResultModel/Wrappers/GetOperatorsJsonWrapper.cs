using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers
{
    public sealed class GetOperatorsJsonWrapper
    {
        public IEnumerable<OperatorResultModel> Operators { get; set; }
    }
}
