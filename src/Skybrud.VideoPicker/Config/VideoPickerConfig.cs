using System.Web.Configuration;

namespace Skybrud.VideoPicker.Config {

    /// <summary>
    /// Class for accessing the Video Picker configuration.
    /// </summary>
    public class VideoPickerConfig {

        /// <summary>
        /// Gets the Google server key used for accesing the YouTube API.
        /// </summary>
        public string GoogleServerKey => WebConfigurationManager.AppSettings["SkybrudVideoPicker:GoogleServerKey"];

        /// <summary>
        /// Gets the OAuth 2.0 access token used for accessing the Vimeo API.
        /// </summary>
        public string VimeoAccessToken => WebConfigurationManager.AppSettings["SkybrudVideoPicker:VimeoAccessToken"];
    }

}