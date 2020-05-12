angular.module("umbraco").controller("Skybrud.VideoPicker.VideoProviders.Controller", function ($scope, $element, $timeout, $http, editorService, notificationsService) {

    $scope.providers = [];

    $scope.loading = true;

    if (!$scope.model.value) $scope.model.value = {};
    if (Array.isArray($scope.model.value)) $scope.model.value = {};

    $http.get("/umbraco/Skybrud/VideoPicker/GetProviders").then(function (response) {

        $scope.providers = response.data;

        $scope.providers.forEach(function(p) {

            if (!$scope.model.value[p.alias]) {
                $scope.model.value[p.alias] = {
                    enabled: false,
                    config: {}
                };
            } else {
                p.enabled = $scope.model.value[p.alias].enabled;
            }

            if ($scope.model.value[p.alias].config) delete $scope.model.value[p.alias].config;

        });

    }, function (res) {

        $scope.loading = false;

        notificationsService.add({
            headline: "Skybrud.VideoPicker",
            message: res.data.Message || "An error occured on the server.",
            type: "error"
        });

    });

    $scope.toggle = function(provider) {
        provider.enabled = !provider.enabled;
        $scope.model.value[provider.alias].enabled = provider.enabled;
    };

    $scope.configure = function(provider) {

        var config = angular.copy($scope.model.value[provider.alias]);

        editorService.open({
            title: "Configure",
            size: "small",
            view: provider.configView,
            provider: provider,
            config: config,
            submit: function (config) {
                $scope.model.value[provider.alias] = config;
                editorService.close();
            },
            close: function() {
                editorService.close();
            }
        });

    };


});