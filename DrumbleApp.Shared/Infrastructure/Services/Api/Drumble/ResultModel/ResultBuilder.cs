using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Models;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel
{
    public static class ResultBuilder
    {
        public static IEnumerable<Resource> BuildResourcesResult(IEnumerable<ResourceResultModel> resources)
        {
            if (resources == null)
                throw new ArgumentNullException("resources");

            return resources.Select(x => ModelFactory.Create(x));
        }
    }
}
