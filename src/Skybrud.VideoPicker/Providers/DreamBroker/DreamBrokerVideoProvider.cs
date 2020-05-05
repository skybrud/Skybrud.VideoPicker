using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Skybrud.VideoPicker.Providers.DreamBroker.Models;
using Skybrud.VideoPicker.Providers.DreamBroker.Models.Options;
using Skybrud.VideoPicker.Providers.DreamBroker.Models.Videos;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.Providers.DreamBroker {

    public class DreamBrokerVideoProvider : IVideoProvider {

        public string Alias => "dreambroker";

        public string Name => "Dream Broker";

        public bool IsMatch(VideoPickerService service, string source, out IVideoOptions options) {

            Match m1 = Regex.Match(source?.Split('?')[0].Trim() ?? string.Empty, "//dreambroker\\.com/channel/(.+?)/(.+?)$");
            Match m2 = Regex.Match(source?.Split('?')[0].Trim() ?? string.Empty, "//www\\.dreambroker\\.com/channel/(.+?)/(.+?)$");

            options = null;

            if (m1.Success) {
                options = new DreamBrokerVideoOptions(m1.Groups[1].Value, m1.Groups[2].Value);
            } else if (m2.Success) {
                options = new DreamBrokerVideoOptions(m2.Groups[1].Value, m2.Groups[2].Value);
            }

            return options != null;

        }

        public VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options) {

            if (!(options is DreamBrokerVideoOptions o)) return null;
            
            string oembedUrl = "https://dreambroker.com/channel/oembed?" + new HttpQueryString {
                {"url", $"https://dreambroker.com/channel/{o.ChannelId}/{o.VideoId}"},
                {"format", "json"}
            };

            // Get video information from the oembed endpoint
            IHttpResponse response = HttpUtils.Requests.Get(oembedUrl);
            if (response.StatusCode != HttpStatusCode.OK) return null;

            DreamBrokerOEmbed oembed = new DreamBrokerOEmbed(JsonUtils.ParseJsonObject(response.Body));

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            DreamBrokerVideoDetails details = new DreamBrokerVideoDetails(o.VideoId, o.ChannelId, oembed);

            DreamBrokerEmbedOptions embed = new DreamBrokerEmbedOptions(details);
            
            return new VideoPickerValue(provider, details, embed);

        }

        public VideoPickerValue ParseValue(JObject obj) {

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            DreamBrokerVideoDetails details = obj.GetObject("details", DreamBrokerVideoDetails.Parse);

            DreamBrokerEmbedOptions embed = new DreamBrokerEmbedOptions(details);

            return new VideoPickerValue(provider, details, embed);

            //string embed = obj.GetString("embed");

            //try {

            //    HtmlDocument document = new HtmlDocument();
            //    document.LoadHtml(embed);

            //    HtmlNode iframe = document.DocumentNode.FirstChild;

            //    HtmlAttribute src = iframe.Attributes["src"];
            //    HtmlAttribute allow = iframe.Attributes["allow"];

            //    if (src != null && string.IsNullOrWhiteSpace(src.Value) == false) {
            //        if (src.Value.Contains("autoplay=") == false) {
            //            if (src.Value.Contains("?")) {
            //                src.Value += "&autoplay=true";
            //            } else {
            //                src.Value += "?autoplay=true";
            //            }
            //        }
            //    }

            //    if (allow == null) {

            //        iframe.Attributes.Add("allow", "autoplay");

            //    } else {

            //        List<string> temp = StringUtils.ParseStringArray(allow.Value, ',').ToList();

            //        temp.Add("allow");

            //        allow.Value = string.Join(",", allow);

            //    }

            //    embed = iframe.OuterHtml.Replace("=\"\"", "");

            //} catch {

            //    throw;

            //    // ignore

            //}

            //return new VideoPickerValue(provider, details);

        }

        public IProviderConfig ParseConfig(XElement xml) {
            return null;
        }

    }

}