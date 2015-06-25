using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Models
{
    public sealed class ResourceResultModel
    {
        public Guid ResourceId { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string GroupCategory { get; set; }
        public string State { get; set; }
    }
}
