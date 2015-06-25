using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class SearchItemResultModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PointResultModel Point { get; set; }
        public Guid ResourceId { get; set; }
        public double? Distance { get; set; }
    }
}
