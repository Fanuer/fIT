function cachedHttp($http, $indexedDB, $q, $log, dbConfig, nextLocalId, cacheStatus) {
    var factory = {};
    var mystore = $indexedDB.objectStore(dbConfig.dbName);

    // model fuer post data
    var syncData = function (localId, verb, url, data) {
        if (localId === null ||typeof localId === "undefined") {
            throw new Error("Parameter 'localId' is required.");
        }
        if (verb === null || typeof verb === "undefined") {
            throw new Error("Parameter 'verb' is required.");
        }
        if (url === null || typeof url === "undefined") {
            throw new Error("Parameter 'url' is required.");
        }

        this.localId = localId;
        this.verb = verb.toLowerCase();
        this.url = url;
        this.data = data;
    }

    var localDataEntry = function (serverModel, status, serverId, verb, url, data) {
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
        if (!serverModel) {
            throw new Error("No server model to transform to local model passed");
        }
        serverModel.status = status;
        if (serverId ===null || typeof serverId !== "undefined") {
            serverModel.localId = serverId;
        } else {
            serverModel.localId = nextLocalId++;
        }

        if (typeof verb !== "undefined" && typeof url !== "undefined") {
            serverModel.syncData = new syncData(serverModel.localId, verb, url, data);
        }
    };

    // verarbeitet alle offenen lokalen aenderungen
    var _sync = function () {
        var promises = [];
        //alle offenen Aenderungen holen

        var _afterSync = function (localId, data) {
            mystore.delete(localId);
            mystore.add(new localDataEntry(data, cacheStatus.Server, data.id));
        }

        var dbrequest = mystore.openCursor(IDBKeyRange.only(cacheStatus.Local));
        dbrequest.onsuccess(function (ev) {
            if (typeof ev.target.result === "undefined" && ev.target.result.length > 0) {
                ev.target.result.forEach(function (entry) {
                    try {
                        if (typeof entry !== "undefined" && typeof entry.syncData !== "undefined") {
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
                                    .then(function (response) { _afterSync(localId, response) });
                                    break;
                                case 'put':
                                case 'patch':
                                    var prom = $http[entry.syncData.verb](entry.syncData.url, entry.syncData.data);
                                    prom.then(function () {
                                        return $http.get(entry.syncData.url);
                                    })
                                    .then(function (response) { _afterSync(localId, response) });
                                    break;
                            }
                            if (prom && !prom.isRejected) {
                                promises.push(prom);
                            }

                            //setze status um
                            prom.then(function () {
                                entry.status = cacheStatus.Server;
                                mystore.put(entry);
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
    }

    var _get = function (url, localId) {
        var deferred = $q.defer();

        $http.get(url)
          .then(function (response) {
              // wenn das senden erfolgreich ist, wird die Antwort lokal gespeichert
              var localEntity = new localDataEntry(response, cacheStatus.Server, response.id);
              var dbrequest = mystore.get(IDBKeyRange.only(localDataEntry.localId));
              dbrequest.onsuccess(function (ev) {
                  // wenn der curser erzeugt werden konnte, ist der Datensatz vorhanden -> update
                  if (ev.target.result) {
                      mystore.put(localEntity);
                  } else {
                      mystore.add(localEntity);
                  }
              });
              _sync();
              deferred.resolve(response);
          })
          .catch(function (response) {
              if (response.statusCode === 0 && typeof localId !== "undefined") {
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
              }
              deferred.reject(message);
          });
        return deferred.promise;
    }

    var _post = function (url, data) {
        var deferred = $q.defer();

        $http.post(url, data)
          .then(function (response) {
              var id = response.id;
              _get(url, id);
          })
          .catch(function (response) {
              var localData = new localDataEntry(response, cacheStatus.Local, undefined, 'post', url, response);
              mystore.add(localData);
          });
        return deferred.promise;
    }

    var _put = function (url, data) {
        var deferred = $q.defer();

        $http.put(url, data)
          .then(function (response) {
              var id = response.id;
              _get(url, id);
          })
          .catch(function (response) {
              var localData = new localDataEntry(response, cacheStatus.Local, undefined, 'put', url, response);
              mystore.put(localData);
          });
        return deferred.promise;
    }

    var _delete = function (url, data) {
        var deferred = $q.defer();

        $http.delete(url, data)
            .then(function(response) {
                var localData = new localDataEntry(response, cacheStatus.Local);
                mystore.delete(localData);
            });
        return deferred.promise;
    }

    factory.get = _get;
    factory.post = _post;
    factory.put = _put;
    factory.delete = _delete;
    return factory;
}