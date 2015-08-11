var fIT = angular.module("fIT", ['ngRoute', 'LocalStorageModule']);

fIT.constant('baseUrl', 'http://fit-bachelor.azurewebsites.net/');

fIT.config(['$routeProvider', function ($routeProvider) {
  $routeProvider.when('/', {
    controller: 'MainController',
    templateUrl: 'App/Partials/Landing.html'
  }).otherwise({
      redirectTo: '/'
    });
}]);
fIT.controller('MainController', MainController);