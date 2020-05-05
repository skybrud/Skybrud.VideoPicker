using HtmlAgilityPack;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;
using Skybrud.VideoPicker.Models.Videos;
using Skybrud.VideoPicker.Providers.DreamBroker.Models.Videos;

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

        public DreamBrokerEmbedOptions(DreamBrokerVideoDetails details) {
            _details = details;
        }

        #endregion

        #region Member methods

        public string GetHtml() {

            HttpQueryString query = new HttpQueryString();
            if (Autoplay) query.Add("autoplay", "true");

            string embedUrl = $"https://dreambroker.com/channel/{_details.ChannelId}/iframe/{_details.Id}{(query.IsEmpty ? string.Empty : "?" + query)}";

            HtmlDocument document = new HtmlDocument();

            HtmlNode iframe = document.CreateElement("iframe");

            iframe.Attributes.Add("frameborder", "0");
            iframe.Attributes.Add("width", "854");
            iframe.Attributes.Add("height", "480");

            iframe.Attributes.Add(RequireConsent ? "consent-src" : "src", embedUrl);

            if (Autoplay) iframe.Attributes.Add("allow", "autoplay");

            iframe.Attributes.Add("allowfullscreen", null);
            iframe.Attributes.Add("webkitallowfullscreen", null);
            iframe.Attributes.Add("mozallowfullscreen", null);

            if (string.IsNullOrWhiteSpace(_details.Title) == false) iframe.Attributes.Add("title", _details.Title);

            return iframe.OuterHtml;

        }

        #endregion

    }

}