using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Models;
using DrumbleApp.Shared.ValueObjects;
using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel
{
    public static class ModelFactory
    {
        public static Resource Create(ResourceResultModel resourceResultModel)
        {
            if (resourceResultModel == null)
            {
                throw new ArgumentNullException("resourceResultModel");
            }

            return new Resource(resourceResultModel.ResourceId, resourceResultModel.Text, resourceResultModel.Image, resourceResultModel.GroupCategory, resourceResultModel.State);
        }
    }
}
