'use strict';
function scheduleFactory(cachedHttp, baseUrl, entityNames) {
    var http = cachedHttp;
    var factory = {};


    var _getSchedules = function () {
        return http.get(baseUrl + 'schedule', entityNames.schedule);
    };

    var _getSchedule = function (id) {
        return http.get(baseUrl + 'schedule/' + id, entityNames.schedule, id);
    }

    var _editSchedule = function(id, data) {
        return http.put(baseUrl + 'schedule/' + id, data, entityNames.schedule);
    }

    var _createSchedule = function(data) {
        return http.post(baseUrl + 'schedule', data, entityNames.schedule);
    }

    var _deleteSchedule = function(id) {
        return http.delete(baseUrl + 'schedule/' + id, entityNames.schedule, id);
    }

    factory.getSchedules = _getSchedules;
    factory.getSchedule = _getSchedule;
    factory.createSchedule = _createSchedule;
    factory.updateSchedule = _editSchedule;
    factory.deleteSchedle = _deleteSchedule;

    return factory;

};