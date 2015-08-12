'use strict';

function authFactory($http, $q, $log, localStorageService, baseUrl, localStorageAuthIndex) {
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
        try {
          localStorageService.set(localStorageAuthIndex, {
            token: response.access_token,
            userName: loginData.userName,
            expireDate: new Date(response['.expires']),
            refreshToken: response.refresh_token
          });
        } catch (e) {
          $log.error('Error: ' + e.message);
          deferred.reject(e);
          return e.message;
        } 
      _authentication.isAuth = true;
      _authentication.userName = loginData.userName;

      deferred.resolve(response);

      })
      .error(function (err, status) {
      _logOut();
      deferred.reject(err);
    });

    return deferred.promise;
  }
  var _logOut = function () {

    localStorageService.remove(localStorageAuthIndex);
    _authentication.isAuth = false;
    _authentication.userName = "";

  }
  var _fillAuthData = function () {

    var authData = localStorageService.get(localStorageAuthIndex);
    if (authData) {

      var expireDate = new Date(authData.expireDate);
      var now = new Date();
      if (expireDate > now) {
        _authentication.isAuth = true;
        _authentication.userName = authData.userName;
      }
    }
  }
  var _checkOnlineStatus = function() {
    var deferred = $q.defer();
    $http.get(baseUrl + 'accounts/ping')
      .success(function() { return true; })
      .error(function() { return false; });
    return deferred.promise;
  }


  factory.register = _register;
  factory.login = _login;
  factory.logOut = _logOut;
  factory.fillAuthData = _fillAuthData;
  factory.checkOnlineStatus = _checkOnlineStatus;
  factory.authentication = _authentication;
  return factory;
}