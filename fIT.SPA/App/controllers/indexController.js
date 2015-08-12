'use strict';
function indexController($scope, $location, $interval, authFactory) {

  $scope.logOut = function () {
    authFactory.logOut();
    $location.path('/home');
  }

  $scope.onlineStatus = 'offline';
  $interval(function () {
    $scope.onlineStatus = authFactory.checkOnlineStatus();
  }, 5000);

  $scope.onlineStatus = true;
  $scope.navbarExpanded = false;
  $scope.authentication = authFactory.authentication;
};
