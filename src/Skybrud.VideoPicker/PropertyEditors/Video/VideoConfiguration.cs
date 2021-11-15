using Umbraco.Core.PropertyEditors;

namespace Skybrud.VideoPicker.PropertyEditors.Video {
    
    public class VideoConfiguration {

        [ConfigurationField("providers", "Providers", "/App_Plugins/Skybrud.VideoPicker/Views/Editors/VideoProviders.html?v=2", Description = "Configure the various video providers.")]
        public VideoPickerProvidersConfig Providers { get; set; }

    }

}