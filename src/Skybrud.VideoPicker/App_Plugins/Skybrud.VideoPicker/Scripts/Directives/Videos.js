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

            // Image picker related
            var startNodeId = null;

            function hest(item) {
                switch (item.type) {
                    case 'vimeo': item.$typeName = 'Vimeo'; break;
                    case 'youtube': item.$typeName = 'YouTube'; break;
                    default: item.$typeName = 'Ukendt'; break;
                }
            }

            function initConfig() {
                if (!scope.config) scope.config = {};
                scope.config.limit = scope.config ? parseInt(scope.config.limit) : 0;
                scope.config.hideDescription = scope.config.hideDescription == 1;
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

                // Get the Umbraco version
                var v = Umbraco.Sys.ServerVariables.application.version.split('.');
                v = parseFloat(v[0] + '.' + v[1]);

                // The new overlay only works from 7.4 and up, so for older
                // versions we should use the dialogService instead
                if (v < 7.4) {

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

                if (!m1 && !m2) {
                    delete item.type;
                    delete item.details;
                    return;
                }

                $http.get('/umbraco/Skybrud/VideoPicker/GetVideoFromUrl?url=' + item.url).success(function (video) {

                    item.url = video.url;
                    item.type = video.type;
                    item.details = video.details;
                    hest(item);

                }).error(function (r) {

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