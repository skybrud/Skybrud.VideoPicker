using System;
using System.Net;

namespace Skybrud.VideoPicker.Exceptions {
    
    public class VideoPickerException : Exception {
        
        public HttpStatusCode StatusCode { get; }

        public VideoPickerException(string message) : base(message) {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public VideoPickerException(string message, Exception innerException) : base(message, innerException) {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public VideoPickerException(HttpStatusCode statusCode, string message) : base(message) {
            StatusCode = statusCode;
        }

        public VideoPickerException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException) {
            StatusCode = statusCode;
        }

    }

}