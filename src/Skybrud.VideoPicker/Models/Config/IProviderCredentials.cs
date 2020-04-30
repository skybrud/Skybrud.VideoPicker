using System;

namespace Skybrud.VideoPicker.Models.Config {
    
    public interface IProviderCredentials {

        /// <summary>
        /// Gets the unique ID of the crendentials.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the friendly name of the credentials.
        /// </summary>
        string Name { get; }

    }

}