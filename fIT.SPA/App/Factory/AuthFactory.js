'use strict';

function authFactory($http, $q, $log, localStorageService, baseUrl) {
  var factory = {};
  var _authentication = {
    isAuth: false,
    userName: ""
  }

  var _register = function(registrationModel) {
    _logout();
    return $http.post(baseUrl + '/api/accounts/register', registrationModel)
      .then(function(response) {
        return response;
      })
      .catch(function(response) {
      $log.error('Error: ' + response.statusCode);
    });
  }

  var _login = function (loginData) {
    var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;
    var deferred = $q.defer();
    $http.post(baseUrl + 'accounts/login', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
      .success(function (response) {
      localStorageService.set('authorizationData', {
        token: response.access_token, 
        userName: loginData.userName, 
        expireDate: new Date(response['.expires']),
        refreshToken: response.refresh_token
      });

      _authentication.isAuth = true;
      _authentication.userName = loginData.userName;

      deferred.resolve(response);

      })
      .error(function (err, status) {
      _logOut();
      deferred.reject(err);
    });

    return deferred.promise;

  };

  var _logOut = function () {

    localStorageService.remove('authorizationData');
    _authentication.isAuth = false;
    _authentication.userName = "";

  };

  var _fillAuthData = function () {

    var authData = localStorageService.get('authorizationData');
    if (authData) {
      _authentication.isAuth = true;
      _authentication.userName = authData.userName;
    }
  }

  factory.register = _register;
  factory.login = _login;
  factory.logOut = _logOut;
  factory.fillAuthData = _fillAuthData;
  factory.authentication = _authentication;


  return factory;
}