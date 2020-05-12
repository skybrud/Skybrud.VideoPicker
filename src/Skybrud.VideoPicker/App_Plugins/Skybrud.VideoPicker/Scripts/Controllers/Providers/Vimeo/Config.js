angular.module("umbraco").controller("Skybrud.VideoPicker.Vimeo.Config.Controller", function ($scope, formHelper) {

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
    bool("loop", false);
    bool("title", true);
    bool("byline", true);
    bool("portrait", true);

    if (!config.color) config.color = { value: "00adef" };
    if (!config.color.value) config.color.value = "00adef";

    $scope.properties = [
        {
            alias: "consent",
            label: "Require consent",
            description: "Select whether the embed code requires prior consent before being shown to the user.",
            value: config.consent.value,
            view: "boolean"
            //view: "/App_Plugins/Skybrud.VideoPicker/Views/Editors/ConfigOption.html",
            //option: {
            //    view: "boolean"
            //}
        },
        {
            alias: "autoplay",
            label: "Autoplay",
            description: "Select whether the video should automatically start to play when the player loads.",
            value: config.autoplay.value,
            view: "boolean"
        },
        {
            alias: "loop",
            label: "Loop",
            description: "Select whether the video should play again and again.",
            value: config.loop.value,
            view: "boolean"
        },
        {
            alias: "color",
            label: "Color",
            description: "Select the color used in the player.",
            value: config.color.value,
            view: "textbox"
        },
        {
            alias: "title",
            label: "Show title",
            description: "Select whether the title of the video should be shown in the player.",
            value: config.title.value,
            view: "boolean"
        },
        {
            alias: "byline",
            label: "Show byline",
            description: "Select whether the byline of the video author should be shown in the player.",
            value: config.byline.value,
            view: "boolean"
        },
        {
            alias: "portrait",
            label: "Show portrait",
            description: "Select whether the portrait of the video author should be shown in the player.",
            value: config.portrait.value,
            view: "boolean"
        }
    ];

    $scope.save = function () {

        if (formHelper.submitForm({ scope: $scope })) {

            formHelper.resetForm({ scope: $scope });
            
            $scope.properties.forEach(function(p) {

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