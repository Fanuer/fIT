/// <reference path="C:\Users\stefan.suermann\Source\Repos\fIT\fIT.SPA\Scripts/angular-indexed-db.js" />
'use strict';
var fIT = angular.module("fIT", ["ngRoute", "LocalStorageModule", "indexedDB"]);

fIT.value("nextLocalId", 1);

//fIT.constant("baseUrl", "http://fit-bachelor.azurewebsites.net/api/");
//fIT.constant("baseUrl", "http://fit-bachelor.azurewebsites123.net/api/");
fIT.constant("baseUrl", "http://localhost:62816/api/");
fIT.constant("localStorageAuthIndex", "fIT.SPA.auth");
fIT.constant("resetDB", true);
fIT.constant("entityNames", {
  jobs: 'jobs',
  gender: 'genders',
  user: 'users',
  schedule: 'schedules',
  fitness: 'fitness'
});
fIT.constant("dbConfig", {
  dbName: 'fIT_SPADB',
  version: 1,
  tableConfigs: [
    {
      name: 'Entities',
      additional: {
        keyPath: ['localId', 'status', 'entityName']
      },
      indexes: [
        {
          name: 'localId_idx',
          column: 'localId',
          additionalData: { unique: false }
        },
        {
          name: 'status_idx',
          column: 'status',
          additionalData: { unique: false }
        },
        {
          name: 'entityName_idx',
          column: 'entityName',
          additionalData: { unique: false }
        }
      ]
    }
  ]
});

fIT.constant('cacheStatus', {
  'Server': 0,
  'Local': 1
});

fIT.config(["$routeProvider", function ($routeProvider) {
  $routeProvider.when("/", {
    controller: "scheduleController",
    templateUrl: "app/views/schedules.html"
  }).when("/schedule/:id", {
    controller: "scheduleController",
    templateUrl: "app/views/schedule.html"
  }).when("/login", {
    controller: "loginController",
    templateUrl: "app/views/login.html"
  }).when("/register", {
    controller: "signupController",
    templateUrl: "app/views/signup.html"
  }).otherwise({
    redirectTo: "/"
  });
}]);
fIT.config(function ($httpProvider) {
  $httpProvider.interceptors.push('authInterceptorFactory');
});

fIT.config(function ($indexedDBProvider, dbConfig, resetDB) {
  $indexedDBProvider
    .connection(dbConfig.dbName)
    .upgradeDatabase(dbConfig.version, function (event, database, transaction) {
      dbConfig.tableConfigs.forEach(function (entry) {
        if (resetDB) {
          try {
            database.deleteObjectStore(entry.name);
          } catch (e) {
            //$log.warn("No Objectstore to delete found: " + e);
          }
        }
        var myStore = database.createObjectStore(entry.name, entry.additional);
        if (entry.indexes != null && entry.indexes.length > 0) {
          entry.indexes.forEach(function (index) {
            myStore.createIndex(index.name, index.column, index.additionalData);
          });
        }
      });
    });
});

// on start, look for available login data
fIT.run(['authFactory', function (authFactory) {
  authFactory.fillAuthData();
}
]);

var setFirstLocalId = function ($indexedDB, dbConfig, cacheStatus, nextLocalId) {
  $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
    mystore.eachBy('status_idx', cacheStatus.Local).then(function (dbResult) {
      // fix weil eachBy immer alle einträge zurückliefert
      var maxId = 0;
      dbResult.forEach(function (value) {
        if (value.status === cacheStatus.Local && value.id > maxId) {
          maxId = value.id;
        }
      });
      nextLocalId = maxId + 1;
    });
  });
}

fIT.run(setFirstLocalId);

fIT.factory("authFactory", authFactory);
fIT.factory("enumFactory", enumFactory);
fIT.factory("scheduleFactory", scheduleFactory);
fIT.factory("authInterceptorFactory", authInterceptorFactory);
fIT.factory("cachedHttp", cachedHttp);

// controllers
fIT.controller("homeController", homeController);
fIT.controller("indexController", indexController);
fIT.controller("signupController", signupController);
fIT.controller("loginController", loginController);
fIT.controller("scheduleController", scheduleController);

// directives
fIT.directive('sameAs', sameAs);