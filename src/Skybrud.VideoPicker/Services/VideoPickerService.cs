using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Providers;

namespace Skybrud.VideoPicker.Services {
    
    public class VideoPickerService {

        #region Properties

        public VideoPickerProviderCollection Providers => VideoPickerProviderCollection.Current;

        public VideoPickerConfig Config => VideoPickerConfig.Current;

        #endregion

        #region Member methods

        public virtual VideoPickerValue GetVideo(string source) {

            foreach (IVideoProvider provider in Providers) {
                if (provider.IsMatch(this, source, out IVideoOptions options)) {
                    return provider.GetVideo(this, options);
                }
            }

            return null;

        }

        #endregion

    }

}