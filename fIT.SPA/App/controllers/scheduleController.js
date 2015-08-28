'use strict';

function scheduleController($scope, scheduleFactory, authFactory, $location, $routeParams) {
    $scope.vm = {};
    function init() {
        if ($routeParams.id) {
            if (!isNaN($routeParams.id)) {
                scheduleFactory.getSchedule($routeParams.id).then(function (response) {
                    var result = {};
                    if (response !== null && typeof response !== "undefined") {
                        if (angular.isArray(response)) {
                            result = response[0];
                        }
                        else if (response.data) {
                            result = response.data;
                        } else {
                            result = response;
                        }
                    }
                    $scope.vm = result;
                });
            } else {
                $scope.vm.userId = authFactory.authentication.userId;
            }
        }
        else {
            scheduleFactory.getSchedules().then(function (response) {
                $scope.vm = response.data || response;
            })
        }
    }

    var _redirectToList = function () {
        $location.path('/');
    }

    var _edit = function edit(id) {
        $location.path('/schedule/' + id);
    }

    var _add = function add() {
        $location.path('/schedule/new');
    }

    var _delete = function (id) {
        scheduleFactory.deleteSchedle(id).then(init);
    }

    var _saveChanges = function () {
        $scope.vm.userId = authFactory.authentication.userId;

        if (typeof $scope.vm.localId === "undefined") {
            scheduleFactory.createSchedule($scope.vm).then(_redirectToList);
        } else {
          scheduleFactory.updateSchedule($scope.vm.localId, $scope.vm).then(_redirectToList);
        }
    }

    $scope.message = "";
    $scope.add = _add;
    $scope.update = _edit;
    $scope.delete = _delete;
    $scope.saveChanges = _saveChanges;
    init();
}