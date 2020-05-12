using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.DreamBroker {
    
    public class DreamBrokerEmbedOptions : IVideoEmbedOptions {

        private readonly DreamBrokerVideoDetails _details;

        #region Properties

        [JsonProperty("url")]
        public string Url => $"https://www.dreambroker.com/channel/{_details.ChannelId}/{_details.Id}";

        /// <summary>
        /// Gets whether the embed code requires prior consent before being show to the user.
        /// </summary>
        [JsonProperty("consent")]
        public bool RequireConsent { get; }

        /// <summary>
        /// Indicates whether the video should automatically start to play when the player loads.
        /// </summary>
        [JsonProperty("autoplay")]
        public bool Autoplay { get; }

        #endregion

        #region Constructors

        public DreamBrokerEmbedOptions(DreamBrokerVideoDetails details) : this(details, null) { }

        public DreamBrokerEmbedOptions(DreamBrokerVideoDetails details, DreamBrokerDataTypeConfig config) {

            _details = details;

            config = config ?? new DreamBrokerDataTypeConfig();

            RequireConsent = config.RequireConsent.Value;

        }

        #endregion

        #region Member methods

        public IHtmlString GetHtml() {
            return GetHtml(854, 480);
        }

        public IHtmlString GetHtml(int width, int height) {

            HttpQueryString query = new HttpQueryString();
            if (Autoplay) query.Add("autoplay", "true");

            string embedUrl = $"https://dreambroker.com/channel/{_details.ChannelId}/iframe/{_details.Id}{(query.IsEmpty ? string.Empty : "?" + query)}";

            HtmlDocument document = new HtmlDocument();

            HtmlNode iframe = document.CreateElement("iframe");

            iframe.Attributes.Add("frameborder", "0");
            iframe.Attributes.Add("width", width.ToString());
            iframe.Attributes.Add("height", height.ToString());

            iframe.Attributes.Add(RequireConsent ? "consent-src" : "src", embedUrl);

            if (Autoplay) iframe.Attributes.Add("allow", "autoplay");

            iframe.Attributes.Add("allowfullscreen", string.Empty);
            iframe.Attributes.Add("webkitallowfullscreen", string.Empty);
            iframe.Attributes.Add("mozallowfullscreen", string.Empty);

            if (string.IsNullOrWhiteSpace(_details.Title) == false) iframe.Attributes.Add("title", _details.Title);

            return new HtmlString(
                iframe.OuterHtml
                    .Replace("mozallowfullscreen=\"\"", "mozallowfullscreen")
                    .Replace("webkitallowfullscreen=\"\"", "webkitallowfullscreen")
                    .Replace("allowfullscreen=\"\"", "allowfullscreen")
            );

        }

        #endregion

    }

}