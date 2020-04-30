using System;
using System.Xml.Linq;
using Skybrud.Essentials.Xml.Extensions;
using Skybrud.VideoPicker.Models.Config;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class YouTubeVideoConfig : IProviderConfig  {

        public YouTubeCredentials[] Credentials { get; }

        public YouTubeVideoConfig(XElement xml) {
            Credentials = xml.GetElements("credentials", x => new YouTubeCredentials(x));
        }

    }

    public class YouTubeCredentials : IProviderCredentials {
        
        public Guid Id { get; }

        public string Name { get; }

        /// <summary>
        /// If configured, gets the Google server key. Server keys allow accessing the YouTube API without a user context.
        /// </summary>
        public string ServerKey { get; }

        public YouTubeCredentials(XElement xml) {
            Id = xml.GetAttributeValue("id", Guid.Parse);
            Name = xml.GetAttributeValue("name");
            ServerKey = xml.GetAttributeValue("serverKey");
        }

    }

}