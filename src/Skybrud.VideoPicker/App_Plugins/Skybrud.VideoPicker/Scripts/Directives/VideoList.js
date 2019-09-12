angular.module("umbraco").directive("skyVideoList", function () {
    return {
        scope: {
            value: "=",
            config: "="
        },
        transclude: true,
        restrict: "E",
        replace: true,
        templateUrl: "/App_Plugins/Skybrud.VideoPicker/Views/Directives/VideoList.html",
        link: function (scope) {

            if (!scope.value || typeof scope.value !== "object") scope.value = { title: "", items: [] };
            if (!scope.value.title) scope.value.title = "";
            if (!scope.value.items) scope.value.items = [];
            
            function initConfig() {

                if (!scope.config) scope.config = {};

                var c = scope.config;

                if (!c.list) c.list = {};
                if (!c.items) c.items = {};
                if (!c.details) c.details = {};

                if (!c.list.limit) c.list.limit = 0;

                if (!c.list.title) c.list.title = {};
                if (!c.list.title.mode) c.list.title.mode = "hidden";
                c.list.title.visible = c.list.title.mode !== "hidden";
                c.list.title.required = c.list.title.mode === "required";

                if (!c.items.title) c.items.title = {};
                if (!c.items.title.mode) c.items.title.mode = "hidden";
                c.items.title.visible = c.items.title.mode !== "hidden";
                c.items.title.required = c.items.title.mode === "required";

                if (!c.items.description) c.items.description = {};
                if (!c.items.description.mode) c.items.description.mode = "hidden";
                c.items.description.visible = c.items.description.mode !== "hidden";
                c.items.description.required = c.items.description.mode === "required";

                if (!c.details.description) c.details.description = {};
                if (c.details.description.visible !== false) c.details.description.visible = true;

                if (!c.services) c.services = {};
                c.services.youtube = c.services.youtube !== false;
                c.services.vimeo = c.services.vimeo !== false;
                c.services.twentythree = c.services.twentythree !== false;

                scope.itemConfig = {
                    services: scope.config.services,
                    title: scope.config.items.title,
                    description: scope.config.items.description,
                    details: scope.config.details
                };

            }

            function init() {

                // Create a shadow object since Umbraco will strip our $ properties on save
                scope.shadowValue = {
                    title: scope.value.title,
                    items: scope.value.items
                };

                // Add a new video item if the list is empty
                if (scope.shadowValue.items.length === 0) {
                    scope.addVideo();
                }

            }

            scope.togglePrompt = function (e) {
                if (e.$deletePrompt === true) {
                    e.$deletePrompt = false;
                } else {
                    e.$deletePrompt = true;
                }
            };

            scope.hidePrompt = function(e) {
                e.$deletePrompt = false;
            };

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
                    if (index !== i) temp.push(scope.shadowValue.items[i]);
                }
                scope.shadowValue.items = temp;
                scope.value = scope.shadowValue;
                scope.updateModel();
            };
            
            initConfig();
            init();

        }
    };
});