angular.module("umbraco").controller("Skybrud.VideoPicker.Video.Controller", function ($scope, $http, notificationsService) {

    if (!$scope.model.value) $scope.model.value = {};
    if (!$scope.model.value.source) $scope.model.value.source = "";


    var group = null;
    var parent = $scope.$parent;
    for (var i = 0; i < 10; i++) {
        if (!parent) break;
        if (parent.group) {
            group = parent.group;
            break;
        }
        parent = parent.$parent;
    }


    var properties = null;
    parent = $scope.$parent;
    for (var j = 0; j < 10; j++) {
        if (!parent) break;
        if (parent.model && parent.model.properties) {
            properties = parent.model.properties;
            break;
        }
        parent = parent.$parent;
    }



    $scope.change = function () {

        if (!$scope.model.value.source) {
            $scope.thumbnail = null;
            $scope.model.value = { source: "" };
            return;
        }

        $scope.loading = true;

        $http({
            url: "/umbraco/api/Videos/GetVideo",
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            umbIgnoreErrors: true,
            transformRequest: function (obj) {
                var str = [];
                for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                return str.join("&");
            },
            data: {
                source: $scope.model.value.source
            }
        }).then(function(response) {

            $scope.loading = false;

            $scope.model.value = {
                source: $scope.model.value.source
            };

            for (var p in response.data) $scope.model.value[p] = response.data[p];

            if (group) {
                var matches = _.where(group.properties, { alias: "title" });
                if (matches && !matches[0].value) matches[0].value = matches[0].value = $scope.model.value.details.title;
            } else if (properties) {
                var matches2 = _.where(properties, { alias: "title" });
                if (matches2 && !matches2[0].value) matches2[0].value = matches2[0].value = $scope.model.value.details.title;
            }

            $scope.thumbnail = getThumbnail($scope.model.value.details);

        }, function (res) {

            $scope.loading = false;

            if (res.status === 404) {
                notificationsService.error("Video not found", "A video with the specified URL could not be found.");
                return;
            }

            notificationsService.add({
                headline: "Skybrud.VideoPicker",
                message: res.data.Message || "An error occured on the server.",
                type: "error"
        });

        });

        };

    if ($scope.model.value.details) {
        $scope.thumbnail = getThumbnail($scope.model.value.details);
    }

    function getThumbnail(details, width) {

        if (!details || !details.thumbnails) return null;

        if (!width) width = 350;

        // Grab the largest thumbnail by default
        var thumbnail = details.thumbnails[details.thumbnails.length - 1];

        for (var i = details.thumbnails.length - 1; i >= 0; i--) {

            if (details.thumbnails[i].width > width) {
                thumbnail = details.thumbnails[i];
            }

        }

        return thumbnail || details.thumbnails[details.thumbnails.length - 1];

    }

});