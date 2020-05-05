using Umbraco.Core.Composing;

namespace Skybrud.VideoPicker.Providers {
    
    public class VideoPickerProviderCollectionBuilder : OrderedCollectionBuilderBase<VideoPickerProviderCollectionBuilder, VideoPickerProviderCollection, IVideoProvider> {
        
        protected override VideoPickerProviderCollectionBuilder This => this;

    }

}