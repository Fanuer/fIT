'use strict';
function loginController($scope, $location, authFactory) {

    $scope.message = "";

    $scope.login = function () {
        authFactory.login($scope.loginData).then(function (response) {
            $location.path('/schedule');
        },
         function (err) {
             $scope.message = err;
         });

        $scope.message = "Login wird verarbeitet";
      $scope.messageClass = "info";
    };
};