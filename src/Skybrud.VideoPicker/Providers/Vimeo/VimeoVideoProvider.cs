using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings;
using Skybrud.Social.Vimeo;
using Skybrud.Social.Vimeo.Models.Videos;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Skybrud.VideoPicker.PropertyEditors;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoProvider : IVideoProvider {
        
        public string Alias => "vimeo";

        public string Name => "Vimeo";

        public string ConfigView => "/App_Plugins/Skybrud.VideoPicker/Views/Vimeo/Config.html";

        public string EmbedView => "/App_Plugins/Skybrud.VideoPicker/Views/Vimeo/Embed.html";

        public virtual bool IsMatch(VideoPickerService service, string source, out IVideoOptions options) {

            options = null;

            // Is "source" an iframe?
            if (source.Contains("<iframe")) {

                // Match the "src" attribute
                Match m0 = Regex.Match(source, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                if (m0.Success == false) return false;

                // Update the source with the value from the "src" attribute
                source = m0.Groups[1].Value;

            }

            // Remove query string (if present)
            source = source.Split('?')[0];

            // Does "source" match known formats of YouTube video URLs?
            Match m1 = Regex.Match(source, "vimeo.com/(video/|)([0-9]+)$");
            if (m1.Success == false) return false;

            // Get a reference to the Vimeo provider configuration
            VimeoVideoConfig config = service.Config.GetConfig<VimeoVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            VimeoCredentials credentials = config?.Credentials.FirstOrDefault();
            if (credentials == null) throw new VideosException("Vimeo provider is not configured (1).");
            if (credentials.IsConfigured == false) throw new VideosException("Vimeo provider is not configured (2).");

            // Get the video ID from the regex
            long videoId = long.Parse(m1.Groups[2].Value);

            options = new VimeoVideoOptions(videoId);
            return true;

        }

        public VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options) {

            if (!(options is VimeoVideoOptions o)) return null;

            // Get a reference to the YouTube provider configuration
            VimeoVideoConfig config = service.Config.GetConfig<VimeoVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            VimeoCredentials credentials = config?.Credentials.FirstOrDefault();
            if (credentials == null) throw new VideosException("Vimeo provider is not configured (3).");
            if (credentials.IsConfigured == false) throw new VideosException("Vimeo provider is not configured (4).");

            // Initialize a new VimeoService instance for accessing the Vimeo API
            VimeoService vimeo = credentials.GetService();

            // Make the request to the Vimeo API
            var response = vimeo.Videos.GetVideo(o.VideoId);

            // Get the video object from the response body
            VimeoVideo video = response.Body;
            if (video == null) throw new VideosException("Vimeo video not found.", HttpStatusCode.NotFound);

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            VimeoVideoDetails details = new VimeoVideoDetails(video);

            VimeoVideoEmbedOptions embed = new VimeoVideoEmbedOptions(details);

            return new VideoPickerValue(provider, details, embed);

        }

        public VideoPickerValue ParseValue(JObject obj, IProviderDataTypeConfig config) {

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            VimeoVideoDetails details = obj.GetObject("details", VimeoVideoDetails.Parse);

            VimeoVideoEmbedOptions embed = new VimeoVideoEmbedOptions(details, config as VimeoDataTypeConfig);

            return new VideoPickerValue(provider, details, embed);

        }

        public IProviderConfig ParseConfig(XElement xml) {
            return new VimeoVideoConfig(xml);
        }

        public IProviderDataTypeConfig ParseDataTypeConfig(JObject obj) {
            return new VimeoDataTypeConfig(obj);
        }

    }

    public class VimeoDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonProperty("consent")]
        public DataTypeConfigOption<bool> RequireConsent { get; }

        [JsonProperty("autoplay")]
        public DataTypeConfigOption<bool> Autoplay { get; }

        [JsonProperty("loop")]
        public DataTypeConfigOption<bool> Loop { get; }

        [JsonProperty("color")]
        public DataTypeConfigOption<string> Color { get; }

        [JsonProperty("title")]
        public DataTypeConfigOption<bool> ShowTitle { get; }

        [JsonProperty("byline")]
        public DataTypeConfigOption<bool> ShowByline { get; }

        [JsonProperty("portrait")]
        public DataTypeConfigOption<bool> ShowPortrait { get; }

        public VimeoDataTypeConfig() : this(null) { }

        public VimeoDataTypeConfig(JObject value) {

            IsEnabled = value.GetBoolean("enabled");
            
            RequireConsent = new DataTypeConfigOption<bool>(value.GetBoolean("consent.value"));
            Autoplay = new DataTypeConfigOption<bool>(value.GetBoolean("autoplay.value"));
            Loop = new DataTypeConfigOption<bool>(value.GetBoolean("loop.value"));
            Color = new DataTypeConfigOption<string>(value.GetString("color.value"));
            ShowTitle = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("title.value"), true));
            ShowByline = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("byline.value"), true));
            ShowPortrait = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("portrait.value"), true));

        }

    }

}