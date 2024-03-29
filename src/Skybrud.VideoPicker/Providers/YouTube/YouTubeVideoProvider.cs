﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Google;
using Skybrud.Social.Google.YouTube;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Models.Providers;
using Skybrud.VideoPicker.PropertyEditors.Video;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.Providers.YouTube {

    public class YouTubeVideoProvider : IVideoProvider {

        public string Alias => "youtube";

        public string Name => "YouTube";

        public string ConfigView => "/App_Plugins/Skybrud.VideoPicker/Views/YouTube/Config.html";

        public string EmbedView => "/App_Plugins/Skybrud.VideoPicker/Views/YouTube/Embed.html";

        public virtual bool IsMatch(VideoPickerService service, string source, out IVideoOptions options) {

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

            // Get a reference to the YouTube provider configuration
            YouTubeVideoConfig youtube = service.Config.GetConfig<YouTubeVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            YouTubeCredentials credentials = youtube?.Credentials.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(credentials?.ServerKey)) throw new VideosException("YouTube provider is not configured.");

            // Get the video ID from the regex
            string videoId = m1.Groups[2].Value;

            options = new YouTubeVideoOptions(videoId);
            return true;

        }

        public VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options) {

            if (!(options is YouTubeVideoOptions o)) return null;

            // Get a reference to the YouTube provider configuration
            YouTubeVideoConfig youtube = service.Config.GetConfig<YouTubeVideoConfig>(this);

            // Get the first credentials (or trigger an error if none)
            YouTubeCredentials credentials = youtube?.Credentials.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(credentials?.ServerKey)) throw new VideosException("YouTube provider is not configured.");

            // Initialize a new GoogleService instance for accessing the YouTube API
            GoogleHttpService google = GoogleHttpService.CreateFromApiKey(credentials.ServerKey);

            // Make the request to the YouTube API
            YouTubeVideoListResponse response = google.YouTube().Videos.GetVideos(new YouTubeGetVideoListOptions {
                Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
                Ids = new List<string> { o.VideoId }
            });

            // Get the first video object from the response body
            YouTubeVideo video = response.Body.Items.FirstOrDefault();
            if (video == null) throw new VideosException("YouTube video not found.", HttpStatusCode.NotFound);

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            VideoProviderCredentialsDetails credentailsDetails = new VideoProviderCredentialsDetails(credentials);

            YouTubeVideoDetails details = new YouTubeVideoDetails(video);

            YouTubeVideoEmbedOptions embed = new YouTubeVideoEmbedOptions(details);

            return new VideoPickerValue(provider, credentailsDetails, details, embed);


        }

        public VideoPickerValue ParseValue(JObject obj, IProviderDataTypeConfig config) {

            VideoProviderDetails provider = new VideoProviderDetails(Alias, Name);

            VideoProviderCredentialsDetails credentials = obj.GetObject("credentials", VideoProviderCredentialsDetails.Parse);

            YouTubeVideoDetails details = obj.GetObject("details", YouTubeVideoDetails.Parse);

            YouTubeVideoEmbedOptions embed = new YouTubeVideoEmbedOptions(details, config as YouTubeDataTypeConfig);

            return new VideoPickerValue(provider, credentials, details, embed);

        }

        public IProviderConfig ParseConfig(XElement xml) {
            return new YouTubeVideoConfig(xml);
        }

        public IProviderDataTypeConfig ParseDataTypeConfig(JObject obj) {
            return new YouTubeDataTypeConfig(obj);
        }

    }

}