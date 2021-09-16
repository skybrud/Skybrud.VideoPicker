using Newtonsoft.Json;

namespace Skybrud.VideoPicker.Models.Providers {

    public interface IVideoProviderDetails {

        [JsonProperty("alias", Order = -99)]
        string Alias { get; }

        [JsonProperty("name", Order = -98)]
        string Name { get; }

    }

}