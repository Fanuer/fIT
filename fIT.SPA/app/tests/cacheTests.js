/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/jasmine/jasmine-html.js" />
/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/jasmine/jasmine.js" />
/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/angular.min.js" />
/// <reference path="../factory/cachedHttpFactory.js" />
"use strict";

function HttpMockUpModel(url, sendData, receiveData) {
    this.url = url;
    this.sendData = sendData;
    this.receiveData = receiveData;
}

function HttpResponse(statusCode, responseData) {
    this.statusCode = statusCode;
    this.responseData = responseData;
}

var scheduleStore = [
    {
        "userId": "7b815457-a918-438d-9697-c1c2b4905648",
        "exercises": [],
        "id": 24,
        "name": "Test_Schedule8569088",
        "url": "http://localhost:62816/api/schedule/24"
    }, {
        "userId": "7b815457-a918-438d-9697-c1c2b4905648",
        "exercises": [],
        "id": 25,
        "name": "Test_Schedule8590990",
        "url": "http://localhost:62816/api/schedule/25"
    }
];
var errorReponse = { status: 0, result: {} };

describe("CachedHttp-Tests", function () {
    var cache;
    var httpBackend;

    var validBaseUrl = 'http://fit-bachelor.azurewebsites.net/';
    var invalidBaseUrl = 'http://fit-bachelor.azurewebsites123.net/';

    var serverRoutes = {
        valid: {
            getAll: new HttpMockUpModel(validBaseUrl + 'api/schedule', null, new HttpResponse(200, scheduleStore)),
            get: new HttpMockUpModel(validBaseUrl + 'api/schedule/24', null, new HttpResponse(200, scheduleStore[0])),
            post: new HttpMockUpModel(validBaseUrl + 'api/schedule', { name: 'Test_Schedule8590990' }, new HttpResponse(200, scheduleStore[1])),
            put: new HttpMockUpModel(validBaseUrl + 'api/schedule/24', { name: 'Test_Schedule8590990' }, new HttpResponse(204, {})),
            'delete': new HttpMockUpModel(validBaseUrl + 'api/schedule/25', null, new HttpResponse(200, {}))
        },
        invalid: {
            getAll: new HttpMockUpModel(invalidBaseUrl + 'api/schedule', null, errorReponse),
            get: new HttpMockUpModel(invalidBaseUrl + 'api/schedule/24', null, errorReponse),
            post: new HttpMockUpModel(invalidBaseUrl + 'api/schedule', { name: 'Test_Schedule8590990' }, errorReponse),
            put: new HttpMockUpModel(invalidBaseUrl + 'api/schedule/24', { name: 'Test_Schedule8590990' }, errorReponse),
            'delete': new HttpMockUpModel(invalidBaseUrl + 'api/schedule/25', null, {})
        }
    };

    //initialise
    beforeEach(function () {
        module("fIT");
        jasmine.DEFAULT_TIMEOUT_INTERVAL = 300000;
    });

    beforeEach(inject(function (authFactory, $httpBackend, cachedHttp) {
        authFactory.login({
            userName: 'Stefan',
            password: 'Test1234'
        });
        httpBackend = $httpBackend;
        httpBackend
            .whenGET(serverRoutes.valid.getAll.url)
            .respond(serverRoutes.valid.getAll.receiveData.status, serverRoutes.valid.getAll.receiveData.responseData);
        cache = cachedHttp;
    }));

    // clean up
    afterEach(inject(function (authFactory) {
        authFactory.logOut();
        httpBackend.verifyNoOutstandingExpectation();
        httpBackend.verifyNoOutstandingRequest();
    }));


    it('should always pass', function () {
        expect(true).toBe(true);
    });
    it('should inplement cached-Http-Service', function () {
        expect(cache.get).toBeDefined();
        expect(cache.post).toBeDefined();
        expect(cache.put).toBeDefined();
        expect(cache.delete).toBeDefined();
    });
    it('should call http.get when using get', inject(function ($http) {
        spyOn($http, 'get');
        cache.get(serverRoutes.valid.get.url);
        expect($http.get).toHaveBeenCalled();
    }));
    it('should receive data when calling get', function (done) {

        cache.get(serverRoutes.valid.getAll.url).then(function (response) {
            expect(response).not.toBe(null);
        }).finally(function () {
            done();
        });

    });

    //it('should get all schedule-Data', function (done) {
    //    inject(function (cachedHttp, $indexedDB, dbConfig) {
    //        cachedHttp.get(serverRoutes.valid.get.url)
    //            .then(function () {
    //                $indexedDB.openStore(dbConfig.dbName, function (store) {
    //                    expect(store.count()).not.toBe(0);
    //                });
    //            })
    //            .finally(function () {
    //                done();
    //            });
    //    });
    //});
});
