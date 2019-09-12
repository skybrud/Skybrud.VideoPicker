using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models.TwentyThree {

    public class TwentyThreeVideoDetails {

        #region Properties

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("token")]
        public string Token { get; }

        [JsonProperty("domain")]
        public string Domain { get; }

        [JsonProperty("embed")]
        public string Embed { get; }

        #endregion

        #region Constructors

        public TwentyThreeVideoDetails(VideoPickerItem item) {

            Id = item.Details?.Id;

            VideoPickerFormat format = item.Details?.Formats.FirstOrDefault();

            Match m1 = Regex.Match(format?.Url ?? string.Empty, "://([a-z0-9-\\.]+)/([0-9]+)/([0-9]+)/([a-z0-9]+)/video_");
            if (m1.Success == false) return;

            Domain = m1.Groups[1].Value;
            Token = m1.Groups[4].Value;

            Embed = item.JObject.GetString("embed");

            if (string.IsNullOrWhiteSpace(Domain)) return;
            if (string.IsNullOrWhiteSpace(Token)) return;

            if (string.IsNullOrWhiteSpace(Embed) == false) return;

            Embed = $"<div style=\"width:100%; height:0; position: relative; padding-bottom:56.25%\"><iframe src=\"https://{Domain}/v.ihtml/player.html?token={Token}&source=embed&photo%5fid={Id}\" style=\"width:100%; height:100%; position: absolute; top: 0; left: 0;\" frameborder=\"0\" border=\"0\" scrolling=\"no\" allowfullscreen=\"1\" mozallowfullscreen=\"1\" webkitallowfullscreen=\"1\" allow=\"autoplay; fullscreen\"></iframe></div>";

        }

        #endregion

    }

}