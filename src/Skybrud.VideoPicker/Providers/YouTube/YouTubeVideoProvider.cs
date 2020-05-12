using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings;
using Skybrud.Social.Google;
using Skybrud.Social.Google.YouTube;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Skybrud.VideoPicker.PropertyEditors;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class YouTubeVideoProvider : IVideoProvider {
        
        public string Alias => "youtube";

        public string Name => "YouTube";

        public string ConfigView => "/App_Plugins/Skybrud.VideoPicker/Views/YouTube/Config.html";

        public string EmbedView => "/App_Plugins/Skybrud.VideoPicker/Views/YouTube/Embed.html";

        public virtual bool IsMatch(VideoPickerService service, string source, out IVideoOptions options) {

            options = null;

            // Is "source" an iframe?
            if (source.StartsWith("<iframe")) {

                // Match the "src" attribute
                Match m0 = Regex.Match(source, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                if (m0.Success == false) return false;

                // Update the source with the value from the "src" attribute
                source = m0.Groups[1].Value;

            }
            
            // Does "source" match known formats of YouTube video URLs?
            Match m1 = Regex.Match(source, @"youtu(?:\.be|be\.com|be-nocookie\.com)/(embed/|)(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (m1.Success == false) return false;

            // Get a reference to the YouTube provider configuration
            YouTubeVideoConfig youtube = service.Config.GetConfig<YouTubeVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            YouTubeCredentials credentials = youtube?.Credentials.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(credentials?.ServerKey)) throw new VideosException("YouTube provider is not configured.");
            
            // Get the video ID from the regex
            string videoId = m1.Groups[2].Value;

            options = new YouTubeVideoOptions(videoId);
            return true;

        }

        public VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options) {

            if (!(options is YouTubeVideoOptions o)) return null;

            // Get a reference to the YouTube provider configuration
            YouTubeVideoConfig youtube = service.Config.GetConfig<YouTubeVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            YouTubeCredentials credentials = youtube?.Credentials.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(credentials?.ServerKey)) throw new VideosException("YouTube provider is not configured.");

            // Initialize a new GoogleService instance for accessing the YouTube API
            GoogleService google = GoogleService.CreateFromServerKey(credentials.ServerKey);
            
            // Make the request to the YouTube API
            YouTubeGetVideoListResponse response = google.YouTube().Videos.GetVideos(new YouTubeGetVideoListOptions {
                Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
                Ids = new[] { o.VideoId }
            });

            // Get the first video object from the response body
            YouTubeVideo video = response.Body.Items.FirstOrDefault();
            if (video == null) throw new VideosException("YouTube video not found.", HttpStatusCode.NotFound);

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            YouTubeVideoDetails details = new YouTubeVideoDetails(video);

            VimeoVideoEmbedOptions embed = new VimeoVideoEmbedOptions(details);

            return new VideoPickerValue(provider, details, embed);


        }

        public VideoPickerValue ParseValue(JObject obj, IProviderDataTypeConfig config) {

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            YouTubeVideoDetails details = obj.GetObject("details", YouTubeVideoDetails.Parse);

            VimeoVideoEmbedOptions embed = new VimeoVideoEmbedOptions(details, config as YouTubeDataTypeConfig);

            return new VideoPickerValue(provider, details, embed);

        }

        public IProviderConfig ParseConfig(XElement xml) {
            return new YouTubeVideoConfig(xml);
        }

        public IProviderDataTypeConfig ParseDataTypeConfig(JObject obj) {
            return new YouTubeDataTypeConfig(obj);
        }

    }

    public class YouTubeDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonProperty("consent")]
        public DataTypeConfigOption<bool> RequireConsent { get; }

        [JsonProperty("nocookie")]
        public DataTypeConfigOption<bool> NoCookie { get; }

        [JsonProperty("controls")]
        public DataTypeConfigOption<bool> ShowControls { get; }

        [JsonProperty("autoplay")]
        public DataTypeConfigOption<bool> Autoplay { get; }

        [JsonProperty("loop")]
        public DataTypeConfigOption<bool> Loop { get; }

        public YouTubeDataTypeConfig() : this(null) { }

        public YouTubeDataTypeConfig(JObject value) {

            IsEnabled = value.GetBoolean("enabled");
            
            JToken controls = value?.SelectToken("controls.value");

            RequireConsent = new DataTypeConfigOption<bool>(value.GetBoolean("consent.value"));
            NoCookie = new DataTypeConfigOption<bool>(value.GetBoolean("nocookie.value"));
            ShowControls = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(controls, true));
            Autoplay = new DataTypeConfigOption<bool>(value.GetBoolean("autoplay.value"));
            Loop = new DataTypeConfigOption<bool>(value.GetBoolean("loop.value"));

        }

    }

}