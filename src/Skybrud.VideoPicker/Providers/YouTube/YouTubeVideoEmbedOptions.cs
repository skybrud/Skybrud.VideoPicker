using HtmlAgilityPack;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class VimeoVideoEmbedOptions : IVideoEmbedOptions {

        private readonly YouTubeVideoDetails _details;

        #region Properties

        [JsonProperty("url")]
        public string Url => "https://www.youtube.com/watch?v=" + _details.Id;

        /// <summary>
        /// Gets whether the embed code requires prior consent before being show to the user.
        /// </summary>
        [JsonProperty("consent")]
        public bool RequireConsent { get; }

        /// <summary>
        /// Gets whether the player should use YouTube's <c>www.youtube-nocookie.com</c> domain instead.
        /// </summary>
        [JsonProperty("nocookie")]
        public bool NoCookie { get; }

        /// <summary>
        /// Gets whether player controls should be displayed in the video player.
        /// </summary>
        [JsonProperty("controls")]
        public bool ShowControls { get; }

        /// <summary>
        /// Indicates whether the video should automatically start to play when the player loads.
        /// </summary>
        [JsonProperty("autoplay")]
        public bool Autoplay { get; }

        /// <summary>
        /// Gets whether the video should play again and again.
        /// </summary>
        [JsonProperty("loop")]
        public bool Loop { get; }

        #endregion

        #region Constructors

        public VimeoVideoEmbedOptions(YouTubeVideoDetails details) : this(details, new YouTubeDataTypeConfig()) { }

        public VimeoVideoEmbedOptions(YouTubeVideoDetails details, YouTubeDataTypeConfig config) {

            _details = details;

            config = config ?? new YouTubeDataTypeConfig();

            RequireConsent = config.RequireConsent.Value;
            NoCookie = config.NoCookie.Value;
            ShowControls = config.ShowControls.Value;
            Autoplay = config.Autoplay.Value;
            Loop = config.Loop.Value;
            RequireConsent = config.RequireConsent.Value;

        }

        #endregion

        public string GetHtml() {

            // Construct the base URL
            string url = $"https://www.{(NoCookie ? "youtube-nocookie" : "youtube")}.com/embed/{_details.Id}";

            // Construct the query string
            HttpQueryString query = new HttpQueryString();
            if (ShowControls == false) query.Add("controls", "0");
            if (Autoplay) query.Add("autoplay", "1");
            if (Loop) query.Add("loop", "1");

            HtmlDocument document = new HtmlDocument();

            HtmlNode iframe = document.CreateElement("iframe");

            iframe.Attributes.Add("width", "560");
            iframe.Attributes.Add("width", "315");

            iframe.Attributes.Add(RequireConsent ? "consent-src" : "src", url + (query.IsEmpty ? string.Empty : "?" + query));
            iframe.Attributes.Add("frameborder", "0");

            iframe.Attributes.Add("allow", "accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture");
            iframe.Attributes.Add("allowfullscreen", null);

            if (string.IsNullOrWhiteSpace(_details.Title) == false) iframe.Attributes.Add("title", _details.Title);

            return iframe.OuterHtml;

        }

    }

}