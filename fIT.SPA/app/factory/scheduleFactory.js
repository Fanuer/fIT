'use strict';
function scheduleFactory($http, baseUrl) {

    var factory = {};

    var _getSchedules = function () {
        return $http.get(baseUrl + 'schedules').then(function (results) {
            return results;
        });
    };

    factory.getSchedules = _getSchedules;
    return factory;

};