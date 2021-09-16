using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers {
    
    public interface IVideoProvider {

        /// <summary>
        /// Gets the alias of the provider - eg. <c>youtube</c> or <c>vimeo</c>. 
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets the friendly name of the provider - eg. <c>YouTube</c> or <c>Vimeo</c>.
        /// </summary>
        string Name { get; }
        
        bool IsMatch(string source, out IVideoOptions options);
        
        VideoPickerItem GetVideo(IVideoOptions options);

    }

}