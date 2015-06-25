using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class PublicTransportOperator : ViewModelBase
    {
        private Guid id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public Guid Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    RaisePropertyChanging("Id");
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private string name;
        [Column]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    RaisePropertyChanging("Name");
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string displayName;
        [Column]
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                if (displayName != value)
                {
                    RaisePropertyChanging("DisplayName");
                    displayName = value;
                    RaisePropertyChanged("DisplayName");
                }
            }
        }

        /*private EntitySet<PublicStop> stops = new EntitySet<PublicStop>();

        [Association(Storage = "stops", ThisKey = "Id", OtherKey = "operatorId")]
        public EntitySet<PublicStop> Stops
        {
            get
            {
                return stops;
            }
            set
            {
                stops.Assign(value);
            }
        }*/

        private string category;
        [Column]
        public string Category
        {
            get { return category; }
            set
            {
                if (category != value)
                {
                    RaisePropertyChanging("Category");
                    category = value;
                    RaisePropertyChanged("Category");
                }
            }
        }

        private string twitterHandle;
        [Column]
        public string TwitterHandle
        {
            get { return twitterHandle; }
            set
            {
                if (twitterHandle != value)
                {
                    RaisePropertyChanging("TwitterHandle");
                    twitterHandle = value;
                    RaisePropertyChanged("TwitterHandle");
                }
            }
        }

        private string facebookPage;
        [Column]
        public string FacebookPage
        {
            get { return facebookPage; }
            set
            {
                if (facebookPage != value)
                {
                    RaisePropertyChanging("FacebookPage");
                    facebookPage = value;
                    RaisePropertyChanged("FacebookPage");
                }
            }
        }

        private string websiteAddress;
        [Column]
        public string WebsiteAddress
        {
            get { return websiteAddress; }
            set
            {
                if (websiteAddress != value)
                {
                    RaisePropertyChanging("WebsiteAddress");
                    websiteAddress = value;
                    RaisePropertyChanged("WebsiteAddress");
                }
            }
        }

        private string contactEmail;
        [Column]
        public string ContactEmail
        {
            get { return contactEmail; }
            set
            {
                if (contactEmail != value)
                {
                    RaisePropertyChanging("ContactEmail");
                    contactEmail = value;
                    RaisePropertyChanged("ContactEmail");
                }
            }
        }

        private string contactNumber;
        [Column]
        public string ContactNumber
        {
            get { return contactNumber; }
            set
            {
                if (contactNumber != value)
                {
                    RaisePropertyChanging("ContactNumber");
                    contactNumber = value;
                    RaisePropertyChanged("ContactNumber");
                }
            }
        }

        private bool isPublic;
        [Column]
        public bool IsPublic
        {
            get { return isPublic; }
            set
            {
                if (isPublic != value)
                {
                    RaisePropertyChanging("IsPublic");
                    isPublic = value;
                    RaisePropertyChanged("IsPublic");
                }
            }
        }

        private string routeImageUrl;
        [Column]
        public string RouteImageUrl
        {
            get { return routeImageUrl; }
            set
            {
                if (routeImageUrl != value)
                {
                    RaisePropertyChanging("RouteImageUrl");
                    routeImageUrl = value;
                    RaisePropertyChanged("RouteImageUrl");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public PublicTransportOperator(Guid id, string name, string displayName, string category, string routeImageUrl, string twitterHandle, string facebookPage, string websiteAddress, string contactEmail, string contactNumber, bool isPublic)
        {
            this.Id = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.Category = category;
            this.RouteImageUrl = routeImageUrl;
            this.TwitterHandle = twitterHandle;
            this.WebsiteAddress = websiteAddress;
            this.FacebookPage = facebookPage;
            this.ContactEmail = contactEmail;
            this.ContactNumber = contactNumber;
            this.IsPublic = isPublic;
        }

        public PublicTransportOperator()
        {
        }

    }
}
