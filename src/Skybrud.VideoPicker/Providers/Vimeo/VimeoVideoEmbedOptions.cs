using System;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoEmbedOptions : IVideoEmbedOptions {

        private readonly VimeoVideoDetails _details;

        #region Properties

        [JsonProperty("url")]
        public string Url => "https://vimeo.com/" + _details.Id;

        /// <summary>
        /// Gets whether the embed code requires prior consent before being show to the user.
        /// </summary>
        [JsonProperty("consent")]
        public bool RequireConsent { get; }

        [JsonProperty("color")]
        public string Color { get; }

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

        [JsonProperty("title")]
        public bool ShowTitle { get; }

        [JsonProperty("byline")]
        public bool ShowByLine { get; }

        [JsonProperty("portrait")]
        public bool ShowPortrait { get; }

        #endregion

        #region Constructors

        public VimeoVideoEmbedOptions(VimeoVideoDetails details) : this(details, new VimeoDataTypeConfig()) { }

        public VimeoVideoEmbedOptions(VimeoVideoDetails details, VimeoDataTypeConfig config) {

            _details = details;

            Autoplay = config.Autoplay.Value;
            Loop = config.Loop.Value;
            Color = string.IsNullOrWhiteSpace(config.Color.Value) ? "00adef" : config.Color.Value.TrimStart('#');
            ShowTitle = config.ShowTitle.Value;
            ShowByLine = config.ShowByline.Value;
            ShowPortrait = config.ShowPortrait.Value;

        }

        #endregion

        public IHtmlString GetHtml() {

            // Vimeo sets the default width of the embed code to 640 pixels, and then calculates the height from the
            // dimensions and aspect ratio of the video
            int width = 640;
            int height = (int)Math.Round(width * (double)_details.Height / _details.Width);

            return GetHtml(width, height);

        }

        public IHtmlString GetHtml(int width, int height) {

            // Construct the query string
            HttpQueryString query = new HttpQueryString();
            if (Autoplay) query.Add("autoplay", "1");
            if (Loop) query.Add("loop", "1");
            if (string.IsNullOrWhiteSpace(Color) == false) query.Add("color", Color);

            if (ShowTitle == false) query.Add("title", "0");
            if (ShowByLine == false) query.Add("byline", "0");
            if (ShowPortrait == false) query.Add("portrait", "0");

            // Construct the embed URL
            string embedUrl = $"//player.vimeo.com/video/{_details.Id}" + (query.Count == 0 ? string.Empty : "?" + query);

            HtmlDocument document = new HtmlDocument();

            HtmlNode iframe = document.CreateElement("iframe");

            iframe.Attributes.Add(RequireConsent ? "consent-src" : "src", embedUrl);
            iframe.Attributes.Add("width", width.ToString());
            iframe.Attributes.Add("height", height.ToString());
            iframe.Attributes.Add("frameborder", "0");

            iframe.Attributes.Add("allow", "autoplay; fullscreen");
            iframe.Attributes.Add("allowfullscreen", string.Empty);

            if (string.IsNullOrWhiteSpace(_details.Title) == false) iframe.Attributes.Add("title", _details.Title);

            return new HtmlString(iframe.OuterHtml.Replace("allowfullscreen=\"\"", "allowfullscreen"));

        }

    }

}