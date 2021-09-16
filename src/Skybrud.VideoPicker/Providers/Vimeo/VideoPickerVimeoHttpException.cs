using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Http;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Umbraco.Core.Logging;

namespace Skybrud.VideoPicker.Providers.Vimeo {

    public class VideoPickerVimeoHttpException : Exception {

        #region Properties

        /// <summary>
        /// Gets a reference to the underlying <see cref="IHttpResponse"/>.
        /// </summary>
        public IHttpResponse Response { get; }

        /// <summary>
        /// Gets the HTTP status code returned by the Vimeo API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the error message returned by the Vimeo API.
        /// </summary>
        public string Error { get; }

        #endregion

        #region Constructors

        public VideoPickerVimeoHttpException(IHttpResponse response) : base("Invalid response received from the Vimeo API (Status: " + ((int)response.StatusCode) + ")") {
            
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

}