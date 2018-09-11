angular.module('umbraco').controller('SkybrudVideoPicker.VideosPreValueEditor.Controller', function ($scope) {

    function init() {

        if (!$scope.model.value) $scope.model.value = {};

        var c = $scope.model.value;

        if (!c.list) c.list = {};
        if (!c.list.limit) c.list.limit = 0;
        if (!c.list.title) c.list.title = {};
        if (!c.list.title.mode) c.list.title.mode = 'hidden';

        if (!c.items) c.items = {};
        if (!c.items.title) c.items.title = {};
        if (!c.items.title.mode) c.items.title.mode = 'hidden';
        if (!c.items.description) c.items.description = {};
        if (!c.items.description.mode) c.items.description.mode = 'hidden';

        if (!c.details) c.details = {};
        if (!c.details.description) c.details.description = {};
        if (c.details.description.visible !== false) c.details.description.visible = true;

        if (!c.services) c.services = {};
        c.services.youtube = c.services.youtube !== false;
        c.services.vimeo = c.services.vimeo !== false;

    }

    init();

});