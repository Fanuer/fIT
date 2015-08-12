'use strict';

function signupController($scope, enumFactory, $location, $timeout, authFactory) {
    $scope.vm = {};
    enumFactory.getFitnessTypes().success(function (data, status) {
        $scope.vm.fitnessTypes = data;
    });
    enumFactory.getGenderTypes().success(function (data, status) {
        $scope.vm.genderTypes = data;
    });
    enumFactory.getJobTypes().success(function (data, status) {
        $scope.vm.jobTypes = data;
    });

    $scope.savedSuccessfully = false;
    $scope.message = "";

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }

    $scope.signUp = function () {

      authFactory.register($scope.vm.register).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
            startTimer();

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 if (response.data.modelState.hasOwnProperty(key)) {
                     for (var i = 0; i < response.data.modelState[key].length; i++) {
                         errors.push(response.data.modelState[key][i]);
                     }
                 }
             }
             $scope.message = "Failed to register user due to:" + errors.join(' ');
         });
    };



};