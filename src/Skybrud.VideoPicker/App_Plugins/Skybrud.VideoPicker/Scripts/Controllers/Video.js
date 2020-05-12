angular.module("umbraco").controller("Skybrud.VideoPicker.Video.Controller", function ($scope, $element, $timeout, $http, notificationsService) {

    if (!$scope.model.value) $scope.model.value = {};
    if (!$scope.model.value.source) $scope.model.value.source = "";

    var input = $element[0].querySelector("input");
    var textarea = $element[0].querySelector("textarea");

    $scope.uniqueId = `videoPicker_${Math.random().toString(36).substr(2, 9)}`;

    // Detect the source mode
    $scope.mode = $scope.value && $scope.value.embed ? "embed" : "url";

    // Attempt to get the parent properties group (if used with Elements)
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

    // Attempt to get the parent properties array (if used with Elements)
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

    $scope.timeout = null;

    $scope.change = function () {

        // Cancel existing timeout if present
        if ($scope.timeout) {
            $timeout.cancel($scope.timeout);
            $scope.timeout = null;
        }

        // Reset if source is empty
        if (!$scope.model.value.source) {

            $scope.mode = "url";
            $scope.thumbnail = null;
            $scope.model.value = { source: "" };

            $timeout(function () {  input.focus(); }, 10);

            return;

        }

        // Detect URL mode
        var m1 = $scope.model.value.source.match("\<iframe");
        $scope.mode = m1 ? "embed" : "url";

        // Add a little timeout so Angular has time to update first
        if ($scope.mode === "embed") {
            $timeout(function() {
                textarea.focus();
            }, 10);

        } else {
            $timeout(function () {
                input.focus();
            }, 10);
        }

        $scope.timeout = $timeout(function () {
            $scope.timeout = null;
            $scope.update();
        }, 500);

    };






    $scope.update = function () {

        console.log("update");

        if (!$scope.model.value.source) return;

        // Make sure to trim leading or trailing whitespace
        $scope.model.value.source = $scope.model.value.source.trim();

        $scope.loading = true;

        $http({
            url: "/umbraco/Skybrud/VideoPicker/GetVideo",
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
        }).then(function (response) {

            $scope.loading = false;

            $scope.model.value = {
                source: $scope.model.value.source
            };

            for (var p in response.data) $scope.model.value[p] = response.data[p];

            if (group) {
                var matches = _.where(group.properties, { alias: "title" });
                if (matches.length > 0 && !matches[0].value) matches[0].value = $scope.model.value.details.title;
            } else if (properties) {
                var matches2 = _.where(properties, { alias: "title" });
                if (matches2.length > 0 && !matches2[0].value) matches2[0].value = $scope.model.value.details.title;
            }

            updateUI($scope.model.value.details);

        }, function (res) {

            $scope.loading = false;

            $scope.thumbnail = null;
            $scope.model.value = { source: $scope.model.value.source };

            if (res.status === 404) {
                notificationsService.error("Skybrud.VideoPicker", "A video with the specified URL could not be found.");
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
        updateUI($scope.model.value.details);
    }

    function updateUI(details) {

        if (!details || !details.thumbnails) {
            $scope.thumbnail = null;
            $scope.duration = null;
            return;
        }

        if (details.thumbnails.length === 0) {

            $scope.thumbnail = null;

        } else {

            // Grab the largest thumbnail by default
            var thumbnail = details.thumbnails[details.thumbnails.length - 1];

            for (var i = details.thumbnails.length - 1; i >= 0; i--) {

                if (details.thumbnails[i].width > 350) {
                    thumbnail = details.thumbnails[i];
                }

            }

            $scope.thumbnail = thumbnail || details.thumbnails[details.thumbnails.length - 1];

        }

        var hours = Math.floor(details.duration / 60 / 60);
        var seconds = details.duration - hours * 60 * 60;

        var minutes = Math.floor(seconds / 60);
        seconds = seconds - 60 * minutes;

        var temp = [];

        if (hours) temp.push(hours + "H");
        if (minutes) temp.push(minutes + "M");
        if (seconds) temp.push(seconds + "S");

        $scope.duration = {
            raw: details.duration,
            hours: hours,
            minutes: minutes,
            seconds: seconds,
            text: temp.join(" ")
        };

    }

});