using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.VideoPicker.PropertyEditors {

    [DataEditor("Skybrud.VideoPicker.Video", EditorType.PropertyValue, "Skybrud VideoPicker Video", "/App_Plugins/Skybrud.VideoPicker/Views/Editors/Video.html?v=2", ValueType = ValueTypes.Json, Group = "Skybrud.dk", Icon = "icon-play")]
    public class VideoPropertyEditor : DataEditor {

        public VideoPropertyEditor(ILogger logger) : base(logger) { }

        /// <inheritdoc />
        protected override IConfigurationEditor CreateConfigurationEditor() => new VideoConfigurationEditor();

    }

    public class VideoConfigurationEditor : ConfigurationEditor<VideoConfiguration> {

        public VideoConfigurationEditor() { }

    }

    public class VideoConfiguration {

        [ConfigurationField("providers", "Providers", "/App_Plugins/Skybrud.VideoPicker/Views/Editors/VideoProviders.html?v=2", Description = "Configure the various video providers.")]
        public VideoPickerProvidersConfig Providers { get; set; }

    }

    [JsonConverter(typeof(Hest))]
    public class VideoPickerProvidersConfig : IEnumerable<IProviderDataTypeConfig> {

        [JsonIgnore]
        public JObject Json { get; }

        public ReadOnlyDictionary<string, IProviderDataTypeConfig> Providers { get; }

        public VideoPickerProvidersConfig(VideoPickerService service, JObject obj) {

            Json = obj;

            Dictionary<string, IProviderDataTypeConfig> temp = new Dictionary<string, IProviderDataTypeConfig>();

            foreach (JProperty property in obj.Properties()) {

                if (property.Value is JObject value) {

                    if (service.Providers.TryGet(property.Name, out IVideoProvider provider)) {
                        temp.Add(property.Name, provider.ParseDataTypeConfig(value));
                    } else {
                        temp.Add(property.Name, new DefaultProviderDataTypeConfig(value));
                    }

                }

            }

            Providers = new ReadOnlyDictionary<string, IProviderDataTypeConfig>(temp);

        }

        public bool TryGet(IVideoProvider provider, out IProviderDataTypeConfig config) {
            return Providers.TryGetValue(provider.Alias, out config);
        }

        public bool TryGet(string providerAlias, out IProviderDataTypeConfig config) {
            return Providers.TryGetValue(providerAlias, out config);
        }

        public IEnumerator<IProviderDataTypeConfig> GetEnumerator() {
            return Providers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

    public class Hest : JsonConverter {
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            if (value is VideoPickerProvidersConfig providers) {

                JObject obj = new JObject();

                foreach (var pair in providers.Providers) {

                    obj.Add(pair.Key, JObject.FromObject(pair.Value, serializer));

                }

                obj.WriteTo(writer);

            }

            if (value is DefaultProviderDataTypeConfig config) {

                config.Value.WriteTo(writer);

            }

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

            if (reader.TokenType == JsonToken.Null) return null;

            switch (reader.TokenType) {

                case JsonToken.Null:
                    return null;

                case JsonToken.StartObject:
                    JObject obj = serializer.Deserialize<JObject>(reader);

                    VideoPickerService service = Current.Factory.GetInstance<VideoPickerService>();

                    return new VideoPickerProvidersConfig(service, obj);
                    

            }

            throw new Exception( reader.TokenType + " => " + reader.Value + " => " + reader.ValueType + "" );

        }

        public override bool CanConvert(Type objectType) {
            return false;
        }

    }

    public interface IProviderDataTypeConfig {

        bool IsEnabled { get; }

    }

    [JsonConverter(typeof(Hest))]
    public class DefaultProviderDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonIgnore]
        public JObject Value { get; }

        public DefaultProviderDataTypeConfig(JObject value) {
            Value = value;
            IsEnabled = value.GetBoolean("enabled");
        }

    }

    public class DataTypeConfigOption<T> {

        [JsonProperty("readonly")]
        public bool IsReadonly { get; set; }

        [JsonProperty("value")]
        public T Value { get; set; }

        public DataTypeConfigOption() { }

        public DataTypeConfigOption(T value) {
            IsReadonly = true;
            Value = value;
        }

    }


}