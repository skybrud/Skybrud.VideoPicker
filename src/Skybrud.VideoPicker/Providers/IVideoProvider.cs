using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.PropertyEditors;
using Skybrud.VideoPicker.Services;

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

        /// <summary>
        /// Gets the URL for the HTML view used for editing the configuration for this editor. Returns <c>null</c> if
        /// there isn't any configuration for this provider.
        /// </summary>
        string ConfigView { get; }

        /// <summary>
        /// Gets the URL for the HTML view used for editing the embed options for the video. Returns <c>null</c> if
        /// this provider doesn't support editing the embed options.
        /// </summary>
        string EmbedView { get; }

        /// <summary>
        /// Returns whether the specified <paramref name="source"/> is recognized by this provider. If <c>true</c>, the
        /// <paramref name="options"/> parameter will contain an instance of <see cref="IVideoOptions"/> with the ID
        /// and/or other identifiers of the video.
        /// </summary>
        /// <param name="service">The current <see cref="VideoPickerService"/> instance.</param>
        /// <param name="source">The source - typically either a URL to the video or an HTML embed code.</param>
        /// <param name="options">An instance of <see cref="IVideoOptions"/>.</param>
        /// <returns><c>true</c> if this provider recognizes the specified <paramref name="source"/>; otherwise <c>false</c>.</returns>
        bool IsMatch(VideoPickerService service, string source, out IVideoOptions options);

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerValue"/> with information about the video matching the specified
        /// <paramref name="options"/>.
        ///
        /// This method may return <c>null</c> if the <paramref name="options"/> is of a type not recognized by this
        /// providers. The provider's implementation of this method will also most likely request information about the
        /// video from a third party API, and may therefore fail if the video isn't found or another error occurs during
        /// this request.
        /// </summary>
        /// <param name="service">The current <see cref="VideoPickerService"/> instance.</param>
        /// <param name="options">The options identifying the video.</param>
        /// <returns>An instance of <see cref="VideoPickerValue"/>.</returns>
        VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options);

        /// <summary>
        /// Parses the specified <paramref name="json"/> object into an instance of <see cref="VideoPickerValue"/>.
        /// </summary>
        /// <param name="json">An instance of <see cref="JObject"/> representing the video value.</param>
        /// <param name="config">The provider specific configuration from the datatype.</param>
        /// <returns>An instance of <see cref="VideoPickerValue"/>.</returns>
        VideoPickerValue ParseValue(JObject json, IProviderDataTypeConfig config);

        /// <summary>
        /// Parses the specified <paramref name="xml"/> configuration for this provider.
        /// </summary>
        /// <param name="xml">An instance of <see cref="XElement"/> representing the configuration specific to this provider.</param>
        /// <returns>An instance of <see cref="IProviderConfig"/>, or <c>null</c> if the provider doesn't have a configuration.</returns>
        IProviderConfig ParseConfig(XElement xml);

        IProviderDataTypeConfig ParseDataTypeConfig(JObject obj);

    }

}