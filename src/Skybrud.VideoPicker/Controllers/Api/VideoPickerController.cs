using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Configuration;
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
using Skybrud.Social.TwentyThree;
using Skybrud.Social.TwentyThree.Exceptions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.OAuth;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.VideoPicker.Config;
using Skybrud.VideoPicker.Models;
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
            Match m3 = Regex.Match(url, "^(http|https)://([a-zA-Z0-9-\\.]+)/manage/video/([0-9]+)$", RegexOptions.IgnoreCase);

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

                    // Return a new video picker item
                    return new VideoPickerItem {
                        Url = "https://vimeo.com/" + videoId,
                        Type = "vimeo",
                        Details = new VideoPickerDetails {
                            Id  = videoId,
                            Url = "https://vimeo.com/" + videoId,
                            Published = obj.GetString("created_time", EssentialsDateTime.Parse).DateTime,
                            Title = obj.GetString("name") ?? String.Empty,
                            Description = obj.GetString("description") ?? String.Empty,
                            Duration = obj.GetDouble("duration", TimeSpan.FromSeconds),
                            Thumbnails = obj.GetObjectArray("pictures.sizes", VideoPickerThumbnail.GetFromVimeo).OrderBy(x => x.Width).ToArray()
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
                
                // Return a new video picker item
                return new VideoPickerItem {
                    Url = "https://www.youtube.com/watch?v=" + videoId,
                    Type = "youtube",
                    Details = new VideoPickerDetails {
                        Id  = videoId,
                        Url = "https://www.youtube.com/watch?v=" + videoId,
                        Published = video.Snippet.PublishedAt,
                        Title = video.Snippet.Title ?? String.Empty,
                        Description = video.Snippet.Description ?? String.Empty,
                        Duration = video.ContentDetails.Duration.Value,
                        Thumbnails = new[] {
                            GetThumbnail("default", video.Snippet.Thumbnails.Default),
                            GetThumbnail("medium", video.Snippet.Thumbnails.Medium),
                            GetThumbnail("high", video.Snippet.Thumbnails.High),
                            GetThumbnail("standard", video.Snippet.Thumbnails.Standard)
                        }.WhereNotNull().ToArray()
                    }
                };

            }

            if (m3.Success) {

                string scheme = m3.Groups[1].Value.ToLowerInvariant();
                string hostname = m3.Groups[2].Value.ToLowerInvariant();
                string id = m3.Groups[3].Value;

                TwentyThreeOAuthClient client = new TwentyThreeOAuthClient {
                    HostName = hostname,
                    ConsumerKey = WebConfigurationManager.AppSettings["SkybrudVideoPicker:TwentyThree{" + hostname + "}:ConsumerKey"],
                    ConsumerSecret = WebConfigurationManager.AppSettings["SkybrudVideoPicker:TwentyThree{" + hostname + "}:ConsumerSecret"],
                    Token = WebConfigurationManager.AppSettings["SkybrudVideoPicker:TwentyThree{" + hostname + "}:AccessToken"],
                    TokenSecret = WebConfigurationManager.AppSettings["SkybrudVideoPicker:TwentyThree{" + hostname + "}:AccessTokenSecret"]
                };

                if (String.IsNullOrWhiteSpace(client.ConsumerKey)) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Twenty Three is not configured for domain " + hostname));
                }

                return GetVideoFromTwentyThree(id, scheme, client);

            }

            return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.BadRequest, "Unknown URL syntax"));

        }

        [HttpGet]
        public object GetTwentyThreeVideo(string domain, string player, string token, string video) {

            if (Regex.IsMatch(domain, "^[a-z0-9-\\.]+$") == false) return "Bah";
            
            // Get a reference to the API service implementation
            TwentyThreeService api = GetTwentyThreeServiceFromDomain(domain);
            
            // Get information about the video (also called photo)
            TwentyThreeGetPhotosResponse response = api.Photos.GetList(new TwentyThreeGetPhotosOptions {
                PhotoId = video,
                Token = token,
                Video = TwentyThreeVideoParameter.OnlyVideos
            });

            // Get the first photo/video
            TwentyThreePhoto photo = response.Body.Photos[0];

            // Get the scheme and host name
            string schemeAndHost = "https://" + domain;

            // Return a new video picker item
            return new VideoPickerItem {
                Url = schemeAndHost + "/manage/video/" + photo.Id,
                Type = "twentythree",
                Details = new VideoPickerDetails {
                    Id = photo.Id,
                    Url = photo.AbsoluteUrl,
                    Published = photo.PublishDate,
                    Title = photo.Title,
                    Description = photo.Content,
                    Duration = photo.VideoLength,
                    Thumbnails = photo.Thumbnails.Select(x => new VideoPickerThumbnail(schemeAndHost, x)).ToArray(),
                    Formats = photo.VideoFormats.Select(x => new VideoPickerFormat(schemeAndHost, x)).ToArray(),
                }
            };

        }

        private TwentyThreeService GetTwentyThreeServiceFromDomain(string domain) {

            string prefix = "SkybrudVideoPicker:TwentyThree{" + domain + "}";

            TwentyThreeOAuthClient client = new TwentyThreeOAuthClient {
                HostName = domain,
                ConsumerKey = WebConfigurationManager.AppSettings[prefix + ":ConsumerKey"],
                ConsumerSecret = WebConfigurationManager.AppSettings[prefix + ":ConsumerSecret"],
                Token = WebConfigurationManager.AppSettings[prefix + ":AccessToken"],
                TokenSecret = WebConfigurationManager.AppSettings[prefix + ":AccessTokenSecret"]
            };

            if (string.IsNullOrWhiteSpace(client.ConsumerKey)) return null;
            if (string.IsNullOrWhiteSpace(client.ConsumerSecret)) return null;
            if (string.IsNullOrWhiteSpace(client.Token)) return null;
            if (string.IsNullOrWhiteSpace(client.TokenSecret)) return null;

            return TwentyThreeService.CreateFromOAuthClient(client);

        }

        private object GetVideoFromTwentyThree(string videoId, string scheme, TwentyThreeOAuthClient client) {

            // Initialize a new service instance from the client
            TwentyThreeService service = TwentyThreeService.CreateFromOAuthClient(client);

            // Declare the options for the API
            TwentyThreeGetPhotosOptions options = new TwentyThreeGetPhotosOptions {
                PhotoId = videoId,
                Video = TwentyThreeVideoParameter.OnlyVideos
            };

            TwentyThreePhoto video;
            try {

                // Make the request to the API
                TwentyThreeGetPhotosResponse response = service.Photos.GetList(options);

                // Get the video from the response body
                video = response.Body.Photos.First();

            } catch (TwentyThreeHttpException ex) {

                if (ex.HasError && ex.Error.Code == "invalid_photo_token") {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.BadRequest, "Video not found"));
                }

                LogHelper.Error<VideoPickerController>("The request to the Twenty Three API failed.", ex);

                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The request to the Twenty Three API failed."));
                
            } catch (Exception ex) {

                LogHelper.Error<VideoPickerController>("The request to the Twenty Three API failed.", ex);

                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The request to the Twenty Three API failed."));

            }

            // Get the scheme and host name
            string schemeAndHost = scheme + "://" + client.HostName;

            // Return a new video picker item
            return new VideoPickerItem {
                Url = schemeAndHost + "/manage/video/" + video.Id,
                Type = "twentythree",
                Details = new VideoPickerDetails {
                    Id  = videoId,
                    Url = video.AbsoluteUrl,
                    Published = video.PublishDate,
                    Title = video.Title,
                    Description = video.Content,
                    Duration = video.VideoLength,
                    Thumbnails = video.Thumbnails.Select(x => new VideoPickerThumbnail(schemeAndHost, x)).ToArray(),
                    Formats = video.VideoFormats.Select(x => new VideoPickerFormat(schemeAndHost, x)).ToArray(),
                }
            };

        }

        private VideoPickerThumbnail GetThumbnail(string alias, YouTubeVideoThumbnail thumbnail) {
            if (thumbnail == null) return null;
            return new VideoPickerThumbnail(alias, thumbnail);
        }

    }

}