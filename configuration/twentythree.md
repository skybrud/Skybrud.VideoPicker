---
order: 23
---

# TwentyThree

The video picker also supports videos from TwentyThree. However the configuration is a bit special, as you're using a custom domain - as a result of this, the configuration should be as following:

```xml
<add key="SkybrudVideoPicker:TwentyThree{videos.yourdomain.com}:ConsumerKey" value="TwentyThree consumer key" />
<add key="SkybrudVideoPicker:TwentyThree{videos.yourdomain.com}:ConsumerSecret" value="TwentyThree consumer secret" />
<add key="SkybrudVideoPicker:TwentyThree{videos.yourdomain.com}:AccessToken" value="TwentyThree access token" />
<add key="SkybrudVideoPicker:TwentyThree{videos.yourdomain.com}:AccessTokenSecret" value="TwentyThree access token secret" />
```

If the parent TwentyThree account allows anonymous access, the access token and access token may be omitted.

Also, as a result of limitions in the Twenty Three API, entered URLs must look like `https://videos.yourdomain.com/manage/video/12345678` instead of `https://videos.yourdomain.com/friendlyVideoName`.