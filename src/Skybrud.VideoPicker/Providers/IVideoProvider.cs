using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.Providers {

    public interface IVideoProvider {

        string Alias { get; }

        string Name { get; }

        bool IsMatch(VideoPickerService service, string source, out IVideoOptions options);

        VideoPickerValue GetVideo(VideoPickerService service, IVideoOptions options);

        VideoPickerValue ParseValue(JObject obj);

        IProviderConfig ParseConfig(XElement xml);

    }

}