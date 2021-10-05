# Skybrud.VideoPicker

*Skybrud.VideoPicker* adds a property editor to your Umbraco backoffice that let's your users enter the URL of either a YouTube or Vimeo video.

![image](https://user-images.githubusercontent.com/3634580/136101139-7f79fc23-e2e1-4aa6-b74f-dfe63ffbbea1.png)


## Links

- <a href="#installation">Installation</a>
- <a href="#configuration">Configuration</a>
  - <a href="#youtube">YouTube</a>
  - <a href="#vimeo">Vimeo</a>



## Installation

**Umbraco 8**

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**ZIP file**][GitHubRelease]  
Grab a ZIP file of the latest release; unzip and move the contents to the root directory of your web application.

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.VideoPicker/
[GitHubRelease]: https://github.com/skybrud/Skybrud.VideoPicker/releases/latest

**Umbraco 7**

See the [**v1/main** branch](https://github.com/skybrud/Skybrud.VideoPicker/tree/v1/main).

## Providers

This package works around a list of providers, where each providr is the implementation for a video service. By default the package contains providers for YouTube, Vimeo and DreamBroker, but also allows you to implement your own provider for other video services.

## Configuration

The package and the individual providers can be configured through the `~/Config/Skybrud.VideoPicker.config` file. The content looks something like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<settings>
  <providers>
    <youtube>...</youtube>
    <vimeo>...</vimeo>
  </providers>
</settings>
```

Each provider may have some service specific settings - most primarily one or more `<credentials />` that contain information on how to access a specific service. You can read more about this below.

### YouTube

In order to get information about YouTube videos, you need to configure a Google server key that is used for accessing the YouTube API. You can do so by adding a `<youtube>` element along with a `<credentials>` child element like in the example below:

```xml
<youtube>
    <credentials
      id="A valid GUID"
      name="A friendly name"
      description=""
      serverKey="" />
  </youtube>
```

### Vimeo

In a similar way, you need to configure Vimeo so we can fetch information about Vimeo videos. This is done by adding your Vimeo OAuth 2.0 access token - eg. as shown below:

```xml
<vimeo>
  <credentials
    id="A valid GUID"
    name="A friendly name"
    description=""
    accessToken="" />
</vimeo>
```

You can find a list of your existing Vimeo apps [**here**](https://developer.vimeo.com/apps), or create a new app [**here**](https://developer.vimeo.com/apps/new). Once you have created an app, click on it in the list, and then go to the *Authentication* tab/page - here you'll be able to generate a new access token.

### DreamBroker

The package also supports DreamBroker out of the box. The implementation uses OEmbed instead of an API, so there is no need to configere the DreamBroker provider.
