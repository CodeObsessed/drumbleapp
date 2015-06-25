using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class StopPointResultModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public PointResultModel Point { get; set; }
    }
}
