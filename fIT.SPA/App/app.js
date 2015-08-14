/// <reference path="C:\Users\stefan.suermann\Source\Repos\fIT\fIT.SPA\Scripts/angular-indexed-db.js" />
'use strict';
var fIT = angular.module("fIT", ["ngRoute", "LocalStorageModule", "indexedDB"]);

fIT.value("nextLocalId", 0);

fIT.constant("baseUrl", "http://fit-bachelor.azurewebsites1.net/api/");
fIT.constant("localStorageAuthIndex", "fIT.SPA.auth");
fIT.constant("dbConfig", {
  dbName: 'fIT_SPADB',
  version: 1,
  tableConfigs: [
    {
      name: 'Schedules',
      additional: {
        autoIncrement: true,
        keyPath: ['localId']
      },
      indexes: [
        {
          name: 'localId_idx',
          column: 'localId',
          additionalData: { unique: true }
        },
        {
          name: 'status_idx',
          column: 'status',
          additionalData: { unique: false }
        },
        {
          name: 'serverId_idx',
          column: 'ServerId',
          additionalData: { unique: true }
        }
      ]
    },
    {
      name: 'syncQueue'
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
fIT.config(function ($indexedDBProvider, dbConfig) {
  $indexedDBProvider
    .connection(dbConfig.dbName)
    .upgradeDatabase(dbConfig.version, function (event, database, transaction) {
      dbConfig.tableConfigs.forEach(function (entry) {
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

fIT.factory("authFactory", authFactory);
fIT.factory("enumFactory", enumFactory);
fIT.factory("scheduleFactory", scheduleFactory);
fIT.factory("authInterceptorFactory", authInterceptorFactory);

// controllers
fIT.controller("homeController", homeController);
fIT.controller("indexController", indexController);
fIT.controller("signupController", signupController);
fIT.controller("loginController", loginController);
fIT.controller("scheduleController", scheduleController);

// directives
fIT.directive('sameAs', sameAs);