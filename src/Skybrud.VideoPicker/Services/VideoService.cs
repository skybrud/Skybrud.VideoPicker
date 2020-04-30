using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Providers.DreamBroker;
using Skybrud.VideoPicker.Providers.YouTube;
using Umbraco.Core.IO;

namespace Skybrud.VideoPicker.Services {
    
    public class VideoService {

        public List<IVideoProvider> Providers { get; }

        public VideoService() {
        
            Providers = new List<IVideoProvider> {
                new DreamBrokerVideoProvider(),
                new YouTubeVideoProvider()
            };

        }

        public virtual VideoPickerValue GetVideo(string source) {

            foreach (IVideoProvider provider in Providers) {
                if (provider.IsMatch(this, source, out IVideoOptions options)) {
                    return provider.GetVideo(this, options);
                }
            }

            return null;

        }

        public VideosConfiguration LoadConfig() {

            string path = IOHelper.MapPath("~/Config/Skybrud.VideoPicker.config");

            if (File.Exists(path) == false) return new VideosConfiguration();

            XElement settings = XElement.Load(path);

            XElement xProviders = settings.Element("providers");

            Dictionary<string, IProviderConfig> providers = new Dictionary<string, IProviderConfig>();

            if (xProviders != null) {
                
                foreach (XElement xProvider in xProviders.Elements()) {

                    IVideoProvider provider = Providers.FirstOrDefault(x => x.Alias == xProvider.Name);

                    IProviderConfig config = provider?.ParseConfig(xProvider);

                    if (config != null) providers.Add(provider.Alias, config);

                }

            }

            return new VideosConfiguration(providers);

        }

    }

}