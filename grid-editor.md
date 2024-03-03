---
order: 3
---

# Grid editor

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
        "visible": false
      }
    }
  }
}
```

Any property within the `config` object can be omitted, in which case the default value will be used instead.

## List 

Via `config.list.limit`, you can set the maximum amount of videos allowed in the list. If you set the limit to `0` (or don't specify it at all), the list won't enforce any limits. A limit of `1` will instead turn the video picker list into a single video picker.

If you wish to let your editors specify a title for the entire list of videos, you can set `config.list.title` to either `required` or `optional`. Default is `hidden`.

## Items

As you might want to overwrite the original title and/or description of a video, you can also control this via the `config.items.title.mode` and `config.items.description.mode` properties. Like with the list title, accepted values are `required`, `optional` and `hidden`. Default is `hidden`.

## Details  

Some videos may have some long descriptions, which then also takes up a lot of space in the UI. If this is the case, you can use the `config.details.description.visible` property to hide the original description. Acceptable values are `true` and `false`. Default is `false`.

There are currently no options to hide the original title or the duration of the video.