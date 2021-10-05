using System;
using Newtonsoft.Json;
using Skybrud.VideoPicker.Models.Config;

namespace Skybrud.VideoPicker.Models.Providers {
    
    /// <summary>
    /// Interface describing a trimmed down version of <see cref="IProviderCredentials"/>.
    ///
    /// The general idea with this interface is that while classes implementing <see cref="IProviderCredentials"/> may
    /// expose sensitive information (eg. API keys or access tokens for the provider), classes implementing
    /// <see cref="IVideoProviderCredentialsDetails"/> should instead be seen as a reference to the crendentials - and
    /// without any sensitive information.
    /// </summary>
    public interface IVideoProviderCredentialsDetails {

        /// <summary>
        /// Gets the unique ID of the credentials.
        /// </summary>
        [JsonProperty("id")]
        Guid Id { get; }

    }

}