angular.module("umbraco").controller("Skybrud.VideoPicker.YouTube.Config.Controller", function ($scope, formHelper) {

    var config = $scope.model.config;

    function bool(alias, value) {

        if (config[alias]) return;

        config[alias] = {
            readonly: true,
            value: value
        };

    }

    bool("consent", false);
    bool("nocookie", false);
    bool("controls", true);
    bool("autoplay", false);
    bool("loop", false);

    $scope.properties = [
        {
            alias: "consent",
            label: "Require consent",
            description: "Sets whether the embed code requires prior consent before being shown to the user.",
            value: config.consent.value,
            view: "boolean"
            //view: "/App_Plugins/Skybrud.VideoPicker/Views/Editors/ConfigOption.html",
            //option: {
            //    view: "boolean"
            //}
        },
        {
            alias: "nocookie",
            label: "No cookies",
            description: "Sets whether the player should use YouTube's <c>www.youtube-nocookie.com</c> domain instead.",
            value: config.nocookie.value,
            view: "boolean"
        },
        {
            alias: "controls",
            label: "Show controls",
            description: "Sets whether player controls should be displayed in the video player.",
            value: config.controls.value,
            view: "boolean"
        },
        {
            alias: "autoplay",
            label: "Autoplay",
            description: "Sets whether the video should automatically start to play when the player loads.",
            value: config.autoplay.value,
            view: "boolean"
        },
        {
            alias: "loop",
            label: "Loop",
            description: "Sets whether the video should play again and again.",
            value: config.loop.value,
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