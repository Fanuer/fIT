function indexController($scope, $location, $interval, authFactory) {  |\label{line:controller:dependency-injection}|
  $scope.onlineStatus = 'offline';
  $scope.logOut = function () {
    authFactory.logOut();
    $location.path('/'); 
  }
  [...]
  $scope.onlineStatus = true; |\label{line:controller:scope}|
  $scope.navbarExpanded = false;
  $scope.authentication = authFactory.authentication;
};
