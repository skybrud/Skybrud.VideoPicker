using Newtonsoft.Json;

namespace Skybrud.VideoPicker.Models.Providers {

    public interface IVideoProviderDetails {

        [JsonProperty("alias")]
        string Alias { get; }

        [JsonProperty("name")]
        string Name { get; }

    }

}