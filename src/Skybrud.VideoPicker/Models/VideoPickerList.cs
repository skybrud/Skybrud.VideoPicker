using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerList : JsonObjectBase {

        #region Properties

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("items")]
        public VideoPickerItem[] Items { get; private set; }

        [JsonIgnore]
        public bool IsValid {
            get { return Items.Any(x => x.IsValid); }
        }

        #endregion

        #region Constructors

        protected VideoPickerList(JObject obj) : base(obj) {
            Title = obj.GetString("title");
            Items = obj.GetArrayItems("items", VideoPickerItem.Parse);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerList"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoPickerList"/>.</returns>
        public static VideoPickerList Parse(JObject obj) {
            return obj == null ? null : new VideoPickerList(obj );
        }

        #endregion

    }

}