using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel
{
    public sealed class UberErrorResponseModel
    {
        public UberMeta meta { get; set; }
        public IEnumerable<UberError> errors { get; set; }
    }
}
