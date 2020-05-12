﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.PropertyEditors;

namespace Skybrud.VideoPicker.Providers.DreamBroker {

    public class DreamBrokerDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonProperty("consent")]
        public DataTypeConfigOption<bool> RequireConsent { get; }

        public DreamBrokerDataTypeConfig() {
            IsEnabled = false;
            RequireConsent = new DataTypeConfigOption<bool>(false);
        }

        public DreamBrokerDataTypeConfig(JObject value) {
            IsEnabled = value.GetBoolean("enabled");
            RequireConsent = new DataTypeConfigOption<bool>(value.GetBoolean("consent.value"));
        }

    }

}