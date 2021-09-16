using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skybrud.VideoPicker.Config;
using Skybrud.VideoPicker.Providers.TwentyThree;
using Skybrud.VideoPicker.Providers.Vimeo;
using Skybrud.VideoPicker.Providers.YouTube;

namespace Skybrud.VideoPicker.Providers {
    
    public class VideoPickerProviderCollection : IEnumerable<IVideoProvider> {

        public static readonly VideoPickerProviderCollection Instance = new VideoPickerProviderCollection();

        private readonly List<IVideoProvider> _providers = new List<IVideoProvider>();

        private VideoPickerProviderCollection() {

            VideoPickerConfig config = new VideoPickerConfig();
            
            _providers.Add(new VimeoProvider(config));
            _providers.Add(new YouTubeProvider(config));
            _providers.Add(new TwentyThreeProvider());

        }

        public bool TryGetProvider<T>(out T result) where T : IVideoProvider {
            result = this.OfType<T>().FirstOrDefault();
            return result != null;
        }

        public bool TryGetProvider(string alias, out IVideoProvider result) {
            result = this.FirstOrDefault(x => x.Alias == alias);
            return result != null;
        }

        public IEnumerator<IVideoProvider> GetEnumerator() {
            return _providers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}