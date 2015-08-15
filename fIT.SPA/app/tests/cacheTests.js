/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/jasmine/jasmine-html.js" />
/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/jasmine/jasmine.js" />
/// <reference path="D:\Projekte\dotnet\Src\fIT\fIT.SPA\Scripts/angular.min.js" />
/// <reference path="../factory/cachedHttpFactory.js" />

describe("CachedHttp-Tests", function () {
    var cache;

    //initialise
    beforeEach(function () {
        var injector = angular.injector(['ng', 'fIT']);
        var $http = injector.get('$http');
        var $indexedDb = injector.get('$indexedDB');
        var $log = injector.get('$log');
        var $q = injector.get('$q');
        var dbConfig = injector.get('dbConfig');
        var nextLocalId = injector.get('nextLocalId');
        var cacheStatus = injector.get('cacheStatus');
        cache = new cachedHttp($http, $indexedDb, $q, $log, dbConfig, nextLocalId, cacheStatus);
    });

    // clean up
    afterEach(function () { });


    it('should pass', function() {
        expect(true).toBe(true);
    });
})
