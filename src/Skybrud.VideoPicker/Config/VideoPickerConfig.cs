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
        /// Gets the OAuth 1.0a consumer key used for accessing the Vimeo API.
        /// </summary>
        public string VimeoConsumerKey {
            get { return ConfigurationManager.AppSettings["SkybrudVideoPicker:VimeoConsumerKey"]; }
        }

        /// <summary>
        /// Gets the OAuth 1.0a consumer secret used for accessing the Vimeo API.
        /// </summary>
        public string VimeoConsumerSecret {
            get { return ConfigurationManager.AppSettings["SkybrudVideoPicker:VimeoConsumerSecret"]; }
        }

    }

}