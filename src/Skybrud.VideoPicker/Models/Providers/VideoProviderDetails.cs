namespace Skybrud.VideoPicker.Models.Providers {

    public class VideoProviderDetails : IVideoProviderDetails {

        public string Alias { get; }

        public string Name { get; }

        public VideoProviderDetails(string alias, string name) {
            Alias = alias;
            Name = name;
        }

    }

}