# Vimeo

In order for the package to access the Vimeo API, you should specify an access token in the `appSettings` area of your `Web.config` file:

```xml
<add key="SkybrudVideoPicker:VimeoAccessToken" value="Your Vimeo access token" />
```

You can find a list of your existing Vimeo apps [**here**](https://developer.vimeo.com/apps), or create a new app [**here**](https://developer.vimeo.com/apps/new). Once you have created an app, click on it in the list, and then go to the *Authentication* tab/page - here you'll be able to generate a new access token.