using System.Linq;
using System.Text.RegularExpressions;
using Skybrud.Social.Google.Common;
using Skybrud.Social.Google.YouTube.Exceptions;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using Skybrud.VideoPicker.Config;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Umbraco.Core;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class YouTubeProvider : IVideoProvider {

        private readonly VideoPickerConfig _config;

        #region Properties

        public string Alias => "youtube";

        public string Name => "YouTube";

        #endregion

        #region Constructors

        public YouTubeProvider(VideoPickerConfig config) {
            _config = config;
        }

        #endregion

        #region Member methods

        public virtual bool IsMatch(string source, out IVideoOptions options) {

            options = null;

            // Is "source" an iframe?
            if (source.StartsWith("<iframe")) {

                // Match the "src" attribute
                Match m0 = Regex.Match(source, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                if (m0.Success == false) return false;

                // Update the source with the value from the "src" attribute
                source = m0.Groups[1].Value;

            }
            
            // Does "source" match known formats of YouTube video URLs?
            Match m1 = Regex.Match(source, @"youtu(?:\.be|be\.com|be-nocookie\.com)/(embed/|)(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (m1.Success == false) return false;
            
            // Validate that the YouTube provider is configured
            if (string.IsNullOrWhiteSpace(_config.GoogleServerKey)) throw new VideoPickerException("YouTube provider is not configured.");
            
            // Get the video ID from the regex
            string videoId = m1.Groups[2].Value;

            options = new YouTubeVideoOptions(videoId);
            return true;

        }

        public VideoPickerItem GetVideo(IVideoOptions options) {
            
            // Must be an instannce of "YouTubeVideoOptions"
            if (!(options is YouTubeVideoOptions o)) return null;

            // Now get the video by it's ID
            return GetVideoById(o.VideoId);

        }

        public VideoPickerItem GetVideoById(string videoId) {
            
            if (string.IsNullOrWhiteSpace(_config.GoogleServerKey)) throw new VideoPickerException("YouTube provider is not configured.");

            GoogleService service = GoogleService.CreateFromServerKey(_config.GoogleServerKey);

            try {

                YouTubeGetVideoListResponse response = service.YouTube.Videos.GetVideos(new YouTubeGetVideoListOptions {
                    Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
                    Ids = new[] { videoId }
                });

                if (response.Body.Items.Length == 0) throw new VideoPickerNotFoundException("Video not found.");

                YouTubeVideo video = response.Body.Items[0];
                
                // Return a new video picker item
                return new VideoPickerItem {
                    Url = "https://www.youtube.com/watch?v=" + videoId,
                    Provider = new VideoProviderDetails(this),
                    Details = new VideoPickerDetails {
                        Id  = videoId,
                        Url = "https://www.youtube.com/watch?v=" + videoId,
                        Published = video.Snippet.PublishedAt,
                        Title = video.Snippet.Title ?? string.Empty,
                        Description = video.Snippet.Description ?? string.Empty,
                        Duration = video.ContentDetails.Duration.Value,
                        Thumbnails = new[] {
                            GetThumbnail("default", video.Snippet.Thumbnails.Default),
                            GetThumbnail("medium", video.Snippet.Thumbnails.Medium),
                            GetThumbnail("high", video.Snippet.Thumbnails.High),
                            GetThumbnail("standard", video.Snippet.Thumbnails.Standard)
                        }.WhereNotNull().ToArray()
                    }
                };

            } catch (YouTubeException ex) {

                if (ex.Message.StartsWith("The request cannot be completed because you have exceeded your")) {
                    throw new VideoPickerException("The quota associated with the YouTube provider has been exceeded.");
                }

                throw;

            }

        }

        public  VideoPickerThumbnail GetThumbnail(string alias, YouTubeVideoThumbnail thumbnail) {
            return thumbnail == null ? null : new VideoPickerThumbnail(alias, thumbnail);
        }

        #endregion

    }

}