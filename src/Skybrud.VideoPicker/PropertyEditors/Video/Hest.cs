using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.VideoPicker.PropertyEditors.Video {
    
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

}