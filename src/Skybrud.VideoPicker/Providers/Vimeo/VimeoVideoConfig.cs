using System.Xml.Linq;
using Skybrud.Essentials.Xml.Extensions;
using Skybrud.VideoPicker.Models.Config;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoConfig : IProviderConfig  {

        public VimeoCredentials[] Credentials { get; }

        public VimeoVideoConfig(XElement xml) {
            Credentials = xml.GetElements("credentials", x => new VimeoCredentials(x));
        }

    }

}