'use strict';
function scheduleFactory($http, baseUrl) {

    var factory = {};

    var _getSchedules = function () {
      return $http.get(baseUrl + 'schedule');
    };

    factory.getSchedules = _getSchedules;
    return factory;

};