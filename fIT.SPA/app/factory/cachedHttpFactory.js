function cachedHttp($http, $indexedDB, $q, $log, dbConfig, nextLocalId, cacheStatus) {
  var factory = {};
  var mystore = $indexedDB.objectStore(dbConfig.dbName);

  // model fuer post data
  var syncData = function (localId, verb, url, data) {
    if (typeof localId === "undefined") {
      throw new Error("Parameter 'localId' is required.");
    }
    if (typeof verb === "undefined") {
      throw new Error("Parameter 'verb' is required.");
    }
    if (typeof url === "undefined") {
      throw new Error("Parameter 'url' is required.");
    }
    
    this.localId = localId;
    this.verb = verb;
    this.url = url;
    this.data = data;
  }

  // erweitert ein servermodel um daten fuer die lokale Haltung
  var localDataEntry = function (serverModel, status, serverId) {
    if (!serverModel) {
      throw new Error("No server model to transform to local model passed");
    }
    serverModel.status = status;
    if (typeof serverId !== "undefined") {
      serverModel.localId = serverId;
    } else {
      serverModel.localId = nextLocalId++;
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
            if (typeof entry !== "undefined" && typeof entry.httpdata !== "undefined") {
              switch (entry.httpdata.verb) {
                case 'get':
                case 'delete':
                case 'head':
                case 'jsonp':
                  var prom = $http[entry.httpdata.verb](entry.httpdata.url);
                  break;
                case 'post':
                  var prom = $http[entry.httpdata.verb](entry.httpdata.url, entry.httpdata.data);
                  prom.then(function (response) {
                    return $http.get(entry.httpdata.url + response.id);
                  })
                  .then(function (response) { _afterSync(localId, response) });
                  break;
                case 'put':
                case 'patch':
                  var prom = $http[entry.httpdata.verb](entry.httpdata.url, entry.httpdata.data);
                  prom.then(function() {
                    return $http.get(entry.httpdata.url);
                  })
                  .then(function (response) { _afterSync(localId, response) });
                  break;
              }
              if (prom && !prom.isRejected) {
                promises.push(prom);
              }

              //setze status um
              prom.then(function() {
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
    dbrequest.onerror(function(msg) {
      $log.error('Unable to open sync-cursor: ' + msg);
    });
  }

  var _get = function (localId, url, data) {
    var deferred = $q.defer();
    
    $http.get(url, data)
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

  return factory;
}