using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.TwentyThree {
    
    public class TwentyThreeVideoOptions : IVideoOptions {
        
        public string Scheme { get; }
        
        public string Domain { get; }
        
        public string VideoId { get; }
        
        public string Token { get; }
        
        public string PlayerId { get; }

        public TwentyThreeVideoOptions(string scheme, string domain, string videoId, string token, string playerId) {
            Scheme = scheme;
            Domain = domain;
            VideoId = videoId;
            Token = token;
            PlayerId = playerId;
        }

    }

}