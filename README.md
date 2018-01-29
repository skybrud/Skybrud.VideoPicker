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
  - <a href="#grid-editor">Grid editor</a>



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

### Grid editor

The package also supports adding a grid editor - the JSON configuration for the editor could look like this:

```JSON
{
  "name": "Video",
  "alias": "VideoPicker",
  "view": "/App_Plugins/Skybrud.VideoPicker/Views/VideosGridEditor.html",
  "icon": "icon-video",
  "config": {
    "list": {
      "limit": 5,
      "title": {
        "mode": "required"
      }
    },
    "items": {
      "title": {
        "mode": "required"
      },
      "description": {
        "mode": "optional"
      }
    },
    "details": {
      "description": {
        "mode": "hidden"
      }
    }
  }
}
```

Any property within the `config` object can be omitted, in which case the default value will be used instead.

**List**  
Via `config.list.limit`, you can set the maximum amount of videos allowed in the list. If you set the limit to `0` (or don't specify it at all), the list won't enforce any limits. A limit of `1` will instead turn the video picker list into a single video picker.

If you wish to let your editors specify a title for the entire list of videos, you can set `config.list.title` to either `required` or `optional`. Default is `hidden`.

**Items**  
As you might want to overwrite the original title and/or description of a video, you can also control this via the `config.items.title.mode` and `config.items.description.mode` properties. Like with the list title, accepted values are `required`, `optional` and `hidden`. Default is `hidden`.

**Details**  
Some videos may have some long descriptions, which then also takes up a lot of space in the UI. If this is the case, you can use the `config.details.description.visible` property to hide the original description. Acceptable values are `true` and `false`. Default is `false`.

There are currently no options to hide the original title or the duration of the video.
