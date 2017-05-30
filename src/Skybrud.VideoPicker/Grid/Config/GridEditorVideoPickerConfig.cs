using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Json;

namespace Skybrud.VideoPicker.Grid.Config {
    
    /// <summary>
    /// Class representing the configuration of a video picker.
    /// </summary>
    public class GridEditorVideoPickerConfig : GridJsonObject, IGridEditorConfig {

        #region Properties

        /// <summary>
        /// Gets a reference to the parent editor.
        /// </summary>
        public GridEditor Editor { get; private set; }
            
        #endregion

        #region Constructors

        private GridEditorVideoPickerConfig(GridEditor editor, JObject obj) : base(obj) {
            Editor = editor;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="GridEditorVideoPickerConfig"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="editor">The parent editor.</param>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        public static GridEditorVideoPickerConfig Parse(GridEditor editor, JObject obj) {
            return obj == null ? null : new GridEditorVideoPickerConfig(editor, obj);
        }

        #endregion

    }

}