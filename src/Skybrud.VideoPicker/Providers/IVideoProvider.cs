using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers {

    public interface IVideoProvider {

        string Alias { get; }

        string Name { get; }

        bool IsMatch(VideoPickerConfig config, string source, out IVideoOptions options);

        VideoPickerValue GetVideo(VideoPickerConfig config, IVideoOptions options);

        VideoPickerValue ParseValue(JObject obj);

        IProviderConfig ParseConfig(XElement xml);

    }

}