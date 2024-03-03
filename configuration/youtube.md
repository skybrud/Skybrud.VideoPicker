# YouTube

When you enter a URL for a YouTube video, the package will automatically recognize it, an request information about the video via the YouTube API. In order to access the API you'll need to configure a Google server key.

The server key can be configured by adding the following line to the `appSettings` element in your `Web.config`:

```xml
<add key="SkybrudVideoPicker:GoogleServerKey" value="Your Google server key" />
```