using System.Configuration;

namespace Skybrud.VideoPicker.Config {

    public class VideoPickerConfig {

        /// <summary>
        /// Gets the Google server key used for accesing the YouTube API.
        /// </summary>
        public string GoogleServerKey {
            get { return ConfigurationManager.AppSettings["SkybrudVideoPicker:GoogleServerKey"]; }
        }

        /// <summary>
        /// Gets the OAuth 2.0 access token used for accessing the Vimeo API.
        /// </summary>
        public string VimeoAccessToken {
            get { return ConfigurationManager.AppSettings["SkybrudVideoPicker:VimeoAccessToken"]; }
        }

    }

}