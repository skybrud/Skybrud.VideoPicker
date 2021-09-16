using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Essentials.Time;
using Skybrud.VideoPicker.Config;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoProvider : IVideoProvider {
        
        private readonly VideoPickerConfig _config;

        #region Properties
        public string Alias => "vimeo";

        public string Name => "Vimeo";

        #endregion

        #region Constructors

        public VimeoProvider(VideoPickerConfig config) {
            _config = config;
        }

        #endregion

        #region Member methods

        public bool IsMatch(string source, out IVideoOptions options) {
            
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
            
            // Validate that the Vimeo provider is configured
            if (string.IsNullOrWhiteSpace(_config.VimeoAccessToken)) throw new VideoPickerException("Vimeo provider is not configured.");

            // Get the video ID from the regex
            string videoId = m1.Groups[2].Value;

            options = new VimeoVideoOptions(videoId);
            return true;

        }

        public VideoPickerItem GetVideo(IVideoOptions options) {
            
            // Must be an instannce of "VimeoVideoOptions"
            if (!(options is VimeoVideoOptions o)) return null;

            // Now get the video by it's ID
            return GetVideoById(o.VideoId);

        }

        public VideoPickerItem GetVideoById(string videoId) {
            
            if (string.IsNullOrWhiteSpace(_config.VimeoAccessToken)) throw new VideoPickerException("Vimeo provider is not configured.");


            // Initialize a new request
            HttpRequest request = new HttpRequest {
                Url = $"https://api.vimeo.com/videos/{videoId}",
                Headers = {
                    Authorization = "Bearer " + _config.VimeoAccessToken
                }
            };

            // Make the request to the API
            IHttpResponse response = request.GetResponse();

            switch (response.StatusCode) {

                case HttpStatusCode.NotFound:
                    throw new VideoPickerNotFoundException("Video not found.");

                case HttpStatusCode.OK:
                    return GetVideoFromResponse(videoId, response);

                default: 
                    throw new VideoPickerVimeoHttpException(response);
            
            }

        }

        private VideoPickerItem GetVideoFromResponse(string videoId, IHttpResponse response) {

            // Parse the response body
            JObject json = JsonUtils.ParseJsonObject(response.Body);

            // Return a new video picker item
            return new VideoPickerItem {
                Url = "https://vimeo.com/" + videoId,
                Provider = new VideoProviderDetails(this),
                Details = new VideoPickerDetails {
                    Id  = videoId,
                    Url = "https://vimeo.com/" + videoId,
                    Published = json.GetString("created_time", EssentialsTime.Parse),
                    Title = json.GetString("name") ?? string.Empty,
                    Description = json.GetString("description") ?? string.Empty,
                    Duration = json.GetDouble("duration", TimeSpan.FromSeconds),
                    Thumbnails = json.GetObjectArray("pictures.sizes", VideoPickerThumbnail.GetFromVimeo).OrderBy(x => x.Width).ToArray(),
                    Embed = json.GetString("embed.html")
                }
            };

        }

        public string GetEmbedCode(string videoId) {
            if (string.IsNullOrWhiteSpace(videoId)) throw new ArgumentNullException(nameof(videoId));
            return GetEmbedCode(new VimeoEmbedOptions(videoId));
        }

        public string GetEmbedCode(VimeoEmbedOptions options) {

            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.VideoId)) throw new PropertyNotSetException(nameof(options.VideoId));

            string url = "https://player.vimeo.com/video/" + options.VideoId;

            HttpQueryString query = new HttpQueryString();

            if (options.Color.HasValue()) query.Add("color", options.Color);
            
            if (options.AutoPause != null) query.Add("autopause", options.AutoPause.Value);
            if (options.AutoPlay != null) query.Add("autoplay", options.AutoPlay.Value ? 1 : 0);
            if (options.Loop != null) query.Add("loop", options.Loop.Value ? 1 : 0);
            if (options.ShowTitle != null) query.Add("title", options.ShowTitle.Value ? 1 : 0);
            if (options.ShowByLine != null) query.Add("byline", options.ShowByLine.Value ? 1 : 0);
            if (options.ShowPortrait != null) query.Add("portrait", options.ShowPortrait.Value ? 1 : 0);
            if (options.ShowControls != null) query.Add("controls", options.ShowControls.Value ? 1 : 0);
            if (options.DoNotTrack != null) query.Add("dnt", options.DoNotTrack.Value ? 1 : 0);
            if (options.IsMuted != null) query.Add("muted", options.IsMuted.Value ? 1 : 0);

            if (query.Count > 0) url += "?" + query;

            return $"<div style=\"padding:56.25% 0 0 0;position:relative;\"><iframe src=\"{url}\" style=\"position:absolute;top:0;left:0;width:100%;height:100%;\" frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture\" allowfullscreen></iframe></div>";

        }

        #endregion

    }
}