﻿function cachedHttp($http, $indexedDB, $q, $log, dbConfig, nextLocalId, cacheStatus) {
    var factory = {};

    // model fuer post data
    function isNotNullOrUndefined(object) {
        return object !== null && typeof object !== "undefined";
    }
    if (!String.prototype.endsWith) {
        String.prototype.endsWith = function (searchString, position) {
            var subjectString = this.toString();
            if (position === undefined || position > subjectString.length) {
                position = subjectString.length;
            }
            position -= searchString.length;
            var lastIndex = subjectString.indexOf(searchString, position);
            return lastIndex !== -1 && lastIndex === position;
        };
    }
    function syncData(localId, verb, url, data) {
        if (!isNotNullOrUndefined(localId)) {
            throw new Error("Parameter 'localId' is required.");
        }
        if (!isNotNullOrUndefined(verb)) {
            throw new Error("Parameter 'verb' is required.");
        }
        if (!isNotNullOrUndefined(url)) {
            throw new Error("Parameter 'url' is required.");
        }

        if (data) {
            if (data.localId) {
                delete data.localId;
            }
            if (data.syncData) {
                delete data.syncData;
            }
            if (data.status) {
                delete data.status;
            }
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
        var result = new Object();
        if (isNotNullOrUndefined(serverModel)) {
            for (var k in serverModel) {
                if (serverModel.hasOwnProperty(k)) {
                    result[k] = serverModel[k];
                }
            }
        }

        if (!isNotNullOrUndefined(status)) {
            throw new Error("Status for local cache object have to be passed");
        }
        if (!isNotNullOrUndefined(entityName)) {
            throw new Error("Entityname for local cache object have to be passed");
        }

        result.status = status;
        result.entityName = entityName;
        if (serverId === null || typeof serverId !== "undefined") {
            result.localId = serverId;
        } else {
            result.localId = nextLocalId++;
        }

        if (typeof result.syncData === "undefined") {
            result.syncData = new Object();
        }
        if (typeof verb !== "undefined" && typeof url !== "undefined") {
            var newSyncData = new syncData(result.localId, verb, url, data);
            result.syncData[verb] = newSyncData;
        }
        return result;
    }

    // verarbeitet alle offenen lokalen aenderungen
    var _sync = function () {
        var resultPromise = $q.defer();
        var promises = [];

        //alle offenen Aenderungen holen
        var afterSync = function (localId, data, entityName, verb) {

            var dbOpenRequest = indexedDB.open(dbConfig.dbName);
            dbOpenRequest.onsuccess = function () {
                var db = dbOpenRequest.result;
                var tx = db.transaction(dbConfig.tableConfigs[0].name, "readwrite");
                var store = tx.objectStore(dbConfig.tableConfigs[0].name);
                var localKey = [localId, cacheStatus.Local, entityName];

                var getRequest = store.get(localKey);
                getRequest.onsuccess = function (e) {
                    var remove = true;
                    var dbresult = e.target.result;
                    if (dbresult && dbresult.syncData) {
                        delete dbresult.syncData[verb];

                        for (var prop in dbresult.syncData) {
                            if (dbresult.hasOwnProperty(prop)) {
                                remove = false;
                                break;
                            }
                        }
                        var changeRequest = null;
                        if (remove) {
                            changeRequest = store.delete(localKey);
                        } else {
                            changeRequest = store.put(dbresult);
                        }

                        changeRequest.onsuccess = function () {
                            var serverObject = new localDataEntry(data || {}, cacheStatus.Server, entityName, data.id);
                            var finalRequest = store.put(serverObject);
                            finalRequest.onsuccess = function () {
                                $log.info('DBEntry "' + data.id + '" synchronisiert');
                            };
                            finalRequest.onerror = function () {
                                $log.warn('DBEntry "' + data.id + '" konnte nicht synchronisiert werden (Fehler beim Neuerstellen)');
                            };
                        }
                    };
                };
                getRequest.onerror = function (e) {
                    $log.error(e);
                };
            };
        }
        var dbresult = new Array();
        $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
            mystore.eachBy('status_idx', cacheStatus.Local).then(function (dbresults) {
                //fix weil eachBy immer alles zurueckliefert
                dbresult = dbresults.filter(function (value) { return value.status === cacheStatus.Local });
            });
        }).then(function () {
            if (dbresult.length > 0) {
                dbresult.forEach(function (entry) {
                    try {
                        if (isNotNullOrUndefined(entry) && isNotNullOrUndefined(entry.syncData)) {
                            if (entry.syncData.delete) {
                                var prom = $q.defer();
                                promises.push(prom.promise);
                                entry.syncData.delete.url = entry.syncData.delete.url.replace("123", "");
                                $http.delete(entry.syncData.delete.url).then(function (response) {
                                    $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                                        mystore.delete([entry.localId, entry.status, entry.entityName]).then(function (dbresponse) {
                                            $log.info('Lokaler DbEntry erfolgreich gelöscht');
                                            prom.resolve();
                                        }).catch(function (dbresponse) {
                                            $log.error('Lokaler DBEntry konnte nicht gelöscht werden: ' + dbresponse);
                                            prom.reject();
                                        });
                                    });
                                }).catch(function (response) {
                                    if (response.status === 404) {
                                        $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                                            mystore.delete([entry.localId, entry.status, entry.entityName]).then(function (dbresponse) {
                                                $log.info('Lokaler DbEntry erfoglreich gelöscht');
                                                prom.resolve();
                                            }).catch(function (dbresponse) {
                                                $log.error('Lokaler DBEntry konnte nicht gelöscht werden: ' + dbresponse);
                                                prom.reject();
                                            });
                                        });
                                    } else {
                                        $log.error(response.message);
                                    }
                                });
                                // nach einem Delete kann das serverseitige Object nicht mehr verändert werden
                                return;
                            } else {
                                var oldlocalId;
                                if (entry.syncData.post) {
                                    oldlocalId = entry.syncData.post.localId;
                                    entry.syncData.post.url = entry.syncData.post.url.replace("fit-bachelor.azurewebsites123.net", "localhost:62816");

                                    prom = $q.defer();
                                    promises.push(prom.promise);
                                    $http.post(entry.syncData.post.url, entry.syncData.post.data).then(function (response) {
                                        return $http.get(entry.syncData.post.url + (entry.syncData.post.url.endsWith("/") ? "" : "/") + response.data.id);
                                    }).then(function (response) {
                                        afterSync(oldlocalId, response.data, entry.entityName, "post");
                                        entry.localId = response.data.id;
                                    }).then(function () {
                                        if (entry.syncData.put) {
                                            entry.syncData.put.url = entry.syncData.put.url
                                              .replace("fit-bachelor.azurewebsites123.net", "localhost:62816")
                                              .replace('/' + oldlocalId, "/" + entry.localId);
                                            entry.syncData.put.data.id = entry.localId;
                                            $http.put(entry.syncData.put.url, entry.syncData.put.data).then(function (response) {
                                                return $http.get(entry.syncData.put.url);
                                            }).then(function (response) {
                                                afterSync(oldlocalId, response.data, entry.entityName, "put");
                                                prom.resolve();
                                            }).catch(function (error) {
                                                prom.reject(error);
                                            });
                                        } else {
                                            prom.resolve();
                                        }
                                    }).catch(function () {
                                        prom.reject();
                                    });
                                } else if (entry.syncData.put) {
                                    oldlocalId = entry.syncData.put.localId;
                                    prom = $q.defer();
                                    promises.push(prom.promise);
                                    entry.syncData.put.url = entry.syncData.put.url.replace("fit-bachelor.azurewebsites123.net", "localhost:62816");
                                    $http.put(entry.syncData.put.url, entry.syncData.put.data).then(function (response) {
                                        return $http.get(entry.syncData.put.url);
                                    }).then(function (response) {
                                        afterSync(oldlocalId, response.data, entry.entityName, "put");
                                        prom.resolve();
                                    }).catch(function (response) {
                                        if (response && response.status && response.status === 404) {
                                            $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                                                mystore.delete([oldlocalId, cacheStatus.Local, entry.entityName]).then(function () {
                                                    prom.resolve();
                                                });
                                            });
                                            $log.info("Lokalen Eintrag gelöscht, da der Eintrag auf dem Server nicht mehr vorhanden war");
                                        } else if (response.message) {
                                            $log.error(response.message);
                                        } else {
                                            $log.error(response);
                                        }
                                        prom.reject();
                                    });
                                }
                            }
                        }
                    } catch (e) {
                        $log.error('Error on sync: ' + e);
                    }
                });
            }
        }).then(function () {
            $q.all(promises).then(function () {
                resultPromise.resolve();
            })
            .catch(function (error) {
                $log.error(error);
                resultPromise.reject();
            });
        });
        return resultPromise.promise;
    }

    var _get = function (url, entityName, localId) {
        if (localId) {
            localId = parseInt(localId);
            if (isNaN(localId)) {
                throw new Error('The local id must be a numeric value');
            }
        }

        var deferred = $q.defer();
        $http.get(url)
          .then(function (response) {
              var array = new Array();
              var useLocal = false;
              // wenn das senden erfolgreich ist, wird die Antwort lokal gespeichert
              if (!angular.isArray(response.data)) {
                  array.push(response.data);
              } else {
                  array = response.data;
              }
              var deleteEntries = new Array();
              $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (store) {
                  store.eachBy('entityName_idx', entityName).then(function (dbResult) {
                      var serverIds = new Array();
                      array.forEach(function (value) {
                          serverIds.push(value.id);
                      });

                      // wenn nicht mit einer id gesucht wurde
                      if (typeof localId === "undefined") {
                          dbResult.filter(function (obj) {
                              return obj.entityName === entityName && serverIds.filter(function (value) { return value === obj.localId; }).length === 0 && typeof obj.syncData.post === "undefined" && typeof obj.syncData.put === "undefined";
                          }).forEach(function (value) {
                              deleteEntries.push([value.localId, value.status, value.entityName]);
                          });
                      }
                  }).then(function () {
                      array.forEach(function (value, index) {
                          var localEntity = new localDataEntry(value, cacheStatus.Server, entityName, value.id);
                          store.upsert(localEntity).then(function (dbReponse) {
                              $log.info(dbReponse);
                          }).catch(function (dbReponse) {
                              $log.error(dbReponse);
                          });
                      });
                  }).then(function () {
                      useLocal = true;
                      _sync().then(function () {
                          $http.get(url).then(function (response) {
                              var syncData = new Array();
                              if (!angular.isArray(response.data)) {
                                  syncData.push(response.data);
                              } else {
                                  syncData = response.data;
                              }
                              var result = new Array();
                              syncData.forEach(function (value) {
                                  result.push(new localDataEntry(value, cacheStatus.Server, entityName, value.id));
                              });
                              return result;
                          })
                          .then(function (result) {
                              deferred.resolve(result);
                          })
                          .catch(function (error) {
                              deferred.reject(error);
                          });
                      });
                  }).then(function () {
                      deleteEntries.forEach(function (value) {
                          store.delete(value).then(function () {
                              $log.info("Lokalen Eintrag, welcher nicht mehr auf dem Server vorhanden ist, wurde gelöscht");
                          }).catch(function (error) {
                              $log.info("Lokalen Eintrag, welcher nicht mehr auf dem Server vorhanden ist, konnte nicht gelöscht werden: " + error);
                          });
                      });
                  });
              }).catch(function (response) {
                  $log.error(response);
                  deferred.reject(response);
              }).finally(function () {
                  // fallcack; wenn irgendwo ein Fehler passiert, wird die erste Antwort vom Server benutzt
                  var result = new Array();
                  if (!useLocal) {
                      array.forEach(function (value) {
                          var localEntity = new localDataEntry(value, cacheStatus.Server, entityName, value.id);
                          result.push(localEntity);
                      });
                      if (result.length === 1) {
                          deferred.resolve(result[0]);
                      } else {
                          deferred.resolve(result);
                      }
                  }
              });

          })
          .catch(function (response) {
              if (response.status === 0) {
                  if (typeof localId !== "undefined") {
                      $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                          //mystore.find([81, 0, "schedules"]).then(function (dbresult) {
                          mystore.find([localId, cacheStatus.Server, entityName]).then(function (dbresult) {
                              deferred.resolve(dbresult);
                          }).catch(function (dbresult) {
                              mystore.find([localId, cacheStatus.Local, entityName]).then(function (localDbResult) {
                                  deferred.resolve(localDbResult);
                              }).catch(function (localDbResult) {
                                  deferred.reject(localDbResult);
                              });
                          });
                      });
                  } else {
                      $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                          mystore.eachBy('entityName_idx', entityName).then(function (dbresult) {
                              deferred.resolve(dbresult.filter(function (obj) {
                                  return !isNotNullOrUndefined(obj.syncData) || !isNotNullOrUndefined(obj.syncData.delete);
                              }));
                          }).catch(function (dbresult) {
                              deferred.reject(dbresult);
                          });
                      });
                  };
              } else {
                  deferred.reject(response);
              }
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
                  var localData = new localDataEntry(data, cacheStatus.Local, entityName, undefined, 'post', url, data);

                  $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                      mystore.upsert(localData).then(function (result) {
                          deferred.resolve(localData);
                      });
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
                  var localId = data.localId;
                  var localData = new localDataEntry(data, cacheStatus.Local, entityName, localId, 'put', url, data);
                  $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                      mystore.delete([localId, cacheStatus.Server, entityName]).then(function (result) {
                          return mystore.upsert(localData);
                      }).then(function (result) {
                          deferred.resolve(localData);
                      });
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
                if (response.status === 0) {
                    var noUpsert = false;
                    var dbFoundElement = null;
                    $indexedDB.openStore(dbConfig.tableConfigs[0].name, function (mystore) {
                        var localData = null;
                        var findpormise = $q.defer();
                        mystore.find([localId, cacheStatus.Server, entityName])
                            .then(function (dbResult) {
                                dbFoundElement = dbResult;
                                return mystore.delete([dbResult.localId, dbResult.status, dbResult.entityName]);
                            })
                            .then(function () {
                                localData = dbFoundElement;
                                localData.status = cacheStatus.Local;
                                localData.syncData = localData.syncData || {};
                                // if an object is created and deleted locally, there is no need to store the data to send them to the Server
                                localData.syncData.delete = new syncData(localId, 'delete', url);
                                findpormise.resolve(localData);
                            }).catch(function () {
                                mystore.find([localId, cacheStatus.Local, entityName]).then(function (dbresult) {
                                    localData = dbresult;
                                    localData.status = cacheStatus.Local;
                                    localData.syncData = localData.syncData || {};
                                    // if an object is created and deleted locally, there is no need to store the data to send them to the Server
                                    localData.syncData.delete = new syncData(localId, 'delete', url);
                                    findpormise.resolve(localData);
                                }).catch(function () {
                                    localData = new localDataEntry({}, cacheStatus.Local, entityName, localId, 'delete', url);
                                    findpormise.resolve(localData);
                                });
                            });

                        findpormise.promise.then(function (localData) {
                            if (typeof localData.syncData.post !== "undefined") {
                                mystore.delete([localId, cacheStatus.Local, entityName]);
                                deferred.resolve(true);
                            } else {
                                mystore.delete([localData.localId, localData.status, localData.entityName]).then(function (result) {
                                    return mystore.upsert(localData);
                                }).then(function (result) {
                                    deferred.resolve(localData);
                                });
                            }
                        });
                    }).catch(function (response) {
                        $log.error(response);
                        deferred.reject(response);
                    });
                }
            });
        return deferred.promise;
    }

    factory.get = _get;
    factory.post = _post;
    factory.put = _put;
    factory.delete = _delete;
    return factory;
}