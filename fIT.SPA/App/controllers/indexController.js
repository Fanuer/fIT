'use strict';
function indexController($scope, $location, $interval, authFactory) {
  $scope.logOut = function () {
    authFactory.logOut();
    $location.path('/');
  }

  $scope.onlineStatus = 'offline';
  $interval(function () {
    try {
      authFactory.checkOnlineStatus().then(function(response) {
        $scope.onlineStatus = true;
      }).catch(function(response) {
        $scope.onlineStatus = false;
      });
    } catch (e) {
      $scope.onlineStatus = false;
    }
  }, 5000);

  $scope.onlineStatus = true;
  $scope.navbarExpanded = false;
  $scope.authentication = authFactory.authentication;
};
