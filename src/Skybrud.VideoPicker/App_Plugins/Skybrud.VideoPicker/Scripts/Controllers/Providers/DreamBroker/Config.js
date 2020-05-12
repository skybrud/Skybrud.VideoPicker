angular.module("umbraco").controller("Skybrud.VideoPicker.DreamBroker.Config.Controller", function ($scope, formHelper) {

    var config = $scope.model.config;

    function bool(alias, value) {

        if (config[alias]) return;

        config[alias] = {
            readonly: true,
            value: value
        };

    }

    bool("consent", false);
    bool("autoplay", false);

    $scope.properties = [
        {
            alias: "consent",
            label: "Require consent",
            description: "Select whether the embed code requires prior consent before being shown to the user.",
            value: config.consent.value,
            view: "boolean"
        },
        {
            alias: "autoplay",
            label: "Autoplay",
            description: "Select whether the video should automatically start to play when the player loads.",
            value: config.autoplay.value,
            view: "boolean"
        }
    ];

    $scope.save = function () {

        if (formHelper.submitForm({ scope: $scope })) {

            formHelper.resetForm({ scope: $scope });
            
            $scope.properties.forEach(function (p) {

                // Fix boolean values (instead of "1" and "0")
                if (p.view === "boolean") {
                    p.value = p.value === "1" || p.value === true;
                }

                config[p.alias] = {
                    readonly: true,
                    value: p.value
                };

            });

            $scope.model.submit(config);

        }

    };

});