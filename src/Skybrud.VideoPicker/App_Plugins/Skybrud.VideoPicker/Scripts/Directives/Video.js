angular.module("umbraco").directive("skyVideo", function ($http, $timeout, userService, entityResource, notificationsService, mediaHelper, localizationService) {
    return {
        scope: {
            value: "=",
            config: "=?"
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

            scope.originalThumbnail = null;

            scope.labels = {
                second: "second",
                seconds: "seconds"
            };

            function initConfig() {

                if (!scope.config) scope.config = {};

                var c = scope.config;

                if (!c.services) c.services = {};
                c.services.youtube = c.services.youtube !== false;
                c.services.vimeo = c.services.vimeo !== false;
                c.services.twentythree = c.services.twentythree !== false;

                if (!c.title) c.title = {};
                if (!c.title.mode) c.title.mode = "hidden";
                c.title.visible = c.title.mode !== "hidden";
                c.title.required = c.title.mode === "required";

                if (!c.description) c.description = {};
                if (!c.description.mode) c.description.mode = "hidden";
                c.description.visible = c.description.mode !== "hidden";
                c.description.required = c.description.mode === "required";

                if (!c.details.description) c.details.description = {};
                if (c.details.description.visible !== false) c.details.description.visible = true;


            }

            function init() {

                // Get the start node of the user (for the thumbnail picker)
                userService.getCurrentUser().then(function (userData) {
                    startNodeId = userData.startMediaId;
                });

                localizationService.localizeMany(["skyVideoPicker_second", "skyVideoPicker_seconds"]).then(function (data) {

                    // Update the labels from the response
                    scope.labels.second = data[0];
                    scope.labels.seconds = data[1];

                    // Continue the rest of the initialization
                    init2();

                });

            }

            function init2() {

                if (scope.value && scope.value.thumbnailId) {
                    entityResource.getById(scope.value.thumbnailId, "media").then(function (data) {
                        if (!data.image) data.image = mediaHelper.resolveFileFromEntity(data);
                        scope.thumbnail = data;
                        scope.thumbnailUrl = scope.thumbnail ? scope.thumbnail.image + "?width=320&height=180&mode=crop" : null;
                    });
                }

                // Add "provider" for legacy values
                if (scope.value && scope.value.type && !scope.value.provider) {
                    switch (scope.value.type) {
                    case "vimeo":
                        scope.value.provider = { alias: "vimeo", name: "Vimeo" };
                        break;
                    case "youtube":
                        scope.value.provider = { alias: "youtube", name: "YouTube" };
                        break;
                    case "twentythree":
                        scope.value.provider = { alias: "twentythree", name: "TwentyThree" };
                        break;
                    default:
                        scope.value.provider = { alias: scope.value.type, name: scope.value.type };
                        break;
                    }
                }

                hest(scope.value);

            }

            function hest(item) {

                scope.duration = null;
                scope.originalThumbnail = null;

                scope.thumbnailWidth = 320;
                scope.thumbnailHeight = 180;

                if (!item || !item.details) return;

                item.details.$thumbnail = null;

                // Get user friendly duration
                if (item.details.duration === 1) {
                    scope.duration = "1 " + scope.labels.second;
                } else if (item.details.duration < 60) {
                    scope.duration = item.details.duration + " " + scope.labels.seconds;
                } else {
                    var seconds = item.details.duration;
                    var hours = Math.floor(seconds / 60 / 60);
                    seconds = seconds - (hours * 60 * 60);
                    var minutes = Math.floor(seconds / 60);
                    seconds = seconds - (minutes * 60);
                    scope.duration = [];
                    if (hours > 0) scope.duration.push(hours);
                    if (hours > 0 || minutes > 0) scope.duration.push(minutes);
                    scope.duration.push(seconds);
                    scope.duration = scope.duration.join(":");
                }

                switch (item.type) {

                    case "vimeo":
                        var t = item.details.thumbnails.length > 1 ? item.details.thumbnails[1] : null;
                        if (t) {
                            item.details.$thumbnail = scope.originalThumbnail = t;
                            scope.originalThumbnail = {
                                width: 320,
                                height: 180,
                                url: t.url.replace("200x150", "320x180"),
                                style: "width: 320px; height: 180px;"
                            };
                        }
                        break;

                    case "youtube":
                        item.details.$thumbnail = item.details.thumbnails[1];
                        scope.originalThumbnail = item.details.thumbnails[1];
                        scope.thumbnailWidth = scope.originalThumbnail.width;
                        scope.thumbnailHeight = scope.originalThumbnail.height;
                        scope.originalThumbnail.style = `width: ${scope.thumbnailWidth}px; height: ${scope.thumbnailHeight};`;
                        break;

                    case "twentythree":
                        var t = _.findWhere(item.details.thumbnails, { alias: "medium" });
                        if (t) {
                            item.details.$thumbnail = scope.originalThumbnail = t;
                            scope.thumbnailWidth = t.width;
                            scope.thumbnailHeight = t.height ? t.height : 0;
                            scope.originalThumbnail.style = `width: 320px;`;
                        }
                        break;

                    default:
                        if (item.details && item.detailsitem.details.thumbnails) item.details.$thumbnail = item.details.thumbnails[0];
                        break;

                }

            }

            function fromSource(item, source) {

                scope.loading = true;

                const config = {
                    umbIgnoreErrors: true,
                    params: {
                        source: source
                    }
                };

                $http.get("/umbraco/Skybrud/VideoPicker/GetVideoFromSource", config).success(function (video) {

                    if (video.type === "youtube" && scope.config.services.youtube === false) {

                        item.error = "Videos from YouTube is not permitted for this picker.";

                        delete item.type;
                        delete item.provider;
                        delete item.details;

                    } else if (video.type === "vimeo" && scope.config.services.vimeo === false) {

                        item.error = "Videos from Vimeo is not permitted for this picker.";

                        delete item.type;
                        delete item.provider;
                        delete item.details;

                    } else if (video.type === "twentythree" && scope.config.services.twentythree === false) {

                        item.error = "Videos from Twenty Three is not permitted for this picker.";

                        delete item.type;
                        delete item.provider;
                        delete item.details;

                    } else {

                        delete item.error;

                        item.url = video.url;
                        item.type = video.type;
                        item.provider = video.provider;
                        item.details = video.details;
                        hest(item);

                    }

                    scope.loading = false;

                }).error(function (r) {

                    scope.loading = false;

                    delete item.type;
                    delete item.provider;
                    delete item.details;

                    if (r && r.meta && r.meta.error) {
                        item.error = r.meta.error;
                        notificationsService.error("VideoPicker", r.meta.error);
                    } else {
                        item.error = "Failed retrieving the specified video.";
                        notificationsService.error("VideoPicker", "Failed retrieving the specified video.");
                    }

                });

            }

            function fromEmbed(item, html) {
                if (!html) html = item.embed;
                fromSource(item, html);
            }

            function fromUrl(item) {
                fromSource(item, item.url);
            }

            scope.update = function () {

                if (scope.mode === "embed") {

                    var m = scope.value.embed ? scope.value.embed.match("\<iframe") || scope.value.embed.match("\<script") : null;

                    // Revert to URL mode if the embed value is empty
                    if (!m) {
                        scope.value.url = "";
                        delete scope.value.type;
                        delete scope.value.details;
                        scope.mode = "url";

                        // Add a little timeout so Angular has time to update first
                        $timeout(function () {
                            input.focus();
                        }, 10);

                        return;

                    }

                    fromEmbed(scope.value);
                    
                    return;

                }

                var m0 = scope.value.url.match("\<iframe");
                var m1 = scope.value.url.match("\<script");
                var m2 = scope.value.url.match("vimeo.com/([0-9]+)$");
                var m3 = scope.value.url.match("youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
                var m4 = scope.value.url.match("/manage/video/([0-9]+)$");

                if (m0 || m1) {

                    scope.value.embed = scope.value.url;
                    scope.value.url = "";
                    scope.mode = "embed";

                    // Add a little timeout so Angular has time to update first
                    $timeout(function () {
                        textarea.focus();
                    }, 10);

                    fromEmbed(scope.value);
                    return;

                }


                delete scope.value.embed;

                if (!m2 && !m3 && !m4) {
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

            initConfig();
            init();

        }
    };
});
