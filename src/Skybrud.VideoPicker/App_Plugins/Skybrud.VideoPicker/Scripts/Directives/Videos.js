angular.module('umbraco').directive('skybrudVideopicker', function ($http, userService, dialogService, notificationsService, entityResource, mediaHelper) {
    return {
        scope: {
            value: '=',
            config: '='
        },
        transclude: true,
        restrict: 'E',
        replace: true,
        templateUrl: '/App_Plugins/Skybrud.VideoPicker/Views/VideosDirective.html',
        link: function (scope, element, attr) {

            if (!scope.value || typeof (scope.value) != 'object') scope.value = { title: '', items: [] };
            if (!scope.value.title) scope.value.title = '';
            if (!scope.value.items) scope.value.items = [];

            var v = Umbraco.Sys.ServerVariables.application.version.split('.');
            scope.umbVersion = v = parseFloat(v[0] + '.' + (v[1].length === 1 ? "0" + v[1] : v[1]));

            // Image picker related
            var startNodeId = null;

            function hest(item) {

                switch (item.type) {

                case 'vimeo':
                    item.$providerName = 'Vimeo';
                    item.details.$thumbnail = item.details.thumbnails[0];
                    break;

                case 'youtube':
                    item.$providerName = 'YouTube';
                    item.details.$thumbnail = item.details.thumbnails[0];
                    break;

                case 'twentythree':
                    item.$providerName = 'Twenty Three';
                    item.details.$thumbnail = _.findWhere(item.details.thumbnails, { alias: 'portrait' });
                    break;

                default:
                    item.$providerName = 'Ukendt';
                    if (item.details && item.detailsitem.details.thumbnails) item.details.$thumbnail = item.details.thumbnails[0];
                    break;

                }

            }

            function initConfig() {

                if (!scope.config) scope.config = {};

                var c = scope.config;

                if (!c.list) c.list = {};
                if (!c.items) c.items = {};
                if (!c.details) c.details = {};

                if (!c.list.limit) c.list.limit = 0;

                if (!c.list.title) c.list.title = {};
                if (!c.list.title.mode) c.list.title.mode = 'hidden';
                c.list.title.visible = c.list.title.mode != 'hidden';
                c.list.title.required = c.list.title.mode == 'required';

                if (!c.items.title) c.items.title = {};
                if (!c.items.title.mode) c.items.title.mode = 'hidden';
                c.items.title.visible = c.items.title.mode != 'hidden';
                c.items.title.required = c.items.title.mode == 'required';

                if (!c.items.description) c.items.description = {};
                if (!c.items.description.mode) c.items.description.mode = 'hidden';
                c.items.description.visible = c.items.description.mode != 'hidden';
                c.items.description.required = c.items.description.mode == 'required';

                if (!c.details.description) c.details.description = {};
                if (c.details.description.visible !== false) c.details.description.visible = true;

                if (!c.services) c.services = {};
                c.services.youtube = c.services.youtube !== false;
                c.services.vimeo = c.services.vimeo !== false;
                c.services.twentythree = c.services.twentythree !== false;

            }

            function init() {

                // Image picker related
                userService.getCurrentUser().then(function (userData) {
                    startNodeId = userData.startMediaId;
                });

                // Create a shadow object since Umbraco will strip our $ properties on save
                scope.shadowValue = {
                    title: scope.value.title,
                    items: scope.value.items
                };

                // Update all existing items
                angular.forEach(scope.shadowValue.items, function (item) {
                    hest(item);
                });

                // Add a new video item if the list is empty
                if (scope.shadowValue.items.length == 0) {
                    scope.addVideo();
                }

                // Get the IDs of the images currently selected
                var ids = [];
                angular.forEach(scope.shadowValue.items, function (item) {

                    // Make sure we pull information about the referenced image
                    if (item.thumbnailId) ids.push(item.thumbnailId);

                });

                if (ids.length > 0) {

                    // Initialize a new hashset (JavaScript object)
                    var hash = {};

                    entityResource.getByIds(ids, 'media').then(function (data) {

                        // Add each media to the hashset for O(1) lookups
                        angular.forEach(data, function (media) {
                            if (!media.image) media.image = mediaHelper.resolveFileFromEntity(media);
                            hash[media.id] = media;
                        });

                        // Update each item with its corresponding media
                        angular.forEach(scope.shadowValue.items, function (item) {

                            console.log(item.thumbnailId, hash[item.thumbnailId]);

                            if (item.thumbnailId && hash[item.thumbnailId]) {
                                item.$thumbnail = hash[item.thumbnailId];
                                item.$thumbnailUrl = getThumbnailUrl(item);
                            }
                        });

                    });

                }

            }

            scope.updateModel = function () {
                scope.value = scope.shadowValue;
            };

            scope.addVideo = function () {
                scope.shadowValue.items.push({ url: '' });
                scope.updateModel();
            };

            scope.removeItem = function (index) {
                var temp = [];
                for (var i = 0; i < scope.shadowValue.items.length; i++) {
                    if (index != i) temp.push(scope.shadowValue.items[i]);
                }
                scope.shadowValue.items = temp;
                scope.value = scope.shadowValue;
                scope.updateModel();
            };

            /// Gets an URL for a cropped version of the image (based on the current configuration)
            function getThumbnailUrl(item) {
                return item.$thumbnail ? item.$thumbnail.image + '?width=320&height=180&mode=crop' : null;
            }

            /// Swaps two items in an array
            function swap(array, i, j) {
                var a = array[i];
                array[i] = array[j];
                array[j] = a;
            }

            scope.addThumbnail = function (item) {

                // The new overlay only works from 7.4 and up, so for older
                // versions we should use the dialogService instead
                if (v < 7.04) {

                    dialogService.mediaPicker({
                        startNodeId: startNodeId,
                        multiPicker: false,
                        onlyImages: true,
                        callback: function (data) {
                            item.thumbnailId = data.id;
                            item.$thumbnail = data;
                            item.$thumbnailUrl = getThumbnailUrl(item);
                            dialogService.closeAll();
                        }
                    });

                } else {

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

                            item.thumbnailId = data.id;
                            item.$thumbnail = data;
                            item.$thumbnailUrl = getThumbnailUrl(item);

                            scope.mediaPickerOverlay.show = false;
                            scope.mediaPickerOverlay = null;

                        }
                    };

                }

            };

            scope.removeThumbnail = function (item) {
                item.thumbnailId = 0;
                item.$thumbnail = null;
                item.$thumbnailUrl = null;
            };

            scope.urlChanged = function (item) {

                var m1 = item.url.match('vimeo.com/([0-9]+)$');
                var m2 = item.url.match('youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)');
                var m3 = item.url.match('/manage/video/([0-9]+)$');

                if (!m1 && !m2 && !m3) {
                    delete item.type;
                    delete item.details;
                    return;
                }

                item.loading = true;

                $http.get('/umbraco/Skybrud/VideoPicker/GetVideoFromUrl?url=' + item.url).success(function (video) {

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

                    item.loading = false;

                }).error(function (r) {

                    item.loading = false;

                    item.type = null;
                    item.details = null;

                    if (r && r.meta && r.meta.error) {
                        notificationsService.error('VideoPicker', r.meta.error);
                    } else {
                        notificationsService.error('VideoPicker', 'Failed retrieving the specified video');
                    }

                });

            };

            initConfig();
            init();

        }
    };
});
