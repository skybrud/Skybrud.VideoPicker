angular.module("umbraco").directive("skyVideo", function ($http, $timeout, userService, entityResource, notificationsService, mediaHelper) {
    return {
        scope: {
            value: "="
        },
        transclude: true,
        restrict: "E",
        replace: true,
        templateUrl: "/App_Plugins/Skybrud.VideoPicker/Views/Directives/Video.html",
        link: function (scope, element) {

            var input = element[0].querySelector("label input");

            var textarea = element[0].querySelector("label textarea");






            // Image picker related
            var startNodeId = null;

            scope.mode = scope.value && scope.value.embed ? "embed" : "url";
            
            scope.loading = false;
            scope.thumbnail = null;

            function init() {

                scope.config = {
                    services: {
                        youtube: true,
                        vimeo: true
                    }
                };

                // Get the start node of the user (for the thumbnail picker)
                userService.getCurrentUser().then(function (userData) {
                    startNodeId = userData.startMediaId;
                });

                if (scope.value && scope.value.thumbnailId) {
                    entityResource.getById(scope.value.thumbnailId, "media").then(function (data) {
                        if (!data.image) data.image = mediaHelper.resolveFileFromEntity(data);
                        scope.thumbnail = data;
                        scope.thumbnailUrl = scope.thumbnail ? scope.thumbnail.image + "?width=320&height=180&mode=crop" : null;
                    });
                }

                hest(scope.value);

            }

            function hest(item) {

                if (!item) return;

                switch (item.type) {

                    case "vimeo":
                        scope.typeName = "Vimeo";
                        item.details.$thumbnail = item.details.thumbnails[0];
                        break;

                    case "youtube":
                        scope.typeName = "YouTube";
                        item.details.$thumbnail = item.details.thumbnails[0];
                        break;

                    case "twentythree":
                        scope.typeName = "Twenty Three";
                        item.details.$thumbnail = _.findWhere(item.details.thumbnails, { alias: "portrait" });
                        break;

                    default:
                        scope.typeName = "Ukendt";
                        if (item.details && item.detailsitem.details.thumbnails) item.details.$thumbnail = item.details.thumbnails[0];
                        break;

                }

            }

            function fromEmbed(item, html) {

                if (!html) html = item.embed;

                console.log("html");
                console.log(html);

                var m = html.match(/:\/\/(.+?)\/(v|[0-9]+)\.ihtml\/player\.html\?token=([a-z0-9]+)&source=embed&photo%5fid=([0-9]+)/);

                if (!m) {
                    delete item.type;
                    delete item.details;
                    return;
                }

                var domain = m[1];
                var player = m[2];
                var token = m[3];
                var video = m[4];

                scope.loading = true;

                $http.get("/umbraco/Skybrud/VideoPicker/GetTwentyThreeVideo?domain=" + domain + "&token=" + token + "&player=" + player + "&video=" + video).success(function (video) {

                    scope.loading = false;

                    item.url = video.url;
                    item.type = video.type;
                    item.details = video.details;
                    hest(item);

                }).error(function (r) {

                    scope.loading = false;

                    item.type = null;
                    item.details = null;

                    if (r && r.meta && r.meta.error) {
                        notificationsService.error("VideoPicker", r.meta.error);
                    } else {
                        notificationsService.error("VideoPicker", "Failed retrieving the specified video.");
                    }

                });

            }

            function fromUrl(item) {

                scope.loading = true;

                $http.get("/umbraco/Skybrud/VideoPicker/GetVideoFromUrl?url=" + item.url).success(function (video) {

                    if (video.type === "youtube" && scope.config.services.youtube === false) {

                        item.error = "Videos from YouTube is not permitted for this picker.";

                        item.type = null;
                        item.details = null;

                    } else if (video.type === "vimeo" && scope.config.services.vimeo === false) {

                        item.error = "Videos from Vimeo is not permitted for this picker.";

                        item.type = null;
                        item.details = null;

                    } else if (video.type === "twentythree" && scope.config.services.twentythree === false) {

                        item.error = "Videos from Vimeo is not permitted for this picker.";

                        item.type = null;
                        item.details = null;

                    } else {

                        item.url = video.url;
                        item.type = video.type;
                        item.details = video.details;
                        hest(item);

                    }

                    scope.loading = false;

                }).error(function (r) {

                    scope.loading = false;

                    item.type = null;
                    item.details = null;

                    if (r && r.meta && r.meta.error) {
                        notificationsService.error("VideoPicker", r.meta.error);
                    } else {
                        notificationsService.error("VideoPicker", "Failed retrieving the specified video.");
                    }

                });

            }

            scope.update = function () {

                if (scope.mode === "embed") {

                    var m = scope.value.embed ? scope.value.embed.match("\<iframe") : null;

                    // Revert to URL mode if the embed value is empty
                    if (!m) {
                        scope.value.url = "";
                        delete scope.value.type;
                        delete scope.value.details;
                        scope.mode = "url";

                        // Add a little timeout so Angular has time to update first
                        $timeout(function () {
                            input.focus();
                            console.log("Focus on input");
                        }, 10);

                        return;

                    }

                    fromEmbed(scope.value);
                    
                    return;

                }

                var m0 = scope.value.url.match("\<iframe");
                var m1 = scope.value.url.match("vimeo.com/([0-9]+)$");
                var m2 = scope.value.url.match("youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
                var m3 = scope.value.url.match("/manage/video/([0-9]+)$");

                if (m0) {

                    scope.value.embed = scope.value.url;
                    scope.value.url = "";
                    scope.mode = "embed";

                    // Add a little timeout so Angular has time to update first
                    $timeout(function () {
                        textarea.focus();
                        console.log("Focus on textarea");
                    }, 10);

                    fromEmbed(scope.value);
                    return;
                }

                delete scope.value.embed;

                if (!m1 && !m2 && !m3) {
                    delete scope.value.type;
                    delete scope.value.details;
                    return;
                }

                fromUrl(scope.value);

            };

            scope.value.$update = function () {
                scope.update();
            };

            scope.thumbnail = null;
            scope.thumbnailUrl = null;

            scope.addThumbnail = function () {
                scope.mediaPickerOverlay = {
                    view: "mediapicker",
                    title: "Select media",
                    startNodeId: startNodeId,
                    multiPicker: false,
                    onlyImages: true,
                    disableFolderSelect: true,
                    show: true,
                    submit: function (model) {

                        var data = model.selectedImages[0];

                        scope.removeThumbnail();

                        $timeout(function () {
                            scope.value.thumbnailId = data.id;
                            scope.thumbnail = data;
                            scope.thumbnailUrl = scope.thumbnail ? scope.thumbnail.image + "?width=320&height=180&mode=crop" : null;

                            scope.mediaPickerOverlay.show = false;
                            scope.mediaPickerOverlay = null;
                        }, 10);

                    }
                };
            };

            scope.removeThumbnail = function () {
                scope.value.thumbnailId = 0;
                scope.thumbnail = null;
                scope.thumbnailUrl = null;
            };

            init();

        }
    };
});
