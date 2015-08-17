function cachedHttp($http, $indexedDB, $q, $log, dbConfig, nextLocalId, cacheStatus) {
    var factory = {};
    var alreadySetUp = false;

    // model fuer post data
    function isNotNullOrUndefined(object) {
        return object !== null && typeof object !== "undefined";
    }
    function syncData(localId, verb, url, data) {
        if (!isNotNullOrUndefined(localId)) {
            throw new Error("Parameter 'localId' is required.");
        }
        if (!isNotNullOrUndefined(verb)) {
            throw new Error("Parameter 'verb' is required.");
        }
        if (!!isNotNullOrUndefined(url)) {
            throw new Error("Parameter 'url' is required.");
        }

        this.localId = localId;
        this.verb = verb.toLowerCase();
        this.url = url;
        this.data = data;
    }
    function localDataEntry(serverModel, status, entityName, serverId, verb, url, data) {
        /// <summary>Erweitert ein servermodel um Daten fuer die lokale Haltung</summary> 
        /// <param name="serverModel" optional="false" type="object">Model mit Stammdaten vom/fuer den Server. Diese werden um eine lokale ID und den aktuellen Synchronisationsstatus</param>
        /// <param name="status" type="cacheStatus" optional="false">
        /// aktueller Status der Synchronisation. 
        /// Server = Daten sind mit dem Server synchron. 
        /// Local = Daten muessen noch synchronisiert werden
        /// </param>
        /// <param name="serverId" type="integer" optional="true">Falls eine ServerId vorhanden ist, wird diese ebenfalls als localId genutzt, andernfalls wird eine neue localId erzeugt</param>
        /// <param name="verb" type="string" optional="false">Daten fuer spaetere Synchronisation: Http Verb</param>
        /// <param name="url" type="string" optional="false">URL , welche mit dem Verb an den Server gesandt wird</param>
        /// <param name="data" type="object" optional="true">Daten, welche als Content zum Server gesendet werden</param>
        if (!isNotNullOrUndefined(serverModel)) {
            throw new Error("No server model to transform to local model passed");
        }
        if (!isNotNullOrUndefined(status)) {
            throw new Error("Status for local cache object have to be passed");
        }
        if (!isNotNullOrUndefined(entityName)) {
            throw new Error("Entityname for local cache object have to be passed");
        }

        serverModel.status = status;
        serverModel.entityName = entityName;
        if (serverId === null || typeof serverId !== "undefined") {
            serverModel.localId = serverId;
        } else {
            serverModel.localId = nextLocalId++;
        }

        if (typeof verb !== "undefined" && typeof url !== "undefined") {
            serverModel.syncData = new syncData(serverModel.localId, verb, url, data);
        }
        return serverModel;
    }

    // verarbeitet alle offenen lokalen aenderungen
    var _sync = function () {
        var promises = [];
        //alle offenen Aenderungen holen

        function afterSync(localId, data) {
            $indexedDB.openStore(dbConfig.dbName, function (mystore) {
                mystore.delete(localId);
                mystore.add(new localDataEntry(data, cacheStatus.Server, data.id));
            });
        }

        $indexedDB.openStore(dbConfig.dbName, function (mystore) {
            var dbrequest = mystore.openCursor(IDBKeyRange.only(cacheStatus.Local));
            dbrequest.onsuccess(function (ev) {
                if (isNotNullOrUndefined(ev.target.result) && ev.target.result.length > 0) {
                    ev.target.result.forEach(function (entry) {
                        try {
                            if (isNotNullOrUndefined(entry) && isNotNullOrUndefined(entry.syncData)) {
                                switch (entry.syncData.verb) {
                                    case 'get':
                                    case 'delete':
                                    case 'head':
                                    case 'jsonp':
                                        var prom = $http[entry.syncData.verb](entry.syncData.url);
                                        break;
                                    case 'post':
                                        var prom = $http[entry.syncData.verb](entry.syncData.url, entry.syncData.data);
                                        prom.then(function (response) {
                                            return $http.get(entry.syncData.url + response.id);
                                        })
                                        .then(function (response) { afterSync(localId, response) });
                                        break;
                                    case 'put':
                                    case 'patch':
                                        var prom = $http[entry.syncData.verb](entry.syncData.url, entry.syncData.data);
                                        prom.then(function () {
                                            return $http.get(entry.syncData.url);
                                        })
                                        .then(function (response) { afterSync(localId, response) });
                                        break;
                                }
                                if (prom && !prom.isRejected) {
                                    promises.push(prom);
                                }

                                //setze status um
                                prom.then(function () {
                                    entry.status = cacheStatus.Server;
                                    $indexedDB.openStore(dbConfig.dbName, function () {
                                        mystore.put(entry);
                                    });

                                });
                            }
                        } catch (e) {
                            $log.error('Error on sync: ' + e);
                        }
                    });
                }
                return $q.all(promises);
            });
            dbrequest.onerror(function (msg) {
                $log.error('Unable to open sync-cursor: ' + msg);
            });

        });
    }

    var _get = function (url, entityName, localId) {
        var deferred = $q.defer();
        $http.get(url)
          .then(function (response) {
              var array = new Array();
              // wenn das senden erfolgreich ist, wird die Antwort lokal gespeichert
              if (!angular.isArray(response.data)) {
                  array.push(response.data);
              } else {
                  array = response.data;
              }

              $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (store) {
                  array.forEach(function (value, index) {
                      var localEntity = new localDataEntry(value, cacheStatus.Server, entityName, value.id);
                      store.upsert(localEntity).then(function(dbReponse) {
                          $log.info(dbReponse);
                      }).catch(function (dbReponse) {
                          $log.error(dbReponse);
                      });
                  });
              })
              .catch(function (response) {
                  $log.error(response);
              });
              _sync();
              deferred.resolve(response);
          })
          .catch(function (response) {
              if (response.statusCode === 0 && typeof localId !== "undefined") {
                  $indexedDB.openStore(dbConfig.dbName, function (mystore) {
                      var dbrequest = mystore.get(IDBKeyRange.only(localId));
                      dbrequest.onsuccess(function (ev) {
                          // wenn der curser erzeigt werden konnte, ist der Datensatz vorhanden -> update
                          if (ev.target.result) {
                              deferred.resolve(ev.target.result);
                          } else {
                              deferred.reject("No data found");
                          }
                          return;
                      });
                  });
              }
              deferred.reject(response.statusText);
          });
        return deferred.promise;
    }

    var _post = function (url, data, entityName) {
        var deferred = $q.defer();

        $http.post(url, data)
          .then(function (response) {
              var id = response.data.id;
              _get(url, entityName, id);
              deferred.resolve(response.data);
          })
          .catch(function (response) {
              if (response.status === 0) {
                  var localData = new localDataEntry(response.data, cacheStatus.Local, entityName, undefined, 'post', url, response.data);

                  $indexedDB.openStore(dbConfig.dbName, function (mystore) {
                      mystore.add(localData);
                      deferred.resolve(localData);
                  }).catch(function (response) {
                      $log.error(response);
                      deferred.reject(response);
                  });
              } else {
                  deferred.reject(response);
              }
          });
        return deferred.promise;
    }

    var _put = function (url, data, entityName) {
        var deferred = $q.defer();

        $http.put(url, data)
          .then(function (response) {
              var id = data.id;
              _get(url, entityName, id);
              deferred.resolve();
          })
          .catch(function (response) {
              if (response.status === 0) {
                  var localData = new localDataEntry(response.data, cacheStatus.Local, entityName, undefined, 'put', url, response.data);
                  $indexedDB.openStore(dbConfig.dbName, function (mystore) {
                      mystore.put(localData);
                      deferred.resolve();
                  })
                  .catch(function (dbresponse) {
                      $log.error(dbresponse);
                      deferred.reject(dbresponse);
                  });
              } else {
                  deferred.reject(response);
              }
          });
        return deferred.promise;
    }

    var _delete = function (url, entityName, localId) {
        var deferred = $q.defer();

        $http.delete(url)
            .then(function (response) {
                var localData = new localDataEntry({}, cacheStatus.Server, entityName, localId);
                $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                    mystore.delete([localData.localId, localData.status, localData.entityName]);
                    deferred.resolve();
                })
                .catch(function (response) {
                    $log.error(response);
                    deferred.reject(response);
                });
            }).catch(function (response) {
                deferred.reject(response);
            });
        return deferred.promise;
    }

    factory.get = _get;
    factory.post = _post;
    factory.put = _put;
    factory.delete = _delete;
    return factory;
}