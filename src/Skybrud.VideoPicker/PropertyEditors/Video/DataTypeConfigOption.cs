using Newtonsoft.Json;

namespace Skybrud.VideoPicker.PropertyEditors.Video {
    
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