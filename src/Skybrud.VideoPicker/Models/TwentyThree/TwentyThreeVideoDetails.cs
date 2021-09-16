using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree;

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

            if (item.Details?.Type == "spot") {
                Domain = item.Details.Domain;
                Embed = item.Details.Embed;
                Token = Embed == null ? null : Regex.Match(Embed, $"/spot/{Id}/([a-z0-9]+)/").Groups[1].Value;
                return;
            }

            VideoPickerFormat format = item.Details?.Formats.FirstOrDefault();

            Match m1 = Regex.Match(format?.Url ?? string.Empty, "://([a-z0-9-\\.]+)/([0-9]+)/([0-9]+)/([a-z0-9]+)/video_");
            if (m1.Success == false) return;

            Domain = m1.Groups[1].Value;
            Token = m1.Groups[4].Value;

            Embed = item.JObject.GetString("embed");

            if (string.IsNullOrWhiteSpace(Domain)) return;
            if (string.IsNullOrWhiteSpace(Token)) return;

            if (string.IsNullOrWhiteSpace(Embed) == false) return;

            Embed = TwentyThreeUtils.GetEmbedCode(Domain, Id, Token);

        }

        #endregion

    }

}