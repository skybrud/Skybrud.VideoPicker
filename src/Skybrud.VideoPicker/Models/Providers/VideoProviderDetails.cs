using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Providers;

namespace Skybrud.VideoPicker.Models.Providers {

    public class VideoProviderDetails : IVideoProviderDetails {

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("name")]
        public string Name { get; }

        #region Constructors

        public VideoProviderDetails(string alias, string name) {
            Alias = alias;
            Name = name;
        }

        protected VideoProviderDetails(JObject obj) {
            Alias = obj.GetString("alias");
            Name = obj.GetString("name");
        }

        public VideoProviderDetails(IVideoProvider provider) {
            Alias = provider.Alias;
            Name = provider.Name;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="VideoProviderDetails"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoProviderDetails"/>.</returns>
        public static VideoProviderDetails Parse(JObject obj) {
            return obj == null ? null : new VideoProviderDetails(obj );
        }

        #endregion

    }

}