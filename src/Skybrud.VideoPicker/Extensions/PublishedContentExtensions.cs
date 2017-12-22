using System.Linq;
using Skybrud.VideoPicker.Models;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.VideoPicker.Extensions {
    
    /// <summary>
    /// Various extension methods for <see cref="IPublishedContent"/> and the image picker.
    /// </summary>
    public static class PublishedContentExtensions {
        
        /// <summary>
        /// Gets the first item from an <see cref="VideoPickerList"/> model from the property with the specified
        /// <code>propertyAlias</code>. If property isn't an image picker (or the list is empty), an empty item will be
        /// returned instead.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> to read the property from.</param>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="VideoPickerItem"/>.</returns>
        public static VideoPickerItem GetVideoPickerItem(this IPublishedContent content, string propertyAlias) {
            VideoPickerList list = content.GetPropertyValue(propertyAlias) as VideoPickerList;
            VideoPickerItem item = list?.Items.FirstOrDefault();
            return item ?? new VideoPickerItem();
        }

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerList"/> from the property with the specified <paramref name="propertyAlias"/>.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> to read the property from.</param>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="VideoPickerList"/>.</returns>
        public static VideoPickerList GetVideoPickerList(this IPublishedContent content, string propertyAlias) {
            return content?.GetPropertyValue<VideoPickerList>(propertyAlias) ?? new VideoPickerList();
        }

    }

}