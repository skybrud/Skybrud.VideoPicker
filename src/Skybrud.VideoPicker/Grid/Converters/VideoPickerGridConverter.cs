using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;
using Skybrud.VideoPicker.Constants;
using Skybrud.VideoPicker.Grid.Config;
using Skybrud.VideoPicker.Grid.Values;
using Umbraco.Core;

namespace Skybrud.VideoPicker.Grid.Converters {

    public class VideoPickerGridConverter : IGridConverter {

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">A reference to the parent <see cref="GridControl"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted control value.</param>
        public bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = null;
            if (IsVideoPicker(control.Editor)) {
                value = GridControlVideoPickerValue.Parse(control, token as JObject);
            }
            return value != null;
        }

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridEditorConfig"/>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the editor config.</param>
        /// <param name="config">The converted editor config.</param>
        public bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = null;
            if (IsVideoPicker(editor)) {
                config = GridEditorVideoPickerConfig.Parse(editor, token as JObject);
            }
            return config != null;
        }

        /// <summary>
        /// Gets an instance <see cref="GridControlWrapper"/> for the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <param name="wrapper">The wrapper.</param>
        public bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = null;
            if (IsVideoPicker(control.Editor)) {
                wrapper = control.GetControlWrapper<GridControlVideoPickerValue>();
            }
            return wrapper != null;
        }

        private bool IsVideoPicker(GridEditor editor) {
            return (
                CultureInfo.InvariantCulture.CompareInfo.IndexOf(editor.View, VideoPickerConstants.GridEditorView, CompareOptions.IgnoreCase) >= 0
                ||
                editor.Alias.Equals(VideoPickerConstants.GridEditorAlias, StringComparison.InvariantCultureIgnoreCase)
                ||
                CultureInfo.InvariantCulture.CompareInfo.IndexOf(editor.Alias, VideoPickerConstants.GridEditorAlias + ".", CompareOptions.IgnoreCase) >= 0
            );
        }

    }

}