using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.VideoPicker.Models;

namespace Skybrud.VideoPicker.Grid.Values {

    public class GridControlVideoPickerValue : VideoPickerList, IGridControlValue {

        #region Properties

        [JsonIgnore]
        public GridControl Control { get; private set; }

        #endregion

        #region Constructors

        protected GridControlVideoPickerValue(GridControl control, JObject obj) : base(obj) {
            Control = control;
        }

        #endregion

        #region Member methods

        public string GetSearchableText() {

			StringBuilder combined = new StringBuilder();

	        combined.AppendLine(Title);

	        foreach (VideoPickerItem item in Items)
	        {
		        combined.AppendLine(item.Title);
		        combined.AppendLine(item.Description);
		        combined.AppendLine(item.Details.Title);
		        combined.AppendLine(item.Details.Description);
			}

	        return combined.ToString();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="GridControlVideoPickerValue"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="control">The parent control.</param>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        public static GridControlVideoPickerValue Parse(GridControl control, JObject obj) {
            return control == null ? null : new GridControlVideoPickerValue(control, obj ?? new JObject());
        }

        #endregion

    }

}