'use strict';
var fIT = angular.module("fIT", ["ngRoute", "LocalStorageModule"]);

fIT.constant("baseUrl", "http://fit-bachelor.azurewebsites.net/api/");

fIT.config(["$routeProvider", function ($routeProvider) {
    $routeProvider.when("/", {
        controller: "homeController",
        templateUrl: "app/views/Home.html"
    }).when("/login", {
        controller: "loginController",
        templateUrl: "app/views/login.html"
    }).when("/register", {
        controller: "signupController",
        templateUrl: "app/views/signup.html"
    }).when("/schedules", {
        controller: "scheduleController",
        templateUrl: "app/views/schedules.html"
    }).otherwise({
        redirectTo: "/"
    });
}]);

fIT.factory("authFactory", authFactory);
fIT.factory("enumFactory", enumFactory);
fIT.factory("scheduleFactory", scheduleFactory);
fIT.factory("authInterceptorFactory", authInterceptorFactory);

// controllers
fIT.controller("homeController", homeController);
fIT.controller("indexController", indexController);
fIT.controller("signupController", signupController);
fIT.controller("loginController", loginController);

// directives
fIT.directive('sameAs', sameAs);