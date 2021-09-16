namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoEmbedOptions {

        public string VideoId { get; set; }
        
        public string Color { get; set; }

        public bool? AutoPause { get; set; }

        public bool? AutoPlay { get; set; }

        /// <summary>
        /// Gets whether the video should play again and again.
        /// </summary>
        public bool? Loop { get; set; }

        public bool? ShowTitle { get; set; }

        public bool? ShowByLine { get; set; }

        public bool? ShowPortrait { get; set; }

        public bool? ShowControls { get; set; }

        public bool? DoNotTrack { get; set; }

        public bool? IsMuted { get; set; }

        public VimeoEmbedOptions() { }

        public VimeoEmbedOptions(string videoId) {
            VideoId = videoId;
        }

    }

}