using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers {

    public interface IVideoProvider {

        string Alias { get; }

        string Name { get; }

        bool IsMatch(string source, out IVideoOptions options);

        VideoPickerValue GetVideo(IVideoOptions options);

        VideoPickerValue ParseValue(JObject obj);

    }

}