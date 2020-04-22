using System.Collections.Generic;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Providers.DreamBroker;

namespace Skybrud.VideoPicker.Services {
    
    public class VideoService {

        public List<IVideoProvider> Providers { get; }

        public VideoService() {
            Providers = new List<IVideoProvider> {
                new DreamBrokerVideoProvider()
            };
        }

    }

}