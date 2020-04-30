using System;
using System.Net;

namespace Skybrud.VideoPicker.Exceptions {
    
    public class VideosException : Exception {

        public HttpStatusCode Status { get; }

        public VideosException(string message) : base(message) {
            Status = HttpStatusCode.InternalServerError;
        }

        public VideosException(HttpStatusCode status, string message) : base(message) {
            Status = status;
        }

        public VideosException(string message, HttpStatusCode status) : base(message) {
            Status = status;
        }

    }

}