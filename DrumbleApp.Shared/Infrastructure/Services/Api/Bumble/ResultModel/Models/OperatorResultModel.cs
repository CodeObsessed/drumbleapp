using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class OperatorResultModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<string> Modes { get; set; }
        public string Category { get; set; }
        public string TwitterHandle { get; set; }
        public string FacebookPage { get; set; }
        public string WebsiteAddress { get; set; }
        public string RouteMapUrl { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public bool IsPublic { get; set; }
    }
}
