using System;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class Resource
    {
        public Guid ResourceId { get; private set; }
        public string Text { get; private set; }
        public string Image { get; private set; }
        public string GroupCategory { get; private set; }
        public string State { get; private set; }

        public Resource(Guid resourceId, string text, string image, string groupCategory, string state)
        {
            this.ResourceId = resourceId;
            this.Text = text;
            this.Image = image;
            this.GroupCategory = groupCategory;
            this.State = state;
        }
    }
}
