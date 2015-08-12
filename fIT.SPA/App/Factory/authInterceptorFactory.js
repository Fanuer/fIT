function authInterceptorFactory($q, $location, localStorageService, localStorageAuthIndex) {

    var authInterceptorServiceFactory = {};

    // wird von einem request ausgefuehrt
    var _request = function (config) {
        config.headers = config.headers || {};

        var authData = localStorageService.get(localStorageAuthIndex);
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    // wird nach einer Response ausgeführt
    var _responseError = function (rejection) {
        if (rejection.status === 401) {
            $location.path('/login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}