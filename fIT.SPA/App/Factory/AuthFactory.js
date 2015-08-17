﻿'use strict';

function authFactory($http, $q, $log, localStorageService, baseUrl, localStorageAuthIndex) {
    var factory = {};

    var modelConstructor = function (isAuth, userName, userId) {
        this.isAuth = isAuth || false;
        this.userName = userName || "";
        this.userId = userId || "";
    }
    var _register = function (registrationModel) {
        _logout();
        return $http.post(baseUrl + '/api/accounts/register', registrationModel)
          .then(function (response) {
              return response;
          })
          .catch(function (response) {
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
                      userId: response.UserId,
                      expireDate: new Date(response['.expires']),
                      refreshToken: response.refresh_token
                  });
              } catch (e) {
                  $log.error('Error: ' + e.message);
                  deferred.reject(e);
              }

              factory.authentication = new modelConstructor(true, loginData.userName, response.UserId);
              deferred.resolve(response);
          })
          .error(function (err, status) {
              _logOut();
              if (err && err.error_description) {
                  $log.error(err.error_description);
                  deferred.reject(err.error_description);
              } else {
                  deferred.reject(err);
              }
              
          });

        return deferred.promise;
    }
    var _logOut = function () {

        localStorageService.remove(localStorageAuthIndex);
        factory.authentication = new modelConstructor();

    }
    var _fillAuthData = function () {

        var authData = localStorageService.get(localStorageAuthIndex);
        if (authData) {
            var expireDate = new Date(authData.expireDate);
            var now = new Date();
            if (expireDate > now) {
                factory.authentication = new modelConstructor(true, authData.userName, authData.userId);
            }
        }
    }
    var _checkOnlineStatus = function () {
        var deferred = $q.defer();
        $http.get(baseUrl + 'accounts/ping')
          .success(function () {
            deferred.resolve(); })
          .error(function () { deferred.reject(); });
        return deferred.promise;
    }

    factory.register = _register;
    factory.login = _login;
    factory.logOut = _logOut;
    factory.fillAuthData = _fillAuthData;
    factory.checkOnlineStatus = _checkOnlineStatus;
    factory.authentication = new modelConstructor();
    return factory;
}