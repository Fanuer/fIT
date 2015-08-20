'use strict';
function loginController($scope, $location, authFactory) {

  $scope.message = "";

  $scope.login = function () {
    authFactory.login($scope.loginData).then(function (response) {
      $location.path('/schedule');
    }).catch(function (err) {
      $scope.message = "Error on logging in";
      $scope.messageClass = "danger";
    });

    $scope.message = "Login wird verarbeitet";
    $scope.messageClass = "info";
  };
};