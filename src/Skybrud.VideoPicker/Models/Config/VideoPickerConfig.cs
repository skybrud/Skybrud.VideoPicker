using System.Collections.Generic;
using Skybrud.VideoPicker.Providers;

namespace Skybrud.VideoPicker.Models.Config {
    
    public class VideoPickerConfig {

        private readonly Dictionary<string, IProviderConfig> _providers;

        public VideoPickerConfig() {
            _providers = new Dictionary<string, IProviderConfig>();
        }

        public VideoPickerConfig(Dictionary<string, IProviderConfig> providers) {
            _providers = providers ?? new Dictionary<string, IProviderConfig>();
        }

        public IProviderConfig GetConfig(string alias) {
            return _providers.TryGetValue(alias, out IProviderConfig config) ? config : null;
        }

        public IProviderConfig GetConfig(IVideoProvider provider) {
            return _providers.TryGetValue(provider.Alias, out IProviderConfig config) ? config : null;
        }

        public T GetConfig<T>(string alias) where T : IProviderConfig {
            return _providers.TryGetValue(alias, out IProviderConfig config) ? (T) config : default;
        }

        public T GetConfig<T>(IVideoProvider provider) where T : IProviderConfig {
            return _providers.TryGetValue(provider.Alias, out IProviderConfig config) ? (T) config : default;
        }

        public bool TryGetConfig(string alias, out IProviderConfig config) {
            return _providers.TryGetValue(alias, out config);
        }

        public bool TryGetConfig(IVideoProvider provider, out IProviderConfig config) {
            return _providers.TryGetValue(provider.Alias, out config);
        }

        public bool TryGetConfig<T>(string alias, out T config) where T : IProviderConfig {
            
            if (_providers.TryGetValue(alias, out IProviderConfig temp)) {
                config = (T) temp;
                return true;
            }

            config = default;
            return false;

        }

        public bool TryGetConfig<T>(IVideoProvider provider, out T config) where T : IProviderConfig {
            return TryGetConfig(provider.Alias, out config);
        }

    }

}