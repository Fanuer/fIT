'use strict';
var fIT = angular.module("fIT", ["ngRoute", "LocalStorageModule"]);

fIT.constant("baseUrl", "http://fit-bachelor.azurewebsites.net/api/");
fIT.constant("localStorageAuthIndex", "fIT.SPA.auth");

fIT.config(["$routeProvider", function ($routeProvider) {
  $routeProvider.when("/", {
    controller: "scheduleController",
    templateUrl: "app/views/schedules.html"
  }).when("/login", {
    controller: "loginController",
    templateUrl: "app/views/login.html"
  }).when("/register", {
    controller: "signupController",
    templateUrl: "app/views/signup.html"
  }).otherwise({
    redirectTo: "/"
  });
}]);

fIT.config(function ($httpProvider) {
  $httpProvider.interceptors.push('authInterceptorFactory');
});

fIT.run(['authFactory', function (authFactory) {
  authFactory.fillAuthData();
}
]);

fIT.factory("authFactory", authFactory);
fIT.factory("enumFactory", enumFactory);
fIT.factory("scheduleFactory", scheduleFactory);
fIT.factory("authInterceptorFactory", authInterceptorFactory);

// controllers
fIT.controller("homeController", homeController);
fIT.controller("indexController", indexController);
fIT.controller("signupController", signupController);
fIT.controller("loginController", loginController);
fIT.controller("scheduleController", scheduleController);

// directives
fIT.directive('sameAs', sameAs);