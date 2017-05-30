using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Skybrud.Essentials.Time;
using Skybrud.Social.Google;
using Skybrud.Social.Google.YouTube.Objects.Videos;
using Skybrud.Social.Google.YouTube.Options;
using Skybrud.Social.Google.YouTube.Responses;
using Skybrud.Social.Vimeo.Advanced;
using Skybrud.Social.Vimeo.Advanced.Objects;
using Skybrud.Social.Vimeo.Advanced.Responses;
using Skybrud.VideoPicker.Config;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.VideoPicker.Controllers.Api {

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

                if (String.IsNullOrWhiteSpace(Config.VimeoConsumerKey)) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Vimeo is not configured"));
                if (String.IsNullOrWhiteSpace(Config.VimeoConsumerSecret)) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Vimeo is not configured"));

                VimeoService vimeo = VimeoService.CreateFromConsumerKey(Config.VimeoConsumerKey, Config.VimeoConsumerSecret);

                try {

                    VimeoVideoResponse response = vimeo.Videos.GetInfo(Int32.Parse(videoId));

                    VimeoVideo video = response.Video;

                    return new {
                        url = "https://vimeo.com/" + videoId,
                        type = "vimeo",
                        details = new {
                            id = videoId,
                            published = TimeUtils.GetUnixTimeFromDateTime(video.UploadDate),
                            title = video.Title,
                            description = video.Description,
                            duration = (int) video.Duration.TotalSeconds,
                            thumbnails = (
                                from thumbnail in video.Thumbnails
                                select new {
                                    url = thumbnail.Url,
                                    width = thumbnail.Width,
                                    height = thumbnail.Height
                                }
                            )
                        }
                    };

                } catch (VimeoException ex) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Something went wrong: " + ex.Message));
                }

            }

            if (m2.Success) {

                if (String.IsNullOrWhiteSpace(Config.GoogleServerKey)) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "YouTube is not configured"));

                string videoId = m2.Groups[1].Value;

                GoogleService service = GoogleService.CreateFromServerKey(Config.GoogleServerKey);

                YouTubeVideoListResponse response = service.YouTube.Videos.GetVideos(new YouTubeVideoListOptions {
                    Part = YouTubeVideoPart.Snippet + YouTubeVideoPart.ContentDetails,
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