using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Providers;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.VideoPicker.Controllers.Api {

    [PluginController("Skybrud")]
    [JsonOnlyConfiguration]
    public class VideoPickerController : UmbracoApiController {

        [HttpGet]
        public object GetVideoFromUrl(string url) {
            return GetVideoFromSource(url);
        }

        [HttpGet]
        public object GetVideoFromSource(string source) {

            // Validate that we have an URL
            if (string.IsNullOrWhiteSpace(source)) return CreateErrorResponse(HttpStatusCode.BadRequest, "No URL specified");

            foreach (IVideoProvider provider in VideoPickerProviderCollection.Instance) {

                try {

                    // Check whether the provider supports the specified "source"
                    if (!provider.IsMatch(source, out IVideoOptions options)) continue;
                    
                    // Get the video from the options
                    VideoPickerItem video = provider.GetVideo(options);

                    // Return wether the video was found or not
                    return video == null ? CreateErrorResponse(HttpStatusCode.NotFound, "Video not found.") : (object) video;

                } catch (VideoPickerNotFoundException ex) {
                    
                    return CreateErrorResponse(ex.StatusCode, ex.Message);

                } catch (VideoPickerException ex) {
                    
                    return CreateErrorResponse(ex.StatusCode, ex.Message);

                } catch (Exception ex) {
                    
                    LogHelper.Error<VideoPickerController>($"Unable to load video via provider {provider.GetType()} from source: {source}", ex);
                    return CreateErrorResponse("Something went wrong.");

                }

            }

            return CreateErrorResponse(HttpStatusCode.BadRequest, "Unknown URL syntax");

        }
        
        private HttpResponseMessage CreateErrorResponse(string message) {
            return CreateErrorResponse(HttpStatusCode.InternalServerError, message);
        }

        private HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string message) {
            return Request.CreateResponse(JsonMetaResponse.GetError(statusCode, message));
        }

    }

}