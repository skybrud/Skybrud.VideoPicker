using System;
using System.Xml.Linq;
using Skybrud.Essentials.Xml.Extensions;
using Skybrud.Social.Vimeo;
using Skybrud.Social.Vimeo.OAuth;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models.Config;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoCredentials : IProviderCredentials {
        
        public Guid Id { get; }

        public string Name { get; }

        public string AccessToken { get; }

        public string ConsumerKey { get; }

        public string ConsumerSecret { get; }

        public bool IsConfigured { get; }

        public VimeoCredentials(XElement xml) {
            Id = xml.GetAttributeValue("id", Guid.Parse);
            Name = xml.GetAttributeValue("name");
            AccessToken = xml.GetAttributeValue("accessToken");
            ConsumerKey = xml.GetAttributeValue("consumerKey");
            ConsumerSecret = xml.GetAttributeValue("consumerSecret");
            IsConfigured = string.IsNullOrWhiteSpace(AccessToken) == false || string.IsNullOrWhiteSpace(ConsumerKey) == false && string.IsNullOrWhiteSpace(ConsumerSecret) == false;
        }

        public VimeoHttpService GetService() {

            if (string.IsNullOrWhiteSpace(AccessToken) == false) return VimeoHttpService.CreateFromAccessToken(AccessToken);

            if (string.IsNullOrWhiteSpace(ConsumerKey) == false && string.IsNullOrWhiteSpace(ConsumerSecret) == false) return VimeoHttpService.CreateFromOAuthClient(new VimeoOAuthClient(ConsumerKey, ConsumerSecret));

            throw new VideosException("Vimeo credentials isn't configured.");

        }

    }

}