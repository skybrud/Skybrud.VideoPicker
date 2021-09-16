using System;
using System.Net;

namespace Skybrud.VideoPicker.Exceptions {
    
    public class VideoPickerNotFoundException : VideoPickerException {

        public VideoPickerNotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }

        public VideoPickerNotFoundException(string message, Exception innerException) : base(HttpStatusCode.NotFound, message, innerException) { }

    }

}