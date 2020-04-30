angular.module("umbraco").controller("Skybrud.VideoPicker.VideoPreview.Controller", function ($scope) {

    $scope.value = null;

    $scope.$watch("item.value", function () {

        $scope.value = $scope.item.value.properties.video;

        if (!$scope.value || !$scope.value.details) {
            $scope.thumbnail = null;
            $scope.duration = null;
            return;
        }

        if (!$scope.value.details.thumbnails || $scope.value.details.thumbnails.length === 0) {

            $scope.thumbnail = null;

        } else {

            // Grab the largest thumbnail by default
            var thumbnail = $scope.value.details.thumbnails[$scope.value.details.thumbnails.length - 1];

            for (var i = $scope.value.details.thumbnails.length - 1; i >= 0; i--) {

                if ($scope.value.details.thumbnails[i].width > 350) {
                    thumbnail = $scope.value.details.thumbnails[i];
                }

            }

            $scope.thumbnail = thumbnail || $scope.value.details.thumbnails[$scope.value.details.thumbnails.length - 1];

        }

        var hours = Math.floor($scope.value.details.duration / 60 / 60);
        var seconds = $scope.value.details.duration - hours * 60 * 60;

        var minutes = Math.floor(seconds / 60);
        seconds = seconds - 60 * minutes;

        var temp = [];

        if (hours) temp.push(hours + "H");
        if (minutes) temp.push(minutes + "M");
        if (seconds) temp.push(seconds + "S");

        $scope.duration = {
            raw: $scope.value.details.duration,
            hours: hours,
            minutes: minutes,
            seconds: seconds,
            text: temp.join(" ")
        };

    }, true);

});