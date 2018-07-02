using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;
using Skybrud.Social.Google.Common;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using Skybrud.Social.Http;
using Skybrud.VideoPicker.Config;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.VideoPicker.Controllers.Api {

    public class VideoPickerVimeoHttpException : Exception {

        #region Properties

        /// <summary>
        /// Gets a reference to the underlying <see cref="SocialHttpResponse"/>.
        /// </summary>
        public SocialHttpResponse Response { get; private set; }

        /// <summary>
        /// Gets the HTTP status code returned by the Vimeo API.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the error message returned by the Vimeo API.
        /// </summary>
        public string Error { get; private set; }

        #endregion

        #region Constructors

        public VideoPickerVimeoHttpException(SocialHttpResponse response) : base("Invalid response received from the Vimeo API (Status: " + ((int)response.StatusCode) + ")") {
            
            Response = response;
            StatusCode = response.StatusCode;

            try {
                JObject obj = JsonUtils.ParseJsonObject(response.Body);
                Error = obj.GetString("error");
            } catch (Exception ex) {
                LogHelper.Error<VideoPickerVimeoHttpException>("Unable to parse error message received from the Vimeo API", ex);
            }
        
        }

        #endregion

    }

    [PluginController("Skybrud")]
    [JsonOnlyConfiguration]
    public class VideoPickerController : UmbracoApiController {

        protected readonly VideoPickerConfig Config = new VideoPickerConfig();

        [HttpGet]
        public object GetVideoFromUrl(string url) {

            // Validate that we have an URL
            if (String.IsNullOrWhiteSpace(url)) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.BadRequest, "No URL specified"));
            }

            Match m1 = Regex.Match(url, "vimeo.com/([0-9]+)$");
            Match m2 = Regex.Match(url, @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);

            if (m1.Success) {

                string videoId = m1.Groups[1].Value;

                if (String.IsNullOrWhiteSpace(Config.VimeoAccessToken)) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Vimeo is not configured"));

                try {
                    
                    SocialHttpRequest request = new SocialHttpRequest {
                        Url = "https://api.vimeo.com/videos/" + videoId,
                        Headers = {
                            Authorization = "Bearer " + Config.VimeoAccessToken
                        }
                    };

                    SocialHttpResponse response = request.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK) {
                        throw new VideoPickerVimeoHttpException(response);
                    }

                    JObject obj = JsonUtils.ParseJsonObject(response.Body);

                    return new {
                        url = "https://vimeo.com/" + videoId,
                        type = "vimeo",
                        details = new {
                            id = videoId,
                            published = obj.GetString("created_time", EssentialsDateTime.Parse).UnixTimestamp,
                            title = obj.GetString("name"),
                            description = obj.GetString("description"),
                            duration = obj.GetInt32("duration"),
                            thumbnails = (
                                from thumbnail in obj.GetObjectArray("pictures.sizes")
                                select new {
                                    url = thumbnail.GetString("link"),
                                    width = thumbnail.GetInt32("width"),
                                    height = thumbnail.GetInt32("height")
                                }
                            )
                        }
                    };

                } catch (Exception ex) {
                    LogHelper.Error<VideoPickerController>("Unable to load Vimeo video: " + videoId, ex);
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Something went wrong."));
                }

            }

            if (m2.Success) {

                if (String.IsNullOrWhiteSpace(Config.GoogleServerKey)) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "YouTube is not configured"));

                string videoId = m2.Groups[1].Value;

                GoogleService service = GoogleService.CreateFromServerKey(Config.GoogleServerKey);

                YouTubeGetVideoListResponse response = service.YouTube.Videos.GetVideos(new YouTubeGetVideoListOptions {
                    Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
                    Ids = new[] { videoId }
                });

                if (response.Body.Items.Length == 0) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.BadRequest, "Video not found"));
                }

                YouTubeVideo video = response.Body.Items[0];

                return new {
                    url = "https://www.youtube.com/watch?v=" + videoId,
                    type = "youtube",
                    details = new {
                        id = videoId,
                        published = TimeUtils.GetUnixTimeFromDateTime(video.Snippet.PublishedAt),
                        title = video.Snippet.Title,
                        description = video.Snippet.Description,
                        duration = (int) video.ContentDetails.Duration.Value.TotalSeconds,
                        thumbnails = new[] {
                            GetThumbnail("default", video.Snippet.Thumbnails.Default),
                            GetThumbnail("medium", video.Snippet.Thumbnails.Medium),
                            GetThumbnail("high", video.Snippet.Thumbnails.High),
                            GetThumbnail("standard", video.Snippet.Thumbnails.Standard)
                        }.WhereNotNull()
                    }
                };

            }

            return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.BadRequest, "Unknown URL syntax"));

        }

        private object GetThumbnail(string alias, YouTubeVideoThumbnail thumbnail) {
            if (thumbnail == null) return null;
            return new {
                alias,
                url = thumbnail.Url,
                width = thumbnail.Width,
                height = thumbnail.Height
            };
        }

    }

}