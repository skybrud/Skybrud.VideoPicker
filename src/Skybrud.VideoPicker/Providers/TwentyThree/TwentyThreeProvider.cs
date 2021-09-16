using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree;
using Skybrud.Social.TwentyThree.Exceptions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;
using Skybrud.Social.TwentyThree.OAuth;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Spots;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Spots;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Umbraco.Core.Logging;

namespace Skybrud.VideoPicker.Providers.TwentyThree {
    
    public class TwentyThreeProvider : IVideoProvider {

        #region Properties

        public string Alias => "twentythree";

        public string Name => "Twenty Three";

        #endregion
        
        #region Member methods
        
        public virtual bool IsMatch(string source, out IVideoOptions options) {

            options = null;
            
            Match m1 = Regex.Match(source, "^(http|https)://([a-zA-Z0-9-\\.]+)/manage/video/([0-9]+)$", RegexOptions.IgnoreCase);
            Match m2 = Regex.Match(source, "(http|https)://(.+?)/(v|[0-9]+)\\.ihtml/player\\.html\\?token=([a-z0-9]+)&source=embed&photo%5fid=([0-9]+)");
            Match m3 = Regex.Match(source, "<script src=\"(http|https)://(.+?)/spot/([0-9]+)/([a-z0-9]+)/include\\.js");

            // From manage URL
            if (m1.Success) {
                
                string scheme = m1.Groups[1].Value;
                string domain = m1.Groups[2].Value;
                string videoId = m1.Groups[3].Value;

                options = new TwentyThreeVideoOptions(scheme, domain, videoId, null, null);

                return true;

            }

            // From <iframe>
            if (m2.Success) {
                
                string scheme = m2.Groups[1].Value;
                string domain = m2.Groups[2].Value;
                string playerId = m2.Groups[3].Value;
                string token = m2.Groups[4].Value;
                string videoId = m2.Groups[5].Value;

                options = new TwentyThreeVideoOptions(scheme, domain, videoId, token, playerId);

                return true;

            }

            // From spot <script>
            if (m3.Success) {
                
                string scheme = m3.Groups[1].Value;
                string domain = m3.Groups[2].Value;
                string spotId = m3.Groups[3].Value;
                string token = m2.Groups[4].Value;

                options = new TwentyThreeSpotOptions(scheme, domain, spotId, token);

                return true;

            }

            return false;

        }
        
        public VideoPickerItem GetVideo(IVideoOptions options) {

            switch (options) {

                case null:
                    throw new ArgumentNullException(nameof(options));

                case TwentyThreeVideoOptions video:
                    return GetVideoFromOptions(video);

                case TwentyThreeSpotOptions spot:
                    return GetSpotFromOptions(spot);

                default:
                    throw new Exception($"Unsupported options: {options}");
                
            }

        }

        private VideoPickerItem GetVideoFromOptions(TwentyThreeVideoOptions options) {

            // Get a reference to the API service implementation
            TwentyThreeHttpService api = GetHttpServiceFromDomain(options.Domain);
            if (api == null) throw new VideoPickerException($"Twenty Three is not configured for domain {options.Domain}");

            // Declare the options for the API
            TwentyThreeGetPhotosOptions o = new TwentyThreeGetPhotosOptions {
                PhotoId = options.VideoId,
                Token = options.Token,
                Video = TwentyThreeVideoParameter.OnlyVideos
            };
            
            TwentyThreeSite site;
            TwentyThreePhoto video;
            try {

                // Make the request to the API
                TwentyThreePhotoListResponse response = api.Photos.GetList(o);

                // Get the site and video from the response body
                site = response.Body.Site;
                video = response.Body.Photos.First();

            } catch (TwentyThreeHttpException ex) {

                if (ex.HasError) {
                    
                    switch (ex.Error.Code) {
                        
                        case "photo_not_found":
                        case "invalid_photo_token":
                            throw new VideoPickerNotFoundException($"No matching video found for TwentyThree account <strong>{options.Domain}</strong>.", ex);

                    }

                }

                LogHelper.Error<TwentyThreeProvider>("The request to the Twenty Three API failed.\r\n\r\n" + ex.Response.Body, ex);

                throw new VideoPickerException(HttpStatusCode.InternalServerError, "The request to the Twenty Three API failed.", ex);
            
            } catch (Exception ex) {
                
                LogHelper.Error<TwentyThreeProvider>("The request to the Twenty Three API failed.", ex);

                throw new VideoPickerException(HttpStatusCode.InternalServerError, "The request to the Twenty Three API failed.", ex);

            }
            
            // Get the scheme and host name
            string schemeAndHost = $"{options.Scheme}://{options.Domain}";

            // Return a new video picker item
            return new VideoPickerItem {
                Url = $"{schemeAndHost}/manage/video/{video.PhotoId}",
                Provider = new VideoProviderDetails(this),
                Details = new VideoPickerDetails {
                    Id = video.PhotoId,
                    Url = video.AbsoluteUrl,
                    Type = "video",
                    Published = video.PublishDate,
                    Title = video.Title,
                    Description = video.Content,
                    Duration = video.VideoLength,
                    Thumbnails = video.Thumbnails.Select(x => new VideoPickerThumbnail(schemeAndHost, x)).ToArray(),
                    Formats = video.VideoFormats.Select(x => new VideoPickerFormat(schemeAndHost, x)).ToArray(),
                    Embed = TwentyThreeUtils.GetEmbedCode(site, video, options.PlayerId)
                }
            };

        }

        private VideoPickerItem GetSpotFromOptions(TwentyThreeSpotOptions options) {

            // Get a reference to the API service implementation
            TwentyThreeHttpService api = GetHttpServiceFromDomain(options.Domain);
            if (api == null) throw new VideoPickerException($"Twenty Three is not configured for domain {options.Domain}");
            
            // Get the scheme and host name
            string schemeAndHost = $"{options.Scheme}://{options.Domain}";

            // Get information about the spot
            TwentyThreeSpotListResponse response = api.Spots.GetList(new TwentyThreeGetSpotsOptions {
                SpotId = options.SpotId,
                Token = options.Token
            });
            
            // Get the first spot
            TwentyThreeSpot spot = response.Body.Spots[0];

            // The spot is made up of one or more videos (aka photos), so we can get information about the first video
            // to find some thumbnails (seems to be the best approach for now)
            string firstPhotoId = spot.SpotSelection
                .Split(' ')
                .Select(x => x.Split(':')[1])
                .FirstOrDefault();

            VideoPickerThumbnail[] thumbnails = null;

            if (firstPhotoId.HasValue()) {
                try {
                    
                    TwentyThreePhotoListResponse response2 = api.Photos.GetList(new TwentyThreeGetPhotosOptions {
                        PhotoId = firstPhotoId
                    });

                    // Get the thumbnails from the first video/photo (currently asuming that one video is returned)
                    thumbnails = response2.Body.Photos[0].Thumbnails.Select(x => new VideoPickerThumbnail(schemeAndHost, x)).ToArray();

                } catch {
                    // Don't care
                }

            }

            // Return a new video picker item
            return new VideoPickerItem {
                Provider = new VideoProviderDetails(this),
                Details = new VideoPickerDetails {
                    Id = spot.SpotId,
                    Domain = options.Domain,
                    Type = "spot",
                    Title = spot.SpotName,
                    Thumbnails = thumbnails,
                    Embed = spot.IncludeHtml
                }
            };

        }

        private TwentyThreeHttpService GetHttpServiceFromDomain(string domain) {

            string prefix = "SkybrudVideoPicker:TwentyThree{" + domain + "}";

            TwentyThreeOAuthClient client = new TwentyThreeOAuthClient {
                Domain = domain,
                ConsumerKey = WebConfigurationManager.AppSettings[prefix + ":ConsumerKey"],
                ConsumerSecret = WebConfigurationManager.AppSettings[prefix + ":ConsumerSecret"],
                Token = WebConfigurationManager.AppSettings[prefix + ":AccessToken"],
                TokenSecret = WebConfigurationManager.AppSettings[prefix + ":AccessTokenSecret"]
            };

            if (string.IsNullOrWhiteSpace(client.ConsumerKey)) return null;
            if (string.IsNullOrWhiteSpace(client.ConsumerSecret)) return null;
            if (string.IsNullOrWhiteSpace(client.Token)) return null;
            if (string.IsNullOrWhiteSpace(client.TokenSecret)) return null;

            return TwentyThreeHttpService.CreateFromOAuthClient(client);

        }

        #endregion

    }

}