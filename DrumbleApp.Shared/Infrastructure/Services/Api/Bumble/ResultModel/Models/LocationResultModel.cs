using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class LocationResultModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public PointResultModel Point { get; set; }
        public int Distance { get; set; }
        public Guid ResourceId { get; set; }
        public string Description { get; set; }
    }
}
