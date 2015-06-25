using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Entities
{
    public sealed class PublicTransportOperator
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Category { get; private set; }
        public string TwitterHandle { get; private set; }
        public string FacebookPage { get; private set; }
        public string WebsiteAddress { get; private set; }
        public string RouteMapUrl { get; private set; }
        public string ContactEmail { get; private set; }
        public string ContactNumber { get; private set; }
        public bool IsPublic { get; private set; }

        public PublicTransportOperator(string name, string displayName, string category, string routeMapUrl, string twitterHandle, string facebookPage, string websiteAddress, string contactEmail, string contactNumber, bool isPublic)
        {
            this.Id = Guid.NewGuid();
            this.DisplayName = displayName;
            this.Name = name;
            this.Category = category;
            this.RouteMapUrl = routeMapUrl;
            this.TwitterHandle = twitterHandle;
            this.WebsiteAddress = websiteAddress;
            this.FacebookPage = facebookPage;
            this.ContactEmail = contactEmail;
            this.ContactNumber = contactNumber;
            this.IsPublic = isPublic;
        }

        public PublicTransportOperator(Guid id, string name, string displayName, string category, string routeMapUrl, string twitterHandle, string facebookPage, string websiteAddress, string contactEmail, string contactNumber, bool isPublic)
            : this(name, displayName, category, routeMapUrl, twitterHandle, facebookPage, websiteAddress, contactEmail, contactEmail, isPublic)
        {
            this.Id = id;
        }

        public PublicTransportOperator() { }

        public static PublicTransportOperator Empty
        {
            get
            {
                return new PublicTransportOperator() { Name = "Walk" };
            }
        }

        public static PublicTransportOperator Walking
        {
            get
            {
                return new PublicTransportOperator() { Name = "Walk" };
            }
        }

        public static PublicTransportOperator Waiting
        {
            get
            {
                return new PublicTransportOperator() { Name = "Wait" };
            }
        }

        //TODO Add Uber contact details etc.
        public static PublicTransportOperator Uber
        {
            get
            {
                return new PublicTransportOperator() { Name = "Uber" };
            }
        }
    }
}
