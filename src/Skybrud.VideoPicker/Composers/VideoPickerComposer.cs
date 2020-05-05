using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;

namespace Skybrud.VideoPicker.Composers {

    public class VideoPickerComposer : IUserComposer {

        public void Compose(Composition composition) {

            composition.Register<VideoPickerService, VideoPickerService>();

            composition.Register(LoadConfiguration, Lifetime.Request);

        }

        private VideoPickerConfig LoadConfiguration(IFactory factory) {

            var service = factory.GetInstance<VideoPickerService>();

            string path = IOHelper.MapPath("~/Config/Skybrud.VideoPicker.config");

            if (File.Exists(path) == false) return new VideoPickerConfig();

            XElement settings = XElement.Load(path);

            XElement xProviders = settings.Element("providers");

            Dictionary<string, IProviderConfig> providers = new Dictionary<string, IProviderConfig>();

            if (xProviders != null) {

                foreach (XElement xProvider in xProviders.Elements()) {

                    IVideoProvider provider = service.Providers.FirstOrDefault(x => x.Alias == xProvider.Name);

                    IProviderConfig config = provider?.ParseConfig(xProvider);

                    if (config != null) providers.Add(provider.Alias, config);

                }

            }

            return new VideoPickerConfig(providers);

        }

    }

}