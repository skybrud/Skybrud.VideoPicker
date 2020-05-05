using Umbraco.Core.Composing;

namespace Skybrud.VideoPicker.Providers {
    
    /// <summary>
    /// Provides extension methods to the <see cref="Composition"/> class.
    /// </summary>
    public static class VideoPickerCompositionExensions {

        /// <summary>
        /// Gets the video picker provider collection builder.
        /// </summary>
        /// <param name="composition">The composition.</param>
        public static VideoPickerProviderCollectionBuilder VideoPickerProviders(this Composition composition) {
            return composition.WithCollectionBuilder<VideoPickerProviderCollectionBuilder>();
        }

    }

}