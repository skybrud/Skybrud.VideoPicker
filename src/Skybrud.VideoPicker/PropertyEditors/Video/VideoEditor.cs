using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.VideoPicker.PropertyEditors.Video {

    [DataEditor(EditorAlias, EditorType.PropertyValue, EditorName, EditorView, ValueType = ValueTypes.Json, Group = EditorGroup, Icon = EditorIcon)]
    public class VideoEditor : DataEditor {

        #region Constants
        
        internal const string EditorGroup = "Skybrud.dk";

        internal const string EditorIcon = "icon-play";

        public const string EditorAlias = "Skybrud.VideoPicker.Video";

        internal const string EditorName = "Skybrud VideoPicker Video";

        internal const string EditorView = "/App_Plugins/Skybrud.VideoPicker/Views/Editors/Video.html";

        #endregion

        #region Constructors

        public VideoEditor(ILogger logger) : base(logger) { }
        
        #endregion

        #region Member methods

        /// <inheritdoc />
        protected override IConfigurationEditor CreateConfigurationEditor() => new VideoConfigurationEditor();

        #endregion

    }

}