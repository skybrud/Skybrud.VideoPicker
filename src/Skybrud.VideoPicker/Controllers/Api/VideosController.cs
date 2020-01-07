using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Providers.DreamBroker;
using Skybrud.WebApi.Json;
using Umbraco.Web.WebApi;

namespace Skybrud.VideoPicker.Controllers.Api {

    //[PluginController("Skybrud")]
    [JsonOnlyConfiguration]
    public class VideosController : UmbracoApiController {

        private readonly VideoService _videoService;

        public VideosController() {
            _videoService = new VideoService();
        }

        #region Public API methods

        [HttpGet]
        [HttpPost]
        public object GetVideo() {

            string source = HttpContext.Current.Request["source"];

            VideoPickerValue value = null;

            foreach (IVideoProvider provider in _videoService.Providers) {
                if (provider.IsMatch(source, out IVideoOptions options)) {
                    value = provider.GetVideo(options);
                    break;
                }
            }

            return value == null ? Request.CreateResponse(HttpStatusCode.NotFound) : (object) value;

        }

        #endregion

    }

    public class VideoService {

        public List<IVideoProvider> Providers { get; }

        public VideoService() {
            Providers = new List<IVideoProvider> {
                new DreamBrokerVideoProvider()
            };
        }

    }

}