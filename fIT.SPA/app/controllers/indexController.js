'use strict';
function indexController($scope, $location, authFactory) {

    $scope.logOut = function () {
        authFactory.logOut();
        $location.path('/home');
    }

    $scope.authentication = authFactory.authentication;
};
