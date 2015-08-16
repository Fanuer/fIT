'use strict';
function scheduleFactory($http, baseUrl) {
    var http = $http;
    var factory = {};

    var _getSchedules = function () {
        return http.get(baseUrl + 'schedule');
    };

    var _getSchedule = function(id) {
        return http.get(baseUrl + 'schedule/' + id);
    }

    var _editSchedule = function(id, data) {
        return http.put(baseUrl + 'schedule/' + id, data);
    }

    var _createSchedule = function(data) {
        return http.post(baseUrl + 'schedule/' + id, data);
    }

    var _deleteSchedule = function(id) {
        return http.delete(baseUrl + 'schedule/' +id);
    }

    factory.getSchedules = _getSchedules;
    factory.getSchedule = _getSchedule;
    factory.createSchedule = _createSchedule;
    factory.updateSchedule = _editSchedule;
    factory.deleteSchedle = _deleteSchedule;

    return factory;

};