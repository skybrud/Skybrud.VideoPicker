using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Skybrud.VideoPicker.Exceptions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Services;
using Skybrud.WebApi.Json;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.VideoPicker.Controllers.Api {

    [PluginController("Skybrud")]
    [JsonOnlyConfiguration]
    public class VideoPickerController : UmbracoApiController {

        private readonly ILogger _logger;

        private readonly VideoPickerService _videoPickerService;

        public VideoPickerController(ILogger logger, VideoPickerService service) {
            _logger = logger;
            _videoPickerService = service;
        }

        #region Public API methods

        [HttpGet]
        [HttpPost]
        public object GetVideo() {

            // Get the "source" parameter from either GET or POST
            string source = HttpContext.Current.Request["source"];

            try {

                // Attempt to get a video based on "source"
                VideoPickerValue value = _videoPickerService.GetVideo(source);

                // Return the video details (or 
                return value == null ? Request.CreateResponse(HttpStatusCode.NotFound) : (object) value;

            } catch (VideosException ex) {

                if (ex.Status != HttpStatusCode.NotFound) _logger.Error<VideoPickerController>(ex, "Failed fetching video from {Source}", source);

                return Request.CreateErrorResponse(ex.Status, ex.Message);

            } catch (Exception ex) {

                _logger.Error<VideoPickerController>(ex, "Failed fetching video from {Source}", source);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occured on the server.");

            }

        }

        #endregion

    }

}