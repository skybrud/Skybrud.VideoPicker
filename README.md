# Skybrud.VideoPicker

*Skybrud.VideoPicker* adds a property editor and a grid editor to your Umbraco backoffice that let's your users enter the URL of either a YouTube or Vimeo video.

The package will then fetch information about the video (title, description, duration etc.) from the relevant API.

Users will also be able to select a custom thumbnail for the video - or just stick with the default thumbnail from YouTube/Vimeo.

![image](https://user-images.githubusercontent.com/3634580/32501030-364256de-c3d7-11e7-9300-02ce084deaf1.png)

## Links

- <a href="#installation">Installation</a>
- <a href="#examples">Configuration</a>
  - <a href="#youtube">YouTube</a>
  - <a href="#vimeo">Vimeo</a>



## Installation

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**ZIP file**][GitHubRelease]  
Grab a ZIP file of the latest release; unzip and move the contents to the root directory of your web application.

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.VideoPicker/
[GitHubRelease]: https://github.com/skybrud/Skybrud.VideoPicker/releases/latest



## Configuration

### YouTube

In order to get information about YouTube videos, you need to configure a Google server key that is used for accessing the YouTube API. You can do so by adding the following line to the `appSettings` element in your `Web.config`:

```xml
<add key="SkybrudVideoPicker:GoogleServerKey" value="Your Google server key" />
```

### Vimeo

In a similar way, you need to configure Vimeo so we can fetch information about Vimeo videos. This is done by adding your Vimeo OAuth 2.0 access token to the `appSettings` element:

```xml
<add key="SkybrudVideoPicker:VimeoAccessToken" value="Your Vimeo access token" />
```

You can find a list of your existing Vimeo apps [**here**](https://developer.vimeo.com/apps), or create a new app [**here**](https://developer.vimeo.com/apps/new). Once you have created an app, click on it in the list, and then go to the *Authentication* tab/page - here you'll be able to generate a new access token.