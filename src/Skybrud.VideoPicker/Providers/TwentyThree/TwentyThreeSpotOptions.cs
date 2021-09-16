using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.TwentyThree {
    
    public class TwentyThreeSpotOptions : IVideoOptions {
        
        public string Scheme { get; }
        
        public string Domain { get; }
        
        public string SpotId { get; }
        
        public string Token { get; }

        public TwentyThreeSpotOptions(string scheme, string domain, string spotId, string token) {
            Scheme = scheme;
            Domain = domain;
            SpotId = spotId;
            Token = token;
        }

    }

}