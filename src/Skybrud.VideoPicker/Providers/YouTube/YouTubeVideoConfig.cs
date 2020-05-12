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

}