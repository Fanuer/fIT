'use strict';

function scheduleController($scope, scheduleFactory) {
  $scope.vm = {};

  scheduleFactory.getSchedules().success(function (data) {
    $scope.vm.schedules = data;
  });
}