using Skybrud.Umbraco.GridData;
using Skybrud.VideoPicker.Grid.Converters;
using Umbraco.Core;

namespace Skybrud.VideoPicker {

public class Startup : ApplicationEventHandler {

    protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
        GridContext.Current.Converters.Add(new VideoPickerGridConverter());
    }

}

}