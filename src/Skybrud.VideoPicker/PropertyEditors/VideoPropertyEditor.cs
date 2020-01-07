using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.VideoPicker.PropertyEditors {

    [DataEditor("Skybrud.VideoPicker.Video", EditorType.PropertyValue, "Skybrud VideoPicker Video", "/App_Plugins/Skybrud.VideoPicker/Views/Editors/Video.html", ValueType = ValueTypes.Json, Group = "Skybrud.dk", Icon = "icon-play")]
    public class VideoPropertyEditor : DataEditor {

        public VideoPropertyEditor(ILogger logger) : base(logger) { }

        /// <inheritdoc />
        protected override IConfigurationEditor CreateConfigurationEditor() => new VideoConfigurationEditor();

    }

    public class VideoConfigurationEditor : ConfigurationEditor<VideoConfiguration> {

        public VideoConfigurationEditor() { }

    }

    public class VideoConfiguration { }


}