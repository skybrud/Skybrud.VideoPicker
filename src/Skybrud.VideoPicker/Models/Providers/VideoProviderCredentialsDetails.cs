using System;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Models.Config;

namespace Skybrud.VideoPicker.Models.Providers {
    
    /// <summary>
    /// Default implementation of the <see cref="IVideoProviderCredentialsDetails"/> interface.
    /// </summary>
    public class VideoProviderCredentialsDetails : IVideoProviderCredentialsDetails {

        #region Properties

        /// <summary>
        /// Gets the unique ID of the credentials.
        /// </summary>
        public Guid Id { get; }

        #endregion

        #region Constructors

        private VideoProviderCredentialsDetails(JObject json) {
            Id = json.GetGuid("id");
        }

        /// <summary>
        /// Initializes a new instance from the specified <paramref name="credentials"/>.
        /// </summary>
        /// <param name="credentials">The full crendentials model this instance should be based on.</param>
        public VideoProviderCredentialsDetails(IProviderCredentials credentials) {
            Id = credentials.Id;
        }

        #endregion

        #region Static methods
        
        /// <summary>
        /// Parses the specified <paramref name="json"/> into an instance of <see cref="VideoProviderCredentialsDetails"/>.
        /// </summary>
        /// <param name="json">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoProviderCredentialsDetails"/>.</returns>
        public static VideoProviderCredentialsDetails Parse(JObject json) {
            return json == null ? null : new VideoProviderCredentialsDetails(json);
        }

        #endregion

    }

}