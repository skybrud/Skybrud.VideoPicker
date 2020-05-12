using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.VideoPicker.Providers {
    
    public class VideoPickerProviderCollection : BuilderCollectionBase<IVideoProvider> {

        /// <summary>
        /// Gets the current provider collection.
        /// </summary>
        public static VideoPickerProviderCollection Current => global::Umbraco.Core.Composing.Current.Factory.GetInstance<VideoPickerProviderCollection>();

        /// <summary>
        /// Initializes a new provider collectio based on the specified <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to make up the collection.</param>
        public VideoPickerProviderCollection(IEnumerable<IVideoProvider> items) : base(items) { }

        public bool TryGet(string alias, out IVideoProvider provider) {
            provider = this.FirstOrDefault(x => x.Alias == alias);
            return provider != null;
        }

    }

}