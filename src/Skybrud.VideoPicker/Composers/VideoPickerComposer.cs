using Skybrud.VideoPicker.Controllers.Api;
using Skybrud.VideoPicker.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.VideoPicker.Composers {

    public class VideoPickerComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Register<VideoService, VideoService>();
        }

    }

}