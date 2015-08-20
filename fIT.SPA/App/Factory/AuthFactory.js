'use strict';

function authFactory($http, $q, $log, localStorageService, baseUrl, localStorageAuthIndex) {
    var factory = {};

    var modelConstructor = function (isAuth, userName, userId) {
        this.isAuth = isAuth || false;
        this.userName = userName || "";
        this.userId = userId || "";
    }

    var updateAuthData = function (newAuthdata) {
        var defaultAuth = new modelConstructor();
        newAuthdata = newAuthdata || defaultAuth;
        factory.authentication.isAuth = newAuthdata.isAuth || defaultAuth.isAuth;
        factory.authentication.userName = newAuthdata.userName || defaultAuth.userName;
        factory.authentication.userId = newAuthdata.userId || defaultAuth.userId;
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
          .then(function (response) {
              try {
                  localStorageService.set(localStorageAuthIndex, {
                      token: response.data.access_token,
                      userName: loginData.userName,
                      userId: response.data.UserId,
                      expireDate: new Date(response.data['.expires']),
                      refreshToken: response.data.refresh_token
                  });
              } catch (e) {
                  $log.error('Error: ' + e.message);
                  deferred.reject(e);
              }

              updateAuthData(new modelConstructor(true, loginData.userName, response.data.UserId));
              deferred.resolve(response);
          })
          .catch(function (response) {
              _logOut();
              if (response.data && response.data.error_description) {
                  $log.error(err.error_description);
                  deferred.reject(response.data.error_description);
              } else {
                  deferred.reject(response);
              }
              
          });

        return deferred.promise;
    }

  var _refreshLogin = function() {
    var authData = localStorageService.get(localStorageAuthIndex);
    
    var deferred = $q.defer();

    if (authData) {
      var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken;
      $http.post(baseUrl + 'accounts/login', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).then(function() {
        localStorageService.set(localStorageAuthIndex, {
          token: response.data.access_token,
          userName: authData.userName,
          userId: response.data.UserId,
          expireDate: new Date(response.data['.expires']),
          refreshToken: response.data.refresh_token
        });
      }).catch(function(response) {
        _logOut();
        if (response.data && response.data.error_description) {
          $log.error(err.error_description);
          deferred.reject(response.data.error_description);
        } else {
          deferred.reject("Error on logging in");
        }
      });
    } else {
      deferred.reject();
    }

    return deferred.promise;
  }

    var _logOut = function () {

        localStorageService.remove(localStorageAuthIndex);
        updateAuthData();

    }
    var _fillAuthData = function () {

        var authData = localStorageService.get(localStorageAuthIndex);
        if (authData) {
            var expireDate = new Date(authData.expireDate);
            var now = new Date();
            if (expireDate <= now) {
              _refreshLogin().then(function() {
                updateAuthData(new modelConstructor(true, authData.userName, authData.userId));
              });
            } else {
              updateAuthData(new modelConstructor(true, authData.userName, authData.userId));
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