using GalaSoft.MvvmLight;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class PublicTransportOperator : ViewModelBase
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
        public bool HasBeenModified { get; set; }

        private bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }

        }

        public PublicTransportOperator(string name, string displayName, string category, string routeMapUrl, string twitterHandle, string facebookPage, string websiteAddress, string contactEmail, string contactNumber, bool isPublic, bool isEnabled, bool hasBeenModified)
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
            this.IsEnabled = isEnabled;
            this.HasBeenModified = false;
        }

        public PublicTransportOperator(Guid id, string name, string displayName, string category, string routeMapUrl, string twitterHandle, string facebookPage, string websiteAddress, string contactEmail, string contactNumber, bool isPublic, bool isEnabled, bool hasBeenModified)
            : this(name, displayName, category, routeMapUrl, twitterHandle, facebookPage, websiteAddress, contactEmail, contactEmail, isPublic, isEnabled, hasBeenModified)
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
