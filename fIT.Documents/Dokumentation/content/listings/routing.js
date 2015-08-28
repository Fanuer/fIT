fIT.config(["$routeProvider", function ($routeProvider) {
  $routeProvider.when("/", {
    controller: "scheduleController",
    templateUrl: "app/views/schedules.html"
  }).when("/schedule/:id", {
    controller: "scheduleController",
    templateUrl: "app/views/schedule.html"
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