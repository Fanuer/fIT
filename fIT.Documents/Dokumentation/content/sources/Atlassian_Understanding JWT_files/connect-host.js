(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        //Allow using this built library as an AMD module
        //in another project. That other project will only
        //see this AMD call, not the internal modules in
        //the closure below.
        define("connect-host", [], factory);
    } else {
        //Browser globals case. Just assign the
        //result to a property on the global.

        if(!window._AP){
            window._AP = {};
        }

        AJS.$.extend(_AP, factory());
    }
}(this, function () {
/**
 * @license almond 0.3.0 Copyright (c) 2011-2014, The Dojo Foundation All Rights Reserved.
 * Available via the MIT or new BSD license.
 * see: http://github.com/jrburke/almond for details
 */
//Going sloppy to avoid 'use strict' string cost, but strict practices should
//be followed.
/*jslint sloppy: true */
/*global setTimeout: false */

var requirejs, require, define;
(function (undef) {
    var main, req, makeMap, handlers,
        defined = {},
        waiting = {},
        config = {},
        defining = {},
        hasOwn = Object.prototype.hasOwnProperty,
        aps = [].slice,
        jsSuffixRegExp = /\.js$/;

    function hasProp(obj, prop) {
        return hasOwn.call(obj, prop);
    }

    /**
     * Given a relative module name, like ./something, normalize it to
     * a real name that can be mapped to a path.
     * @param {String} name the relative name
     * @param {String} baseName a real name that the name arg is relative
     * to.
     * @returns {String} normalized name
     */
    function normalize(name, baseName) {
        var nameParts, nameSegment, mapValue, foundMap, lastIndex,
            foundI, foundStarMap, starI, i, j, part,
            baseParts = baseName && baseName.split("/"),
            map = config.map,
            starMap = (map && map['*']) || {};

        //Adjust any relative paths.
        if (name && name.charAt(0) === ".") {
            //If have a base name, try to normalize against it,
            //otherwise, assume it is a top-level require that will
            //be relative to baseUrl in the end.
            if (baseName) {
                //Convert baseName to array, and lop off the last part,
                //so that . matches that "directory" and not name of the baseName's
                //module. For instance, baseName of "one/two/three", maps to
                //"one/two/three.js", but we want the directory, "one/two" for
                //this normalization.
                baseParts = baseParts.slice(0, baseParts.length - 1);
                name = name.split('/');
                lastIndex = name.length - 1;

                // Node .js allowance:
                if (config.nodeIdCompat && jsSuffixRegExp.test(name[lastIndex])) {
                    name[lastIndex] = name[lastIndex].replace(jsSuffixRegExp, '');
                }

                name = baseParts.concat(name);

                //start trimDots
                for (i = 0; i < name.length; i += 1) {
                    part = name[i];
                    if (part === ".") {
                        name.splice(i, 1);
                        i -= 1;
                    } else if (part === "..") {
                        if (i === 1 && (name[2] === '..' || name[0] === '..')) {
                            //End of the line. Keep at least one non-dot
                            //path segment at the front so it can be mapped
                            //correctly to disk. Otherwise, there is likely
                            //no path mapping for a path starting with '..'.
                            //This can still fail, but catches the most reasonable
                            //uses of ..
                            break;
                        } else if (i > 0) {
                            name.splice(i - 1, 2);
                            i -= 2;
                        }
                    }
                }
                //end trimDots

                name = name.join("/");
            } else if (name.indexOf('./') === 0) {
                // No baseName, so this is ID is resolved relative
                // to baseUrl, pull off the leading dot.
                name = name.substring(2);
            }
        }

        //Apply map config if available.
        if ((baseParts || starMap) && map) {
            nameParts = name.split('/');

            for (i = nameParts.length; i > 0; i -= 1) {
                nameSegment = nameParts.slice(0, i).join("/");

                if (baseParts) {
                    //Find the longest baseName segment match in the config.
                    //So, do joins on the biggest to smallest lengths of baseParts.
                    for (j = baseParts.length; j > 0; j -= 1) {
                        mapValue = map[baseParts.slice(0, j).join('/')];

                        //baseName segment has  config, find if it has one for
                        //this name.
                        if (mapValue) {
                            mapValue = mapValue[nameSegment];
                            if (mapValue) {
                                //Match, update name to the new value.
                                foundMap = mapValue;
                                foundI = i;
                                break;
                            }
                        }
                    }
                }

                if (foundMap) {
                    break;
                }

                //Check for a star map match, but just hold on to it,
                //if there is a shorter segment match later in a matching
                //config, then favor over this star map.
                if (!foundStarMap && starMap && starMap[nameSegment]) {
                    foundStarMap = starMap[nameSegment];
                    starI = i;
                }
            }

            if (!foundMap && foundStarMap) {
                foundMap = foundStarMap;
                foundI = starI;
            }

            if (foundMap) {
                nameParts.splice(0, foundI, foundMap);
                name = nameParts.join('/');
            }
        }

        return name;
    }

    function makeRequire(relName, forceSync) {
        return function () {
            //A version of a require function that passes a moduleName
            //value for items that may need to
            //look up paths relative to the moduleName
            var args = aps.call(arguments, 0);

            //If first arg is not require('string'), and there is only
            //one arg, it is the array form without a callback. Insert
            //a null so that the following concat is correct.
            if (typeof args[0] !== 'string' && args.length === 1) {
                args.push(null);
            }
            return req.apply(undef, args.concat([relName, forceSync]));
        };
    }

    function makeNormalize(relName) {
        return function (name) {
            return normalize(name, relName);
        };
    }

    function makeLoad(depName) {
        return function (value) {
            defined[depName] = value;
        };
    }

    function callDep(name) {
        if (hasProp(waiting, name)) {
            var args = waiting[name];
            delete waiting[name];
            defining[name] = true;
            main.apply(undef, args);
        }

        if (!hasProp(defined, name) && !hasProp(defining, name)) {
            throw new Error('No ' + name);
        }
        return defined[name];
    }

    //Turns a plugin!resource to [plugin, resource]
    //with the plugin being undefined if the name
    //did not have a plugin prefix.
    function splitPrefix(name) {
        var prefix,
            index = name ? name.indexOf('!') : -1;
        if (index > -1) {
            prefix = name.substring(0, index);
            name = name.substring(index + 1, name.length);
        }
        return [prefix, name];
    }

    /**
     * Makes a name map, normalizing the name, and using a plugin
     * for normalization if necessary. Grabs a ref to plugin
     * too, as an optimization.
     */
    makeMap = function (name, relName) {
        var plugin,
            parts = splitPrefix(name),
            prefix = parts[0];

        name = parts[1];

        if (prefix) {
            prefix = normalize(prefix, relName);
            plugin = callDep(prefix);
        }

        //Normalize according
        if (prefix) {
            if (plugin && plugin.normalize) {
                name = plugin.normalize(name, makeNormalize(relName));
            } else {
                name = normalize(name, relName);
            }
        } else {
            name = normalize(name, relName);
            parts = splitPrefix(name);
            prefix = parts[0];
            name = parts[1];
            if (prefix) {
                plugin = callDep(prefix);
            }
        }

        //Using ridiculous property names for space reasons
        return {
            f: prefix ? prefix + '!' + name : name, //fullName
            n: name,
            pr: prefix,
            p: plugin
        };
    };

    function makeConfig(name) {
        return function () {
            return (config && config.config && config.config[name]) || {};
        };
    }

    handlers = {
        require: function (name) {
            return makeRequire(name);
        },
        exports: function (name) {
            var e = defined[name];
            if (typeof e !== 'undefined') {
                return e;
            } else {
                return (defined[name] = {});
            }
        },
        module: function (name) {
            return {
                id: name,
                uri: '',
                exports: defined[name],
                config: makeConfig(name)
            };
        }
    };

    main = function (name, deps, callback, relName) {
        var cjsModule, depName, ret, map, i,
            args = [],
            callbackType = typeof callback,
            usingExports;

        //Use name if no relName
        relName = relName || name;

        //Call the callback to define the module, if necessary.
        if (callbackType === 'undefined' || callbackType === 'function') {
            //Pull out the defined dependencies and pass the ordered
            //values to the callback.
            //Default to [require, exports, module] if no deps
            deps = !deps.length && callback.length ? ['require', 'exports', 'module'] : deps;
            for (i = 0; i < deps.length; i += 1) {
                map = makeMap(deps[i], relName);
                depName = map.f;

                //Fast path CommonJS standard dependencies.
                if (depName === "require") {
                    args[i] = handlers.require(name);
                } else if (depName === "exports") {
                    //CommonJS module spec 1.1
                    args[i] = handlers.exports(name);
                    usingExports = true;
                } else if (depName === "module") {
                    //CommonJS module spec 1.1
                    cjsModule = args[i] = handlers.module(name);
                } else if (hasProp(defined, depName) ||
                           hasProp(waiting, depName) ||
                           hasProp(defining, depName)) {
                    args[i] = callDep(depName);
                } else if (map.p) {
                    map.p.load(map.n, makeRequire(relName, true), makeLoad(depName), {});
                    args[i] = defined[depName];
                } else {
                    throw new Error(name + ' missing ' + depName);
                }
            }

            ret = callback ? callback.apply(defined[name], args) : undefined;

            if (name) {
                //If setting exports via "module" is in play,
                //favor that over return value and exports. After that,
                //favor a non-undefined return value over exports use.
                if (cjsModule && cjsModule.exports !== undef &&
                        cjsModule.exports !== defined[name]) {
                    defined[name] = cjsModule.exports;
                } else if (ret !== undef || !usingExports) {
                    //Use the return value from the function.
                    defined[name] = ret;
                }
            }
        } else if (name) {
            //May just be an object definition for the module. Only
            //worry about defining if have a module name.
            defined[name] = callback;
        }
    };

    requirejs = require = req = function (deps, callback, relName, forceSync, alt) {
        if (typeof deps === "string") {
            if (handlers[deps]) {
                //callback in this case is really relName
                return handlers[deps](callback);
            }
            //Just return the module wanted. In this scenario, the
            //deps arg is the module name, and second arg (if passed)
            //is just the relName.
            //Normalize module name, if it contains . or ..
            return callDep(makeMap(deps, callback).f);
        } else if (!deps.splice) {
            //deps is a config object, not an array.
            config = deps;
            if (config.deps) {
                req(config.deps, config.callback);
            }
            if (!callback) {
                return;
            }

            if (callback.splice) {
                //callback is an array, which means it is a dependency list.
                //Adjust args if there are dependencies
                deps = callback;
                callback = relName;
                relName = null;
            } else {
                deps = undef;
            }
        }

        //Support require(['a'])
        callback = callback || function () {};

        //If relName is a function, it is an errback handler,
        //so remove it.
        if (typeof relName === 'function') {
            relName = forceSync;
            forceSync = alt;
        }

        //Simulate async callback;
        if (forceSync) {
            main(undef, deps, callback, relName);
        } else {
            //Using a non-zero value because of concern for what old browsers
            //do, and latest browsers "upgrade" to 4 if lower value is used:
            //http://www.whatwg.org/specs/web-apps/current-work/multipage/timers.html#dom-windowtimers-settimeout:
            //If want a value immediately, use require('id') instead -- something
            //that works in almond on the global level, but not guaranteed and
            //unlikely to work in other AMD implementations.
            setTimeout(function () {
                main(undef, deps, callback, relName);
            }, 4);
        }

        return req;
    };

    /**
     * Just drops the config on the floor, but returns req in case
     * the config return value is used.
     */
    req.config = function (cfg) {
        return req(cfg);
    };

    /**
     * Expose module registry for debugging and tooling
     */
    requirejs._defined = defined;

    define = function (name, deps, callback) {

        //This module may not have dependencies
        if (!deps.splice) {
            //deps is not an array, so probably means
            //an object literal or factory function for
            //the value. Adjust args.
            callback = deps;
            deps = [];
        }

        if (!hasProp(defined, name) && !hasProp(waiting, name)) {
            waiting[name] = [name, deps, callback];
        }
    };

    define.amd = {
        jQuery: true
    };
}());

;
/**
 * Private namespace for host-side code.
 * @type {*|{}}
 * @private
 * @deprecated use AMD instead of global namespaces. The only thing that should be on _AP is _AP.define and _AP.require.
 */
if(!window._AP){
    window._AP = {};
}
;
/**
 * The iframe-side code exposes a jquery-like implementation via _dollar.
 * This runs on the product side to provide AJS.$ under a _dollar module to provide a consistent interface
 * to code that runs on host and iframe.
 */
define("_dollar", [],function () {
  return AJS.$;
});

define("host/_util", [],function () {
    "use strict";

    return {
        escapeSelector: function( s ){
            if(!s){
                throw new Error("No selector to escape");
            }
            return s.replace(/[!"#$%&'()*+,.\/:;<=>?@[\\\]^`{|}~]/g, "\\$&");
        }
    };
});
( (typeof _AP !== "undefined") ? define : AP.define)("_events", ["_dollar"], function ($) {

  "use strict";

  var w = window,
      log = (w.AJS && w.AJS.log) || (w.console && w.console.log) || (function() {});

  /**
   * A simple pub/sub event bus capable of running on either side of the XDM bridge with no external
   * JS lib dependencies.
   *
   * @param {String} key The key of the event source
   * @param {String} origin The origin of the event source
   * @constructor
   */
  function Events(key, origin) {
    this._key = key;
    this._origin = origin;
    this._events = {};
    this._any = [];
  }

  var proto = Events.prototype;

  /**
   * Subscribes a callback to an event name.
   *
   * @param {String} name The event name to subscribe the listener to
   * @param {Function} listener A listener callback to subscribe to the event name
   * @returns {Events} This Events instance
   */
  proto.on = function (name, listener) {
    if (name && listener) {
      this._listeners(name).push(listener);
    }
    return this;
  };

  /**
   * Subscribes a callback to an event name, removing the it once fired.
   *
   * @param {String} name The event name to subscribe the listener to
   * @param {Function}listener A listener callback to subscribe to the event name
   * @returns {Events} This Events instance
   */
  proto.once = function (name, listener) {
    var self = this;
    var interceptor = function () {
      self.off(name, interceptor);
      listener.apply(null, arguments);
    };
    this.on(name, interceptor);
    return this;
  };

  /**
   * Subscribes a callback to all events, regardless of name.
   *
   * @param {Function} listener A listener callback to subscribe for any event name
   * @returns {Events} This Events instance
   */
  proto.onAny = function (listener) {
    this._any.push(listener);
    return this;
  };

  /**
   * Unsubscribes a callback to an event name.
   *
   * @param {String} name The event name to unsubscribe the listener from
   * @param {Function} listener The listener callback to unsubscribe from the event name
   * @returns {Events} This Events instance
   */
  proto.off = function (name, listener) {
    var all = this._events[name];
    if (all) {
      var i = $.inArray(listener, all);
      if (i >= 0) {
        all.splice(i, 1);
      }
      if (all.length === 0) {
        delete this._events[name];
      }
    }
    return this;
  };

  /**
   * Unsubscribes all callbacks from an event name, or unsubscribes all event-name-specific listeners
   * if no name if given.
   *
   * @param {String} [name] The event name to unsubscribe all listeners from
   * @returns {Events} This Events instance
   */
  proto.offAll = function (name) {
    if (name) {
      delete this._events[name];
    } else {
      this._events = {};
    }
    return this;
  };

  /**
   * Unsubscribes a callback from the set of 'any' event listeners.
   *
   * @param {Function} listener A listener callback to unsubscribe from any event name
   * @returns {Events} This Events instance
   */
  proto.offAny = function (listener) {
    var any = this._any;
    var i = $.inArray(listener, any);
    if (i >= 0) {
      any.splice(i, 1);
    }
    return this;
  };

  /**
   * Emits an event on this bus, firing listeners by name as well as all 'any' listeners. Arguments following the
   * name parameter are captured and passed to listeners.  The last argument received by all listeners after the
   * unpacked arguments array will be the fired event object itself, which can be useful for reacting to event
   * metadata (e.g. the bus's namespace).
   *
   * @param {String} name The name of event to emit
   * @param {Array.<String>} args 0 or more additional data arguments to deliver with the event
   * @returns {Events} This Events instance
   */
  proto.emit = function (name) {
    return this._emitEvent(this._event.apply(this, arguments));
  };

  /**
   * Creates an opaque event object from an argument list containing at least a name, and optionally additional
   * event payload arguments.
   *
   * @param {String} name The name of event to emit
   * @param {Array.<String>} args 0 or more additional data arguments to deliver with the event
   * @returns {Object} A new event object
   * @private
   */
  proto._event = function (name) {
    return {
      name: name,
      args: [].slice.call(arguments, 1),
      attrs: {},
      source: {
        key: this._key,
        origin: this._origin
      }
    };
  };

  /**
   * Emits a previously-constructed event object to all listeners.
   *
   * @param {Object} event The event object to emit
   * @param {String} event.name The name of the event
   * @param {Object} event.source Metadata about the original source of the event, containing key and origin
   * @param {Array} event.args The args passed to emit, to be delivered to listeners
   * @returns {Events} This Events instance
   * @private
   */
  proto._emitEvent = function (event) {
    var args = event.args.concat(event);
    fire(this._listeners(event.name), args);
    fire(this._any, [event.name].concat(args));
    return this;
  };

  /**
   * Returns an array of listeners by event name, creating a new name array if none are found.
   *
   * @param {String} name The event name for which listeners should be returned
   * @returns {Array} An array of listeners; empty if none are registered
   * @private
   */
  proto._listeners = function (name) {
    return this._events[name] = this._events[name] || [];
  };

  // Internal helper for firing an event to an array of listeners
  function fire(listeners, args) {
    for (var i = 0; i < listeners.length; ++i) {
      try {
        listeners[i].apply(null, args);
      } catch (e) {
        log(e.stack || e.message || e);
      }
    }
  }

  return {
    Events: Events
  };

});

/*
 Copyright (c) 2008 Fred Palmer fred.palmer_at_gmail.com

 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:

 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.

 Modified slightly to wrap in our define() and function($) logic.
 */

( (typeof _AP !== "undefined") ? define : AP.define)("_base64", ["_dollar"], function ($) {

    "use strict";


    function StringBuffer()
    {
        this.buffer = [];
    }

    StringBuffer.prototype.append = function append(string)
    {
        this.buffer.push(string);
        return this;
    };

    StringBuffer.prototype.toString = function toString()
    {
        return this.buffer.join("");
    };

    var Base64 =
    {
        codex : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

        encode : function (input)
        {
            var output = new StringBuffer();

            var enumerator = new Utf8EncodeEnumerator(input);
            while (enumerator.moveNext())
            {
                var chr1 = enumerator.current;

                enumerator.moveNext();
                var chr2 = enumerator.current;

                enumerator.moveNext();
                var chr3 = enumerator.current;

                var enc1 = chr1 >> 2;
                var enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                var enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                var enc4 = chr3 & 63;

                if (isNaN(chr2))
                {
                    enc3 = enc4 = 64;
                }
                else if (isNaN(chr3))
                {
                    enc4 = 64;
                }

                output.append(this.codex.charAt(enc1) + this.codex.charAt(enc2) + this.codex.charAt(enc3) + this.codex.charAt(enc4));
            }

            return output.toString();
        },

        decode : function (input)
        {
            var output = new StringBuffer();

            var enumerator = new Base64DecodeEnumerator(input);
            while (enumerator.moveNext())
            {
                var charCode = enumerator.current;

                if (charCode < 128)
                    output.append(String.fromCharCode(charCode));
                else if ((charCode > 191) && (charCode < 224))
                {
                    enumerator.moveNext();
                    var charCode2 = enumerator.current;

                    output.append(String.fromCharCode(((charCode & 31) << 6) | (charCode2 & 63)));
                }
                else
                {
                    enumerator.moveNext();
                    var charCode2 = enumerator.current;

                    enumerator.moveNext();
                    var charCode3 = enumerator.current;

                    output.append(String.fromCharCode(((charCode & 15) << 12) | ((charCode2 & 63) << 6) | (charCode3 & 63)));
                }
            }

            return output.toString();
        }
    }


    function Utf8EncodeEnumerator(input)
    {
        this._input = input;
        this._index = -1;
        this._buffer = [];
    }

    Utf8EncodeEnumerator.prototype =
    {
        current: Number.NaN,

        moveNext: function()
        {
            if (this._buffer.length > 0)
            {
                this.current = this._buffer.shift();
                return true;
            }
            else if (this._index >= (this._input.length - 1))
            {
                this.current = Number.NaN;
                return false;
            }
            else
            {
                var charCode = this._input.charCodeAt(++this._index);

                // "\r\n" -> "\n"
                //
                if ((charCode == 13) && (this._input.charCodeAt(this._index + 1) == 10))
                {
                    charCode = 10;
                    this._index += 2;
                }

                if (charCode < 128)
                {
                    this.current = charCode;
                }
                else if ((charCode > 127) && (charCode < 2048))
                {
                    this.current = (charCode >> 6) | 192;
                    this._buffer.push((charCode & 63) | 128);
                }
                else
                {
                    this.current = (charCode >> 12) | 224;
                    this._buffer.push(((charCode >> 6) & 63) | 128);
                    this._buffer.push((charCode & 63) | 128);
                }

                return true;
            }
        }
    }

    function Base64DecodeEnumerator(input)
    {
        this._input = input;
        this._index = -1;
        this._buffer = [];
    }

    Base64DecodeEnumerator.prototype =
    {
        current: 64,

        moveNext: function()
        {
            if (this._buffer.length > 0)
            {
                this.current = this._buffer.shift();
                return true;
            }
            else if (this._index >= (this._input.length - 1))
            {
                this.current = 64;
                return false;
            }
            else
            {
                var enc1 = Base64.codex.indexOf(this._input.charAt(++this._index));
                var enc2 = Base64.codex.indexOf(this._input.charAt(++this._index));
                var enc3 = Base64.codex.indexOf(this._input.charAt(++this._index));
                var enc4 = Base64.codex.indexOf(this._input.charAt(++this._index));

                var chr1 = (enc1 << 2) | (enc2 >> 4);
                var chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                var chr3 = ((enc3 & 3) << 6) | enc4;

                this.current = chr1;

                if (enc3 != 64 && chr2 != 0)
                    this._buffer.push(chr2);

                if (enc4 != 64 && chr3 != 0)
                    this._buffer.push(chr3);

                return true;
            }
        }
    };

    function encode(plainText) {
        return Base64.encode(plainText);
    }

    function decode(encodedText) {
        return Base64.decode(encodedText);
    }

    return {
        encode: encode,
        decode: decode
    };

});

( (typeof _AP !== "undefined") ? define : AP.define)("_jwt", ["_base64"], function(base64){
    "use strict";

    function parseJwtIssuer(jwt) {
        return parseJwtClaims(jwt)['iss'];
    }

    function parseJwtClaims(jwt) {

        if (null === jwt || '' === jwt) {
            throw('Invalid JWT: must be neither null nor empty-string.');
        }

        var firstPeriodIndex = jwt.indexOf('.');
        var secondPeriodIndex = jwt.indexOf('.', firstPeriodIndex + 1);

        if (firstPeriodIndex < 0 || secondPeriodIndex <= firstPeriodIndex) {
            throw('Invalid JWT: must contain 2 period (".") characters.');
        }

        var encodedClaims = jwt.substring(firstPeriodIndex + 1, secondPeriodIndex);

        if (null === encodedClaims || '' === encodedClaims) {
            throw('Invalid JWT: encoded claims must be neither null nor empty-string.');
        }

        var claimsString = base64.decode(encodedClaims);
        return JSON.parse(claimsString);
    }

    function isJwtExpired(jwtString, skew){
        if(skew === undefined){
            skew = 60; // give a minute of leeway to allow clock skew
        }
        var claims = parseJwtClaims(jwtString),
        expires = 0,
        now = Math.floor(Date.now() / 1000); // UTC timestamp now

        if(claims && claims.exp){
            expires = claims.exp;
        }

        if( (expires - now) < skew){
            return true;
        }

        return false;

    }

    return {
        parseJwtIssuer: parseJwtIssuer,
        parseJwtClaims: parseJwtClaims,
        isJwtExpired: isJwtExpired
    };
});
/*!
 * jsUri
 * https://github.com/derek-watson/jsUri
 *
 * Copyright 2013, Derek Watson
 * Released under the MIT license.
 *
 * Includes parseUri regular expressions
 * http://blog.stevenlevithan.com/archives/parseuri
 * Copyright 2007, Steven Levithan
 * Released under the MIT license.
 */

 /*globals define, module */
( (typeof _AP !== "undefined") ? define : AP.define)("_uri", [], function () {

  var re = {
    starts_with_slashes: /^\/+/,
    ends_with_slashes: /\/+$/,
    pluses: /\+/g,
    query_separator: /[&;]/,
    uri_parser: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*)(?::([^:@]*))?)?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/
  };

  /**
   * Define forEach for older js environments
   * @see https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/forEach#Compatibility
   */
  if (!Array.prototype.forEach) {
    Array.prototype.forEach = function(fn, scope) {
      for (var i = 0, len = this.length; i < len; ++i) {
        fn.call(scope || this, this[i], i, this);
      }
    };
  }

  /**
   * unescape a query param value
   * @param  {string} s encoded value
   * @return {string}   decoded value
   */
  function decode(s) {
    if (s) {
      s = decodeURIComponent(s);
      s = s.replace(re.pluses, ' ');
    }
    return s;
  }

  /**
   * Breaks a uri string down into its individual parts
   * @param  {string} str uri
   * @return {object}     parts
   */
  function parseUri(str) {
    var parser = re.uri_parser;
    var parserKeys = ["source", "protocol", "authority", "userInfo", "user", "password", "host", "port", "relative", "path", "directory", "file", "query", "anchor"];
    var m = parser.exec(str || '');
    var parts = {};

    parserKeys.forEach(function(key, i) {
      parts[key] = m[i] || '';
    });

    return parts;
  }

  /**
   * Breaks a query string down into an array of key/value pairs
   * @param  {string} str query
   * @return {array}      array of arrays (key/value pairs)
   */
  function parseQuery(str) {
    var i, ps, p, n, k, v;
    var pairs = [];

    if (typeof(str) === 'undefined' || str === null || str === '') {
      return pairs;
    }

    if (str.indexOf('?') === 0) {
      str = str.substring(1);
    }

    ps = str.toString().split(re.query_separator);

    for (i = 0; i < ps.length; i++) {
      p = ps[i];
      n = p.indexOf('=');

      if (n !== 0) {
        k = decodeURIComponent(p.substring(0, n));
        v = decodeURIComponent(p.substring(n + 1).replace(/\+/g, " "));
        pairs.push(n === -1 ? [p, null] : [k, v]);
      }

    }
    return pairs;
  }

  /**
   * Creates a new Uri object
   * @constructor
   * @param {string} str
   */
  function Uri(str) {
    this.uriParts = parseUri(str);
    this.queryPairs = parseQuery(this.uriParts.query);
    this.hasAuthorityPrefixUserPref = null;
  }

  /**
   * Define getter/setter methods
   */
  ['protocol', 'userInfo', 'host', 'port', 'path', 'anchor'].forEach(function(key) {
    Uri.prototype[key] = function(val) {
      if (typeof val !== 'undefined') {
        this.uriParts[key] = val;
      }
      return this.uriParts[key];
    };
  });

  /**
   * if there is no protocol, the leading // can be enabled or disabled
   * @param  {Boolean}  val
   * @return {Boolean}
   */
  Uri.prototype.hasAuthorityPrefix = function(val) {
    if (typeof val !== 'undefined') {
      this.hasAuthorityPrefixUserPref = val;
    }

    if (this.hasAuthorityPrefixUserPref === null) {
      return (this.uriParts.source.indexOf('//') !== -1);
    } else {
      return this.hasAuthorityPrefixUserPref;
    }
  };

  /**
   * Serializes the internal state of the query pairs
   * @param  {string} [val]   set a new query string
   * @return {string}         query string
   */
  Uri.prototype.query = function(val) {
    var s = '', i, param;

    if (typeof val !== 'undefined') {
      this.queryPairs = parseQuery(val);
    }

    for (i = 0; i < this.queryPairs.length; i++) {
      param = this.queryPairs[i];
      if (s.length > 0) {
        s += '&';
      }
      if (param[1] === null) {
        s += param[0];
      } else {
        s += param[0];
        s += '=';
        if (param[1]) {
          s += encodeURIComponent(param[1]);
        }
      }
    }
    return s.length > 0 ? '?' + s : s;
  };

  /**
   * returns the first query param value found for the key
   * @param  {string} key query key
   * @return {string}     first value found for key
   */
  Uri.prototype.getQueryParamValue = function (key) {
    var param, i;
    for (i = 0; i < this.queryPairs.length; i++) {
      param = this.queryPairs[i];
      if (key === param[0]) {
        return param[1];
      }
    }
  };

  /**
   * returns an array of query param values for the key
   * @param  {string} key query key
   * @return {array}      array of values
   */
  Uri.prototype.getQueryParamValues = function (key) {
    var arr = [], i, param;
    for (i = 0; i < this.queryPairs.length; i++) {
      param = this.queryPairs[i];
      if (key === param[0]) {
        arr.push(param[1]);
      }
    }
    return arr;
  };

  /**
   * removes query parameters
   * @param  {string} key     remove values for key
   * @param  {val}    [val]   remove a specific value, otherwise removes all
   * @return {Uri}            returns self for fluent chaining
   */
  Uri.prototype.deleteQueryParam = function (key, val) {
    var arr = [], i, param, keyMatchesFilter, valMatchesFilter;

    for (i = 0; i < this.queryPairs.length; i++) {

      param = this.queryPairs[i];
      keyMatchesFilter = decode(param[0]) === decode(key);
      valMatchesFilter = param[1] === val;

      if ((arguments.length === 1 && !keyMatchesFilter) || (arguments.length === 2 && (!keyMatchesFilter || !valMatchesFilter))) {
        arr.push(param);
      }
    }

    this.queryPairs = arr;

    return this;
  };

  /**
   * adds a query parameter
   * @param  {string}  key        add values for key
   * @param  {string}  val        value to add
   * @param  {integer} [index]    specific index to add the value at
   * @return {Uri}                returns self for fluent chaining
   */
  Uri.prototype.addQueryParam = function (key, val, index) {
    if (arguments.length === 3 && index !== -1) {
      index = Math.min(index, this.queryPairs.length);
      this.queryPairs.splice(index, 0, [key, val]);
    } else if (arguments.length > 0) {
      this.queryPairs.push([key, val]);
    }
    return this;
  };

  /**
   * replaces query param values
   * @param  {string} key         key to replace value for
   * @param  {string} newVal      new value
   * @param  {string} [oldVal]    replace only one specific value (otherwise replaces all)
   * @return {Uri}                returns self for fluent chaining
   */
  Uri.prototype.replaceQueryParam = function (key, newVal, oldVal) {
    var index = -1, i, param;

    if (arguments.length === 3) {
      for (i = 0; i < this.queryPairs.length; i++) {
        param = this.queryPairs[i];
        if (decode(param[0]) === decode(key) && decodeURIComponent(param[1]) === decode(oldVal)) {
          index = i;
          break;
        }
      }
      this.deleteQueryParam(key, oldVal).addQueryParam(key, newVal, index);
    } else {
      for (i = 0; i < this.queryPairs.length; i++) {
        param = this.queryPairs[i];
        if (decode(param[0]) === decode(key)) {
          index = i;
          break;
        }
      }
      this.deleteQueryParam(key);
      this.addQueryParam(key, newVal, index);
    }
    return this;
  };

  /**
   * Define fluent setter methods (setProtocol, setHasAuthorityPrefix, etc)
   */
  ['protocol', 'hasAuthorityPrefix', 'userInfo', 'host', 'port', 'path', 'query', 'anchor'].forEach(function(key) {
    var method = 'set' + key.charAt(0).toUpperCase() + key.slice(1);
    Uri.prototype[method] = function(val) {
      this[key](val);
      return this;
    };
  });

  /**
   * Scheme name, colon and doubleslash, as required
   * @return {string} http:// or possibly just //
   */
  Uri.prototype.scheme = function() {
    var s = '';

    if (this.protocol()) {
      s += this.protocol();
      if (this.protocol().indexOf(':') !== this.protocol().length - 1) {
        s += ':';
      }
      s += '//';
    } else {
      if (this.hasAuthorityPrefix() && this.host()) {
        s += '//';
      }
    }

    return s;
  };

  /**
   * Same as Mozilla nsIURI.prePath
   * @return {string} scheme://user:password@host:port
   * @see  https://developer.mozilla.org/en/nsIURI
   */
  Uri.prototype.origin = function() {
    var s = this.scheme();

    if (s == 'file://') {
      return s + this.uriParts.authority;
    }

    if (this.userInfo() && this.host()) {
      s += this.userInfo();
      if (this.userInfo().indexOf('@') !== this.userInfo().length - 1) {
        s += '@';
      }
    }

    if (this.host()) {
      s += this.host();
      if (this.port()) {
        s += ':' + this.port();
      }
    }

    return s;
  };

  /**
   * Adds a trailing slash to the path
   */
  Uri.prototype.addTrailingSlash = function() {
    var path = this.path() || '';

    if (path.substr(-1) !== '/') {
      this.path(path + '/');
    }

    return this;
  };

  /**
   * Serializes the internal state of the Uri object
   * @return {string}
   */
  Uri.prototype.toString = function() {
    var path, s = this.origin();

    if (this.path()) {
      path = this.path();
      if (!(re.ends_with_slashes.test(s) || re.starts_with_slashes.test(path))) {
        s += '/';
      } else {
        if (s) {
          s.replace(re.ends_with_slashes, '/');
        }
        path = path.replace(re.starts_with_slashes, '/');
      }
      s += path;
    } else {
      if (this.host() && (this.query().toString() || this.anchor())) {
        s += '/';
      }
    }
    if (this.query().toString()) {
      if (this.query().toString().indexOf('?') !== 0) {
        s += '?';
      }
      s += this.query().toString();
    }

    if (this.anchor()) {
      if (this.anchor().indexOf('#') !== 0) {
        s += '#';
      }
      s += this.anchor();
    }

    return s;
  };

  /**
   * Clone a Uri object
   * @return {Uri} duplicate copy of the Uri
   */
  Uri.prototype.clone = function() {
    return new Uri(this.toString());
  };

  return {
    init: Uri
  };

});
( (typeof _AP !== "undefined") ? define : AP.define)("_ui-params", ["_dollar", "_base64", "_uri"], function($, base64, Uri) {

    /**
    * These are passed into the main host create statement and can override
    * any options inside the velocity template.
    * Additionally these are accessed by the js inside the client iframe to check if we are in a dialog.
    */

    return {
        /**
        * Encode options for transport
        */
        encode: function(options){
            if(options){
                return base64.encode(JSON.stringify(options));
            }
        },
        /**
        * return ui params from a Url
        **/
        fromUrl: function(url){
            var url = new Uri.init(url),
            params = url.getQueryParamValue('ui-params');
            return this.decode(params);
        },
        /**
        * returns ui params from window.name
        */
        fromWindowName: function(w, param){
            w = w || window;
            var decoded = this.decode(w.name);

            if(!param){
                return decoded;
            }
            return (decoded) ? decoded[param] : undefined;
        },
        /**
        * Decode a base64 encoded json string containing ui params
        */
        decode: function(params){
            var obj = {};
            if(params && params.length > 0){
                try {
                    obj = JSON.parse(base64.decode(params));
                } catch(e) {
                    if(console && console.log){
                        console.log("Cannot decode passed ui params", params);
                    }
                }
            }
            return obj;
        }
    };

});

var deps = ["_events", "_jwt", "_uri",  "_ui-params", "host/_util"];
if(this.AP){
  deps = ["_events", "_jwt", "_uri",  "_ui-params"];
}
( (typeof _AP !== "undefined") ? define : AP.define)("_xdm", deps, function (events, jwt, uri, uiParams, util) {

  "use strict";

  // Capture some common values and symbol aliases
  var count = 0;

  /**
   * Sets up cross-iframe remote procedure calls.
   * If this is called from a parent window, iframe is created and an RPC interface for communicating with it is set up.
   * If this is called from within the iframe, an RPC interface for communicating with the parent is set up.
   *
   * Calling a remote function is done with the signature:
   *     fn(data..., doneCallback, failCallback)
   * doneCallback is called after the remote function executed successfully.
   * failCallback is called after the remote function throws an exception.
   * doneCallback and failCallback are optional.
   *
   * @param {Object} $ jquery or jquery-like utility
   * @param {Object} config Configuration parameters
   * @param {String} config.remoteKey The remote peer's add-on key (host only)
   * @param {String} config.remote The src of remote iframe (host only)
   * @param {String} config.container The id of element to which the generated iframe is appended (host only)
   * @param {Object} config.props Additional attributes to add to iframe element (host only)
   * @param {String} config.channel Channel (host only); deprecated
   * @param {Object} bindings RPC method stubs and implementations
   * @param {Object} bindings.local Local function implementations - functions that exist in the current context.
   *    XdmRpc exposes these functions so that they can be invoked by code running in the other side of the iframe.
   * @param {Array} bindings.remote Names of functions which exist on the other side of the iframe.
   *    XdmRpc creates stubs to these functions that can be invoked from the current page.
   * @returns XdmRpc instance
   * @constructor
   */
  function XdmRpc($, config, bindings) {

    var self, id, target, remoteOrigin, channel, mixin,
        localKey, remoteKey, addonKey,
        w = window,
        loc = w.location.toString(),
        locals = bindings.local || {},
        remotes = bindings.remote || [],
        localOrigin = getBaseUrl(loc);

    // A hub through which all async callbacks for remote requests are parked until invoked from a response
    var nexus = function () {
      var callbacks = {};
      return {
        // Registers a callback of a given type by uid
        add: function (uid, done, fail) {
          callbacks[uid] = {
            done: done || null,
            fail: fail || null,
            async: !!done
          };
        },
        // Invokes callbacks for a response of a given type by uid if registered, then removes all handlers for the uid
        invoke: function (type, uid, arg) {
          var handled;
          if (callbacks[uid]) {
            if (callbacks[uid][type]) {
              // If the intended callback exists, invoke it and mark the response as handled
              callbacks[uid][type](arg);
              handled = true;
            } else {
              // Only mark other calls as handled if they weren't expecting a callback and didn't fail
              handled = !callbacks[uid].async && type !== "fail";
            }
            delete callbacks[uid];
          }
          return handled;
        }
      };
    }();

    // Use the config and enviroment to construct the core of the new XdmRpc instance.
    //
    // Note: The xdm_e|c|p variables that appear in an iframe URL are used to pass message to the XdmRpc bridge
    // when running inside an add-on iframe.  Their names are holdovers from easyXDM, which was used prior
    // to building this proprietary library (which was done both to greatly reduce the total amount of JS
    // needed to drive the postMessage-based RPC communication, and to allow us to extend its capabilities).
    //
    // AC-451 describes how we can reduce/improve these (and other) iframe url parameters, but until that is
    // addressed, here's a brief description of each:
    //
    //  - xdm_e contains the base url of the host app; it's presence indicates that the XdmRpc is running in
    //    an add-on iframe
    //  - xdm_c contains a unique channel name; this is a holdover from easyXDM that was used to distinguish
    //    postMessage events between multiple iframes with identical xdm_e values, though this may now be
    //    redundant with the current internal implementation of the XdmRpc and should be considered for removal
    if (!/xdm_e/.test(loc)) {
      // Host-side constructor branch

      // if there is already an iframe created. Destroy it. It's an old version.
      $("#" + util.escapeSelector(config.container)).find('iframe').trigger('ra.iframe.destroy');

      var iframe = createIframe(config);
      target = iframe.contentWindow;
      localKey = param(config.remote, "oauth_consumer_key") || param(config.remote, "jwt");
      remoteKey = config.remoteKey;
      addonKey = remoteKey;
      remoteOrigin = getBaseUrl(config.remote).toLowerCase();
      channel = config.channel;
      // Define the host-side mixin
      mixin = {
        isHost: true,
        iframe: iframe,
        uiParams: config.uiParams,
        destroy: function () {
          window.clearTimeout(self.timeout); //clear the iframe load time.
          // Unbind postMessage handler when destroyed
          unbind();
          // Then remove the iframe, if it still exists
          if (self.iframe) {
            $(self.iframe).remove();
            delete self.iframe;
          }
        },
        isActive: function () {
          // Host-side instances are only active as long as the iframe they communicate with still exists in the DOM
          return $.contains(document.documentElement, self.iframe);
        }
      };
      $(iframe).on('ra.iframe.destroy', mixin.destroy);
    } else {
      // Add-on-side constructor branch
      target = w.parent;
      localKey = "local"; // Would be better to make this the add-on key, but it's not readily available at this time

      // identify the add-on by unique key: first try JWT issuer claim and fall back to OAuth1 consumer key
      var jwtParam = param(loc, "jwt");
      remoteKey = jwtParam ? jwt.parseJwtIssuer(jwtParam) : param(loc, "oauth_consumer_key");

      // if the authentication method is "none" then it is valid to have no jwt and no oauth in the url
      // but equally we don't trust this iframe as far as we can throw it, so assign it a random id
      // in order to prevent it from talking to any other iframe
      if (null === remoteKey) {
          remoteKey = Math.random(); // unpredictable and unsecured, like an oauth consumer key
      }

      addonKey = localKey;
      remoteOrigin = param(loc, "xdm_e").toLowerCase();
      channel = param(loc, "xdm_c");
      // Define the add-on-side mixin
      mixin = {
        isHost: false,
        isActive: function () {
          // Add-on-side instances are always active, as they must always have a parent window peer
          return true;
        }
      };
    }

    id = addonKey + "|" + (count += 1);

    // Create the actual XdmRpc instance, and apply the context-sensitive mixin
    self = $.extend({
      id: id,
      remoteOrigin: remoteOrigin,
      channel: channel,
      addonKey: addonKey
    }, mixin);

    // Sends a message of a specific type to the remote peer via a post-message event
    function send(sid, type, message) {
      try {
        target.postMessage(JSON.stringify({
          c: channel,
          i: sid,
          t: type,
          m: message
        }), remoteOrigin);
      } catch (ex) {
        log(errmsg(ex));
      }
    }

    // Sends a request with a specific remote method name, args, and optional callbacks
    function sendRequest(methodName, args, done, fail) {
      // Generate a random ID for this remote invocation
      var sid = Math.floor(Math.random() * 1000000000).toString(16);
      // Register any callbacks with the nexus so they can be invoked when a response is received
      nexus.add(sid, done, fail);
      // Send a request to the remote, where:
      //  - n is the name of the remote function
      //  - a is an array of the (hopefully) serializable, non-callback arguments to this method
      send(sid, "request", {n: methodName, a: args});
    }

    function sendDone(sid, message) {
      send(sid, "done", message);
    }

    function sendFail(sid, message) {
      send(sid, "fail", message);
    }

    // Handles an normalized, incoming post-message event
    function receive(e) {
      try {
        // Extract message payload from the event
        var payload = JSON.parse(e.data),
            pid = payload.i, pchannel = payload.c, ptype = payload.t, pmessage = payload.m;

        // if the iframe has potentially been reloaded. re-attach the source contentWindow object
        if (e.source !== target && e.origin.toLowerCase() === remoteOrigin && pchannel === channel) {
          target = e.source;
        }

        // If the payload doesn't match our expected event signature, assume its not part of the xdm-rpc protocol
        if (e.source !== target || e.origin.toLowerCase() !== remoteOrigin || pchannel !== channel){
          return;
        }

        if (ptype === "request") {
          // If the payload type is request, this is an incoming method invocation
          var name = pmessage.n, args = pmessage.a,
              local = locals[name], done, fail, async;
          if (local) {
            // The message name matches a locally defined RPC method, so inspect and invoke it according
            // Create responders for each response type
            done = function (message) { sendDone(pid, message); };
            fail = function (message) { sendFail(pid, message); };
            // The local method is considered async if it accepts more arguments than the message has sent;
            // the additional arguments are filled in with the above async responder callbacks;
            // TODO: consider specifying args somehow in the remote stubs so that non-callback args can be
            //       verified before sending a request to fail fast at the callsite
            async = (args ? args.length : 0) < local.length;
            var context = locals;
            if(self.isHost === true){
                context = self;
                if(context.analytics){
                  context.analytics.trackBridgeMethod(name);
                }
            } else {
              context.isHost = false;
            }
            try {
              if (async) {
                // If async, apply the method with the responders added to the args list
                local.apply(context, args.concat([done, fail]));
              } else {
                // Otherwise, immediately respond with the result
                done(local.apply(context, args));
              }
            } catch (ex) {
              // If the invocation threw an error, invoke the fail responder callback with it
              fail(errmsg(ex));
              logError(ex);
            }
          } else {
            // No such local rpc method name found
            debug("Unhandled request:", payload);
          }
        } else if (ptype === "done" || ptype === "fail") {
          // The payload is of a response type, so try to invoke the appropriate callback via the nexus registry
          if (!nexus.invoke(ptype, pid, pmessage)) {
            // The nexus didn't find an appropriate reponse callback to invoke
            debug("Unhandled response:", ptype, pid, pmessage);
          }
        }
      } catch (ex) {
        log(errmsg(ex));
      }
    }

    // Creates a bridging invocation function for a remote method
    function bridge(methodName) {
      // Add a method to this instance that will convert from 'rpc.method(args..., done?, fail?)'-style
      // invocations to a postMessage event via the 'send' function
      return function () {
        var args = [].slice.call(arguments), done, fail;
        // Pops the last arg off the args list if it's a function
        function popFn() {
          if ($.isFunction(args[args.length - 1])) {
            return args.pop();
          }
        }
        // Remove done/fail callbacks from the args list
        fail = popFn();
        done = popFn();
        if (!done) {
          // Set the done cb to the value of the fail cb if only one callback fn was given
          done = fail;
          fail = undefined;
        }
        sendRequest(methodName, args, done, fail);
      };
    }

    // For each remote method, generate a like-named interceptor on this instance that converts invocations to
    // post-message request events, tracking async callbacks as necessary.
    $.each(remotes, function (methodName, v) {
      // If remotes were specified as an array rather than a map, promote v to methodName
      if (typeof methodName === "number") methodName = v;
      self[methodName] = bridge(methodName);
    });

    // Create and attach a local event emitter for bridged pub/sub
    var bus = self.events = new events.Events(localKey, localOrigin);
    // Attach an any-listener to forward all locally-originating events to the remote peer
    bus.onAny(function () {
      // The actual event object is the last argument passed to any listener
      var event = arguments[arguments.length - 1];
      var trace = event.trace = event.trace || {};
      var traceKey = self.id + "|xdm";
      if ((self.isHost && !trace[traceKey] && event.source.channel !== self.id)
          || (!self.isHost && event.source.key === localKey)) {
        // Only forward an event once in this listener
        trace[traceKey] = true;
        // Clone the event and forward without tracing info, to avoid leaking host-side iframe topology to add-ons
        event = $.extend({}, event);
        delete event.trace;
        debug("Forwarding " + (self.isHost ? "host" : "addon") + " event:", event);
        sendRequest("_event", [event]);
      }
    });
    // Define our own reserved local to receive remote events
    locals._event = function (event) {
      // Reset/ignore any tracing info that may have come across the bridge
      delete event.trace;
      if (this.isHost) {
        // When the running on the host-side, forcibly reset the event's key and origin fields, to prevent spoofing by
        // untrusted add-ons; also include the host-side XdmRpc instance id to tag the event with this particular
        // instance of the host/add-on relationship
        event.source = {
          channel: this.id || id, // Note: the term channel here != the deprecated xdm channel param
          key: this.addonKey,
          origin: this.remoteOrigin || remoteOrigin
        };
      }
      debug("Receiving as " + (this.isHost ? "host" : "addon") + " event:", event);
      // Emit the event on the local bus
      bus._emitEvent(event);
    };

    // Handles incoming postMessages from this XdmRpc instance's remote peer
    function postMessageHandler(e) {
      if (self.isActive()) {
        // Normalize and forward the event message to the receiver logic
        receive(e.originalEvent ? e.originalEvent : e);
      } else {
        // If inactive (due to the iframe element having disappeared from the DOM), force cleanup of this callback
        unbind();
      }
    }

    // Starts listening for window messaging events
    function bind() {
      $(window).bind("message", postMessageHandler);
    }

    // Stops listening for window messaging events
    function unbind() {
      $(window).unbind("message", postMessageHandler);
    }

    // Crudely extracts a query param value from a url by name
    function param(url, name) {
      return new uri.init(url).getQueryParamValue(name);
    }

    // Determines a base url consisting of protocol+domain+port from a given url string
    function getBaseUrl(url) {
      return new uri.init(url).origin();
    }

    // Appends a map of query parameters to a base url
    function toUrl(base, params) {
      var url = new uri.init(base);
      $.each(params, function (k, v) {
        url.addQueryParam(k,v);
      });
      return url.toString();
    }

    // Creates an iframe element from a config option consisting of the following values:
    //  - container:  the parent element of the new iframe
    //  - remote:     the src url of the new iframe
    //  - props:      a map of additional HTML attributes for the new iframe
    //  - channel:    deprecated
    function createIframe(config) {
      if(!config.container){
        throw new Error("config.container must be defined");
      }
      var iframe = document.createElement("iframe"),
        id = "easyXDM_" + config.container + "_provider",
        windowName = "";

      if(config.uiParams){
        windowName = uiParams.encode(config.uiParams);
      }
      $.extend(iframe, {id: id, name: windowName, frameBorder: "0"}, config.props);
      //$.extend will not add the attribute rel.
      iframe.setAttribute('rel', 'nofollow');
      $("#" + util.escapeSelector(config.container)).append(iframe);
      $(iframe).trigger("ra.iframe.create");
      iframe.src = config.remote;
      return iframe;
    }

    function errmsg(ex) {
      return ex.message || ex.toString();
    }

    function debug() {
      if (XdmRpc.debug) log.apply(w, ["DEBUG:"].concat([].slice.call(arguments)));
    }

    function log() {
      var log = $.log || (w.AJS && w.AJS.log);
      if (log) log.apply(w, arguments);
    }

    function logError() {
      // $.error seems to do the same thing as $.log in client console
      var error = (w.AJS && w.AJS.error);
      if (error) error.apply(w, arguments);
    }

    // Immediately start listening for events
    bind();

    return self;
  }

//  XdmRpc.debug = true;

  return XdmRpc;

});

define("host/jwt-keepalive", ["_dollar", "_jwt"], function($, jwt){
    "use strict";

    function updateUrl (config){
        var promise = $.Deferred(function(defer){
            var contentPromise = window._AP.contentResolver.resolveByParameters({
                addonKey: config.addonKey,
                moduleKey: config.moduleKey,
                productContext: config.productContext,
                uiParams: config.uiParams,
                width: config.width,
                height: config.height,
                classifier: 'json'
            });

            contentPromise.done(function(data){
                var values = JSON.parse(data);
                defer.resolve(values.src);
            });
        });

        return promise;
    }

    return {
        updateUrl: updateUrl,
        isExpired: jwt.isJwtExpired
    };

});
define("_rpc", ["_dollar", "_xdm", "host/jwt-keepalive", "_uri"], function ($, XdmRpc, jwtKeepAlive, uri) {

    "use strict";

    var each = $.each,
        extend = $.extend,
        isFn = $.isFunction,
        rpcCollection = [],
        apis = {},
        stubs = [],
        internals = {},
        inits = [];

    return {

        extend: function (config) {
            if (isFn(config)) config = config();
            extend(apis, config.apis);
            extend(internals, config.internals);
            stubs = stubs.concat(config.stubs || []);

            var init = config.init;
            if (isFn(init)) inits.push(init);
            return config.apis;
        },

        // init connect host side
        // options = things that go to all init functions

        init: function (options, xdmConfig) {

            var remoteUrl = new uri.init(xdmConfig.remote),
            remoteJwt = remoteUrl.getQueryParamValue('jwt'),
            promise;

            options = options || {};
            // add stubs for each public api
            each(apis, function (method) { stubs.push(method); });

            // refresh JWT tokens as required.
            if(remoteJwt && jwtKeepAlive.isExpired(remoteJwt)){
                promise = jwtKeepAlive.updateUrl({
                    addonKey: xdmConfig.remoteKey,
                    moduleKey: options.ns,
                    productContext: options.productContext || {},
                    uiParams: xdmConfig.uiParams,
                    width: xdmConfig.props.width,
                    height: xdmConfig.props.height
                });
            }

            $.when(promise).always(function(src){
                // if the promise resolves to a new url. update it.
                if(src){
                    xdmConfig.remote = src;
                }
                // TODO: stop copying internals and fix references instead (fix for events going across add-ons when they shouldn't)
                var rpc = new XdmRpc($, xdmConfig, {remote: stubs, local: $.extend({}, internals)});

                rpcCollection[rpc.id] = rpc;
                each(inits, function (_, init) {
                    try { init(extend({}, options), rpc); }
                    catch (ex) { console.log(ex); }
                });
            });

        }

    };

});

define("resize", ["_dollar", "_rpc"], function ($, rpc) {
    "use strict";
    rpc.extend(function () {
        return {
            init: function (config, xdm) {
                xdm.resize = AJS.debounce(function resize ($, width, height) {
                    $(this.iframe).css({width: width, height: height});
                    var nexus = $(this.iframe).closest('.ap-container');
                    nexus.trigger('resized', {width: width, height: height});

                });
            },
            internals: {
                resize: function(width, height) {
                    if(!this.uiParams.isDialog){
                        this.resize($, width, height);
                    }
                },
                sizeToParent: AJS.debounce(function() {

                    var resizeHandler = function (iframe) {
                        var height = $(document).height() - $("#header > nav").outerHeight() - $("#footer").outerHeight() - 20;
                        $(iframe).css({width: "100%", height: height + "px"});
                    };
                    // sizeToParent is only available for general-pages
                    if (this.uiParams.isGeneral) {
                        // This adds border between the iframe and the page footer as the connect addon has scrolling content and can't do this
                        $(this.iframe).addClass("full-size-general-page");
                        $(window).on('resize', function(){
                            resizeHandler(this.iframe);
                        });
                        resizeHandler(this.iframe);
                    }
                    else {
                        // This is only here to support integration testing
                        // see com.atlassian.plugin.connect.test.pageobjects.RemotePage#isNotFullSize()
                        $(this.iframe).addClass("full-size-general-page-fail");
                    }
                })
            }
        };
    });

});

require("resize");
/**
 * Methods for showing the status of a connect-addon (loading, time'd-out etc)
 */

define("host/_status_helper", ["_dollar"], function ($) {
    "use strict";

    var statuses = {
        loading: {
            descriptionHtml: '<div class="small-spinner"></div>Loading add-on...'
        },
        "load-timeout": {
            descriptionHtml: '<div class="small-spinner"></div>Add-on is not responding. Wait or <a href="#" class="ap-btn-cancel">cancel</a>?'
        },

        "load-error": {
            descriptionHtml: 'Add-on failed to load.'
        }
    };

    function hideStatuses($home){
        // If there's a pending timer to show the loading status, kill it.
        if ($home.data('loadingStatusTimer')) {
            clearTimeout($home.data('loadingStatusTimer'));
            $home.removeData('loadingStatusTimer');
        }
        $home.find(".ap-status").addClass("hidden");
    }

    function showStatus($home, status){
        hideStatuses($home);
        $home.closest('.ap-container').removeClass('hidden');
        $home.find(".ap-stats").removeClass("hidden");
        $home.find('.ap-' + status).removeClass('hidden');
        /* setTimout fixes bug in AUI spinner positioning */
        setTimeout(function(){
            var spinner = $home.find('.small-spinner','.ap-' + status);
            if(spinner.length && spinner.spin){
                spinner.spin({zIndex: "1"});
            }
        }, 10);
    }

    //when an addon has loaded. Hide the status bar.
    function showLoadedStatus($home){
        hideStatuses($home);
    }

    function showLoadingStatus($home, delay){
        if (!delay) {
            showStatus($home, 'loading');
        } else {
            // Wait a second before showing loading status.
            var timer = setTimeout(showStatus.bind(null, $home, 'loading'), delay);
            $home.data('loadingStatusTimer', timer);
        }
    }

    function showloadTimeoutStatus($home){
        showStatus($home, 'load-timeout');
    }

    function showLoadErrorStatus($home){
        showStatus($home, 'load-error');
    }

    function createStatusMessages() {
        var i,
        stats = $('<div class="ap-stats" />');

        for(i in statuses){
            var status = $('<div class="ap-' + i + ' ap-status hidden" />');
            status.append('<small>' + statuses[i].descriptionHtml + '</small>');
            stats.append(status);
        }
        return stats;
    }

    return {
        createStatusMessages: createStatusMessages,
        showLoadingStatus: showLoadingStatus,
        showloadTimeoutStatus: showloadTimeoutStatus,
        showLoadErrorStatus: showLoadErrorStatus,
        showLoadedStatus: showLoadedStatus
    };

});

require(["_dollar", "_rpc", "host/_status_helper"], function ($, rpc, statusHelper) {
    "use strict";

    rpc.extend(function (config) {
        return {
            init: function (state, xdm) {
                var $home = $(xdm.iframe).closest(".ap-container");
                statusHelper.showLoadingStatus($home, 0);

                $home.find(".ap-load-timeout a.ap-btn-cancel").click(function () {
                    statusHelper.showLoadErrorStatus($home);
                    if(xdm.analytics && xdm.analytics.iframePerformance){
                        xdm.analytics.iframePerformance.cancel();
                    }
                });

                xdm.timeout = setTimeout(function(){
                    xdm.timeout = null;
                    statusHelper.showloadTimeoutStatus($home);
                    // if inactive, the iframe has been destroyed by the product.
                    if(xdm.isActive() && xdm.analytics && xdm.analytics.iframePerformance){
                        xdm.analytics.iframePerformance.timeout();
                    }
                }, 20000);
            },
            internals: {
                init: function() {
                    if(this.analytics && this.analytics.iframePerformance){
                        this.analytics.iframePerformance.end();
                    }
                    var $home = $(this.iframe).closest(".ap-container");
                    statusHelper.showLoadedStatus($home);

                    clearTimeout(this.timeout);
                    // Let the integration tests know the iframe has loaded.
                    $home.find(".ap-content").addClass("iframe-init");
                }
            }
        };

    });

});

/**
 * Utility methods for rendering connect addons in AUI components
 */

define("host/content", ["_dollar", "_uri"], function ($, uri) {
    "use strict";

    function getWebItemPluginKey(target){
        var cssClass = target.attr('class');
        var m = cssClass ? cssClass.match(/ap-plugin-key-([^\s]*)/) : null;
        return $.isArray(m) ? m[1] : false;
    }
    function getWebItemModuleKey(target){
        var cssClass = target.attr('class');
        var m = cssClass ? cssClass.match(/ap-module-key-([^\s]*)/) : null;
        return $.isArray(m) ? m[1] : false;
    }

    function getOptionsForWebItem(target){
        var moduleKey = getWebItemModuleKey(target),
            type = target.hasClass('ap-inline-dialog') ? 'inlineDialog' : 'dialog';
            return window._AP[type + 'Options'][moduleKey] || {};
    }

    function contextFromUrl (url) {
        var pairs = new uri.init(url).queryPairs;
        var obj = {};
        $.each(pairs, function (key, value) {
            obj[value[0]] = value[1];
        });
        return obj;
    }

    function eventHandler(action, selector, callback) {

        function domEventHandler(event) {
            event.preventDefault();
            var $el = $(event.target).closest(selector),
            href = $el.attr("href"),
            url = new uri.init(href),
            options = {
                bindTo: $el,
                header: $el.text(),
                width:  url.getQueryParamValue('width'),
                height: url.getQueryParamValue('height'),
                cp:     url.getQueryParamValue('cp'),
                key: getWebItemPluginKey($el),
                productContext: contextFromUrl(href)
            };
            callback(href, options, event.type);
        }

        $(window.document).on(action, selector, domEventHandler);

    }

    return {
        eventHandler: eventHandler,
        getOptionsForWebItem: getOptionsForWebItem,
        getWebItemPluginKey: getWebItemPluginKey,
        getWebItemModuleKey: getWebItemModuleKey
    };


});

require(["_dollar", "_rpc"], function ($, rpc) {

  "use strict";

  // Note that if it's desireable to publish host-level events to add-ons, this would be a good place to wire
  // up host listeners and publish to each add-on, rather than using each XdmRpc.events object directly.

    var _channels = {};

  // Tracks all channels (iframes with an XDM bridge) for a given add-on key, managing event propagation
  // between bridges, and potentially between add-ons.

    rpc.extend(function () {

        var self = {
            _emitEvent: function (event) {
                $.each(_channels[event.source.key], function (id, channel) {
                    channel.bus._emitEvent(event);
                });
            },
            remove: function (xdm) {
                var channel = _channels[xdm.addonKey][xdm.id];
                if (channel) {
                    channel.bus.offAny(channel.listener);
                }
                delete _channels[xdm.addonKey][xdm.id];
                return this;
            },
            init: function (config, xdm) {
                if(!_channels[xdm.addonKey]){
                    _channels[xdm.addonKey] = {};
                }
                var channel = _channels[xdm.addonKey][xdm.id] = {
                    bus: xdm.events,
                    listener: function () {
                        var event = arguments[arguments.length - 1];
                        var trace = event.trace = event.trace || {};
                        var traceKey = xdm.id + "|addon";
                        if (!trace[traceKey]) {
                            // Only forward an event once in this listener
                            trace[traceKey] = true;
                            self._emitEvent(event);
                        }
                    }
                };
                channel.bus.onAny(channel.listener); //forward add-on events.

                // Remove reference to destroyed iframes such as closed dialogs.
                channel.bus.on("ra.iframe.destroy", function(){
                    self.remove(xdm);
                }); 
            }
        };
        return self;
    });

});

define("analytics/analytics", ["_dollar"], function($){
    "use strict";

    /**
     * Blacklist certain bridge functions from being sent to analytics
     * @const
     * @type {Array}
     */
    var BRIDGEMETHODBLACKLIST = [
        "resize",
        "init"
    ];

    /**
     * Timings beyond 20 seconds (connect's load timeout) will be clipped to an X.
     * @const
     * @type {int}
     */
    var THRESHOLD = 20000;

    /**
     * Trim extra zeros from the load time.
     * @const
     * @type {int}
     */
    var TRIMPPRECISION = 100;

    function time() {
        return window.performance && window.performance.now ? window.performance.now() : new Date().getTime();
    }

    function Analytics(addonKey, moduleKey) {
        var metrics = {};
        this.addonKey = addonKey;
        this.moduleKey = moduleKey;
        this.iframePerformance = {
            start: function(){
                metrics.startLoading = time();
            },
            end: function(){
                var value = time() - metrics.startLoading;
                proto.track('iframe.performance.load', {
                    addonKey: addonKey,
                    moduleKey: moduleKey,
                    value: value > THRESHOLD ? 'x' : Math.ceil((value) / TRIMPPRECISION)
                });
                delete metrics.startLoading;
            },
            timeout: function(){
                proto.track('iframe.performance.timeout', {
                    addonKey: addonKey,
                    moduleKey: moduleKey
                });
                //track an end event during a timeout so we always have complete start / end data.
                this.end();
            },
            // User clicked cancel button during loading
            cancel: function(){
                proto.track('iframe.performance.cancel', {
                    addonKey: addonKey,
                    moduleKey: moduleKey
                });
            }
        };

    }

    var proto = Analytics.prototype;

    proto.getKey = function () {
        return this.addonKey + ':' + this.moduleKey;
    };

    proto.track = function (name, data) {
        var prefixedName = "connect.addon." + name;
        if(AJS.Analytics){
            AJS.Analytics.triggerPrivacyPolicySafeEvent(prefixedName, data);
        } else if(AJS.trigger) {
            // BTF fallback
            AJS.trigger('analyticsEvent', {
                name: prefixedName,
                data: data
            });
        } else {
            return false;
        }

        return true;
    };

    proto.trackBridgeMethod = function(name){
        if($.inArray(name, BRIDGEMETHODBLACKLIST) !== -1){
            return false;
        }
        this.track('bridge.invokemethod', {
            name: name,
            addonKey: this.addonKey,
            moduleKey: this.moduleKey
        });
    };

    return {
        get: function (addonKey, moduleKey) {
            return new Analytics(addonKey, moduleKey);
        }
    };


});

(function(context){
  "use strict";

  define('host/create', ["_dollar","host/_util", "_rpc", "_ui-params", "analytics/analytics"], function($, utils, rpc, uiParams, analytics){

      var defer = window.requestAnimationFrame || function (f) {setTimeout(f,10); };

      function contentDiv(ns) {
          if(!ns){
            throw new Error("ns undefined");
          }
          return $("#embedded-" + utils.escapeSelector(ns));
      }

      /**
      * @name Options
      * @class
      * @property {String}  ns            module key
      * @property {String}  src           url of the iframe
      * @property {String}  w             width of the iframe
      * @property {String}  h             height of the iframe
      * @property {String}  dlg           is a dialog (disables the resizer)
      * @property {String}  simpleDlg     deprecated, looks to be set when a confluence macro editor is being rendered as a dialog
      * @property {Boolean} general       is a page that can be resized
      * @property {String}  productCtx    context to pass back to the server (project id, space id, etc)
      * @property {String}  key           addon key from the descriptor
      * @property {String}  uid           id of the current user
      * @property {String}  ukey          user key
      * @property {String}  data.timeZone timezone of the current user
      * @property {String}  cp            context path
      */

      /**
      * @param {Options} options These values come from the velocity template and can be overridden using uiParams
      */
      function create(options) {
      if(typeof options.uiParams !== "object"){
        options.uiParams = uiParams.fromUrl(options.src);
      }

      var ns = options.ns,
          contentId = "embedded-" + ns,
          channelId = "channel-" + ns,
          initWidth = options.w || "100%",
          initHeight = options.h || "0";

      if(typeof options.uiParams !== "object"){
        options.uiParams = {};
      }

      if(!!options.general) {
        options.uiParams.isGeneral = true;
      }

      var xdmOptions = {
        remote: options.src,
        remoteKey: options.key,
        container: contentId,
        channel: channelId,
        props: {width: initWidth, height: initHeight},
        uiParams: options.uiParams
      };

      if(options.productCtx && !options.productContext){
        options.productContext = JSON.parse(options.productCtx);
      }

      rpc.extend({
        init: function(opts, xdm){
          xdm.analytics = analytics.get(xdm.addonKey, ns);
          xdm.analytics.iframePerformance.start();
          xdm.productContext = options.productContext;
        }
      });

      rpc.init(options, xdmOptions);

    }

    return function (options) {

      var attemptCounter = 0;
      function doCreate() {
          //If the element we are going to append the iframe to doesn't exist in the dom (yet). Wait for it to appear.
          if(contentDiv(options.ns).length === 0 && attemptCounter < 10){
              setTimeout(function(){
                  attemptCounter++;
                  doCreate();
              }, 50);
              return;
          }

        // create the new iframe
        create(options);
      }
      if (AJS.$.isReady) {
        // if the dom is ready then this is being run during an ajax update;
        // in that case, defer creation until the next event loop tick to ensure
        // that updates to the desired container node's parents have completed
        defer(doCreate);
      } else {
        $(doCreate);
      }

    };

  });

}(this));


    var rpc = require("_rpc");

    return {
        extend: rpc.extend,
        init: rpc.init,
        uiParams: require("_ui-params"),
        create: require('host/create'),
        _uriHelper: require('_uri'),
        _statusHelper: require('host/_status_helper'),
        webItemHelper: require('host/content')
    };
}));;
(function(define, AJS){
    "use strict";
    define("ac/cookie/main", [], function () {

        function prefixCookie (addonKey, name){
            if (!addonKey || addonKey.length === 0) {
                throw new Error('addon key must be defined on cookies');
            }

            if (!name || name.length === 0) {
                throw new Error('Name must be defined');
            }
            return addonKey + '$$' + name;
        }

        return {
            saveCookie: function(addonKey, name, value, expires){
                AJS.Cookie.save(prefixCookie(addonKey, name), value, expires);
            },
            readCookie: function(addonKey, name, callback){
                var value = AJS.Cookie.read(prefixCookie(addonKey, name));
                if (typeof callback === "function") {
                    callback(value);
                }
            },
            eraseCookie: function(addonKey, name){
                 AJS.Cookie.erase(prefixCookie(addonKey, name));
            }
        };
    });
})(define, AJS);
(function(define){
    "use strict";
    define('ac/cookie', ['ac/cookie/main', 'connect-host'], function(cookie, _AP){
        _AP.extend(function () {
            return {
                internals: {
                    saveCookie: function(name, value, expires){
                        cookie.saveCookie(this.addonKey, name, value, expires);
                    },
                    readCookie: function(name, callback){
                        cookie.readCookie(this.addonKey, name, callback);
                    },
                    eraseCookie: function(name){
                        cookie.eraseCookie(this.addonKey, name);
                    }
                }
            };
        });
    });
})(define);
;
(function(define){
    "use strict";
    define("ac/env", ['connect-host'], function (_AP) {

        var connectModuleData; // data sent from the velocity template

        _AP.extend(function () {
            return {
                init: function (state) {
                    connectModuleData = state;
                },
                internals: {
                    getLocation: function () {
                        return window.location.href;
                    }
                }
            };
        });

    });
})(define);
;
(function(define, AJS, $){
    "use strict";
    define("ac/messages/main", [],function() {
        var MESSAGE_BAR_ID = 'ac-message-container',
            MESSAGE_TYPES = ["generic", "error", "warning", "success", "info", "hint"];

        function validateMessageId(msgId){
            return msgId.search(/^ap\-message\-[0-9]+$/) == 0;
        }

        function getMessageBar(){
            var msgBar = $('#' + MESSAGE_BAR_ID);

            if(msgBar.length < 1){
                msgBar = $('<div id="' + MESSAGE_BAR_ID + '" />').appendTo('body');
            }
            return msgBar;
        }

        function filterMessageOptions(options){
            var i,
            key,
            copy = {},
            allowed = ['closeable', 'fadeout', 'delay', 'duration', 'id'];

            for (i in allowed){
                key = allowed[i];
                if (key in options){
                    copy[key] = options[key];
                }
            }

            return copy;
        }

        return {
            showMessage: function (name, title, bodyHTML, options) {
                var msgBar = getMessageBar();

                options = filterMessageOptions(options);
                $.extend(options, {
                    title: title,
                    body: AJS.escapeHtml(bodyHTML)
                });

                if($.inArray(name, MESSAGE_TYPES) < 0){
                    throw "Invalid message type. Must be: " + MESSAGE_TYPES.join(", ");
                }
                if(validateMessageId(options.id)){
                    AJS.messages[name](msgBar, options);
                    // Calculate the left offset based on the content width.
                    // This ensures the message always stays in the centre of the window.
                    msgBar.css('margin-left', '-' + msgBar.innerWidth()/2 + 'px');
                }
            },
            clearMessage: function (id) {
                if(validateMessageId(id)){
                    $('#' + id).remove();
                }
            }
        };
    });
})(define, AJS, AJS.$);
(function(define){
    "use strict";
    define('ac/messages', ["ac/messages/main", 'connect-host'], function(messages, _AP) {
        _AP.extend(function () {
            return {
                internals: {
                    showMessage: function (name, title, body, options) {
                        return messages.showMessage(name, title, body, options);
                    },
                    clearMessage: function (id) {
                        return messages.clearMessage(id);
                    }
                }
            };
        });
    });
})(define);
;
(function(define, AJS, $){
    "use strict";
    define("ac/request", ['connect-host'], function (_AP) {

        var xhrProperties = ["status", "statusText", "responseText"],
            xhrHeaders = ["Content-Type", "ETag"],
            requestHeadersWhitelist = [
                "If-Match", "If-None-Match"
            ],
            contextPath = null;

        _AP.extend(function () {
            return {
                init: function(xdm){
                    contextPath = xdm.cp;
                },
                internals: {
                    request: function (args, success, error) {
                        // add the context path to the request url
                        var url = contextPath + args.url;
                        url = url.replace(/\/\.\.\//ig,''); // strip /../ from urls

                        // reduce the xhr object to the just bits we can/want to expose over the bridge
                        function toJSON (xhr) {
                            var json = {headers: {}};
                            // only copy key properties and headers for transport across the bridge
                            $.each(xhrProperties, function (i, v) { json[v] = xhr[v]; });
                            // only copy key response headers for transport across the bridge
                            $.each(xhrHeaders, function (i, v) { json.headers[v] = xhr.getResponseHeader(v); });
                            return json;
                        }
                        function done (data, textStatus, xhr) {
                            success([data, textStatus, toJSON(xhr)]);
                        }
                        function fail (xhr, textStatus, errorThrown) {
                            error([toJSON(xhr), textStatus, errorThrown]);
                        }

                        var headers = {};
                        $.each(args.headers || {}, function (k, v) { headers[k.toLowerCase()] = v; });
                        // Disable system ajax settings. This stops confluence mobile from injecting callbacks and then throwing exceptions.
                        // $.ajaxSettings = {};

                        // execute the request with our restricted set of inputs
                        var ajaxOptions = {
                            url: url,
                            type: args.type || "GET",
                            data: args.data,
                            dataType: "text", // prevent jquery from parsing the response body
                            contentType: args.contentType,
                            cache: (typeof args.cache !== "undefined") ? !!args.cache : undefined,
                            headers: {
                                // */* will undo the effect on the accept header of having set dataType to "text"
                                "Accept": headers.accept || "*/*",
                                // send the client key header to force scope checks
                                "AP-Client-Key": this.addonKey
                            }
                        };
                        $.each(requestHeadersWhitelist, function(index, header) {
                            if (headers[header.toLowerCase()]) {
                                ajaxOptions.headers[header] = headers[header.toLowerCase()];
                            }
                        });
                        $.ajax(ajaxOptions).then(done, fail);
                    }

                }
            };
        });

    });
})(define, AJS, AJS.$);
;
/**
 * Methods for showing the status of a connect-addon (loading, time'd-out etc)
 */
(function(define){
    "use strict";

    define("ac/history/main", ["connect-host"], function (_AP) {

        var lastAdded,
            anchorPrefix = "!",
            Uri = _AP._uriHelper;

        function stripPrefix (text) {
            if(text === undefined || text === null){
                return "";
            }
            return text.toString().replace(new RegExp("^" + anchorPrefix), "");
        }

        function addPrefix (text) {
            if(text === undefined || text === null){
                throw "You must supply text to prefix";
            }

            return anchorPrefix + stripPrefix(text);
        }

        function changeState (anchor, replace) {
            var currentUrlObj = new Uri.init(window.location.href),
            newUrlObj = new Uri.init(window.location.href);

            newUrlObj.anchor(addPrefix(anchor));

            // If the url has changed.
            if(newUrlObj.anchor() !== currentUrlObj.anchor()){
                lastAdded = newUrlObj.anchor();
                // If it was replaceState or pushState?
                if(replace){
                    window.location.replace("#" + newUrlObj.anchor());
                } else {
                    window.location.assign("#" + newUrlObj.anchor());
                }
                return newUrlObj.anchor();
            }
        }

        function pushState (url) {
            changeState(url);
        }

        function replaceState (url) {
            changeState(url, true);
        }

        function go (delta) {
            history.go(delta);
        }

        function hashChange (event, historyMessage) {
            var newUrlObj = new Uri.init(event.newURL);
            var oldUrlObj = new Uri.init(event.oldURL);
            if( ( newUrlObj.anchor() !== oldUrlObj.anchor() ) && // if the url has changed
                ( lastAdded !== newUrlObj.anchor() ) //  and it's not the page we just pushed.
             ){
                historyMessage(sanitizeHashChangeEvent(event));
            }
            lastAdded = null;
        }

        function sanitizeHashChangeEvent (e) {
            return {
                newURL: stripPrefix(new Uri.init(e.newURL).anchor()),
                oldURL: stripPrefix(new Uri.init(e.oldURL).anchor())
            };
        }

        function getState () {
            var hostWindowUrl = new Uri.init(window.location.href),
            anchor = stripPrefix(hostWindowUrl.anchor());
            return anchor;
        }

        return {
            pushState: pushState,
            replaceState: replaceState,
            go: go,
            hashChange: hashChange,
            getState: getState
        };

    });

})(define);
(function(define, AJS, $){
    "use strict";
    define('ac/history', ['ac/history/main', 'connect-host'], function(cHistory, _AP){

        _AP.extend(function(){
            return {
                init: function (state, xdm) {
                    if(state.uiParams.isGeneral){
                        // register for url hash changes to invoking history.popstate callbacks.
                        $(window).on("hashchange", function(e){
                            cHistory.hashChange(e.originalEvent, xdm.historyMessage);
                        });
                    }
                },
                internals: {
                    historyPushState: function (url) {
                        if(this.uiParams.isGeneral){
                            return cHistory.pushState(url);
                        } else {
                            AJS.log("History is only available to page modules");
                        }
                    },
                    historyReplaceState: function (url) {
                        if(this.uiParams.isGeneral){
                            return cHistory.replaceState(url);
                        } else {
                            AJS.log("History is only available to page modules");
                        }
                    },
                    historyGo: function (delta) {
                        if(this.uiParams.isGeneral){
                            return cHistory.go(delta);
                        } else {
                            AJS.log("History is only available to page modules");
                        }
                    }
                },
                stubs: ["historyMessage"]
            };
        });

    });
})(define, AJS, AJS.$);
;
(function(define, $){
    "use strict";
    define("ac/dialog/button", [], function() {

        function button(options){
            this.$el = $('<button />')
                .text(options.text)
                .addClass('aui-button aui-button-' + options.type)
                .addClass(options.additionalClasses);

            this.isEnabled = function(){
                return !(this.$el.attr('aria-disabled') === "true");
            };

            this.setEnabled = function(enabled){
                //cannot disable a noDisable button
                if(options.noDisable === true){
                    return false;
                }
                this.$el.attr('aria-disabled', !enabled);
                return true;
            };

            this.setEnabled(true);

            this.click = function(listener){
                if (listener) {
                    this.$el.unbind("ra.dialog.click");
                    this.$el.bind("ra.dialog.click", listener);
                } else {
                    this.dispatch(true);
                }
            };

            this.dispatch = function (result) {
                var name = result ? "done" : "fail";
                options.actions && options.actions[name] && options.actions[name]();
            };

            this.setText = function(text){
                if(text){
                    this.$el.text(text);
                }
            };

        }

        return {
            submit: function(actions){
                return new button({
                    type: 'primary',
                    text: 'Submit',
                    additionalClasses: 'ap-dialog-submit',
                    actions: actions
                });
            },
            cancel: function(actions){
                return new button({
                    type: 'link',
                    text: 'Cancel',
                    noDisable: true,
                    additionalClasses: 'ap-dialog-cancel',
                    actions: actions
                });
            }
        };

    });
})(define, AJS.$);

;
(function(define, require, AJS, $){
    "use strict";
    define("ac/dialog", ["connect-host", "ac/dialog/button"], function(connect, dialogButton) {

        var $global = $(window);
        var idSeq = 0;
        var $nexus;
        var dialog;
        var dialogId;

        var buttons = {
            submit: dialogButton.submit({
                done: closeDialog
            }),
            cancel: dialogButton.cancel({
                done: closeDialog
            })
        };

        var keyPressListener = function(e){
            if(e.keyCode === 27 && dialog && dialog.hide){
                dialog.hide();
                $(document).unbind("keydown", keyPressListener);
            }
        };

        function createDialogElement(options, $nexus, chromeless){
            var $el,
            extraClasses = ['ap-aui-dialog2'];

            if(chromeless){
                extraClasses.push('ap-aui-dialog2-chromeless');
            }

            $el = $(aui.dialog.dialog2({
                id: options.id,
                titleText: options.header,
                titleId: options.titleId,
                size: options.size,
                extraClasses: extraClasses,
                removeOnHide: true,
                footerActionContent: true,
                modal: true
            }));

            if(chromeless){
                $el.find('header, footer').remove();
            } else {
                buttons.submit.setText(options.submitText);
                buttons.cancel.setText(options.cancelText);
                //soy templates don't support sending objects, so make the template and bind them.
                $el.find('.aui-dialog2-footer-actions').empty().append(buttons.submit.$el, buttons.cancel.$el);
            }

            $el.find('.aui-dialog2-content').append($nexus);
            $nexus.data('ra.dialog.buttons', buttons);

            function handler(button) {
                // ignore clicks on disabled links
                if(button.isEnabled()){
                    button.$el.trigger("ra.dialog.click", button.dispatch);
                }
            }

            $.each(buttons, function(i, button) {
                button.$el.click(function(){
                    handler(button);
                });
            });

            return $el;
        }

        function displayDialogContent($container, options){
            $container.append('<div id="embedded-' + options.ns + '" class="ap-dialog-container ap-content" />');
        }


        function parseDimension(value, viewport) {
            if (typeof value === "string") {
                var percent = value.indexOf("%") === value.length - 1;
                value = parseInt(value, 10);
                if (percent) value = value / 100 * viewport;
            }
            return value;
        }

        function closeDialog() {
            if ($nexus) {
                // Signal the XdmRpc for the dialog's iframe to clean up
                $nexus.trigger("ra.iframe.destroy")
                .removeData("ra.dialog.buttons")
                .unbind();
                // Clear the nexus handle to allow subsequent dialogs to open
                $nexus = null;
            }
            dialog.hide();
        }

        return {
            id: dialogId,
            getButton: function(name){
                var buttons = $nexus ? $nexus.data('ra.dialog.buttons') : null;
                return (name) && (buttons) ? buttons[name] : buttons;
            },

            /**
            * Constructs a new AUI dialog. The dialog has a single content panel containing a single iframe.
            * The iframe's content is either created by loading [options.src] as the iframe url. Or fetching the content from the server by add-on key + module key.
            *
            * @param {Object} options Options to configure the behaviour and appearance of the dialog.
            * @param {String} [options.header="Remotable Plugins Dialog Title"]  Dialog header.
            * @param {String} [options.headerClass="ap-dialog-header"] CSS class to apply to dialog header.
            * @param {String|Number} [options.width="50%"] width of the dialog, expressed as either absolute pixels (eg 800) or percent (eg 50%)
            * @param {String|Number} [options.height="50%"] height of the dialog, expressed as either absolute pixels (eg 600) or percent (eg 50%)
            * @param {String} [options.id] ID attribute to assign to the dialog. Default to "ap-dialog-n" where n is an autoincrementing id.
            */
            create: function(options, showLoadingIndicator) {

                var defaultOptions = {
                        // These options really _should_ be provided by the caller, or else the dialog is pretty pointless
                        width: "50%",
                        height: "50%"
                    },
                    dialogId = options.id || "ap-dialog-" + (idSeq += 1),
                    mergedOptions = $.extend({id: dialogId}, defaultOptions, options, {dlg: 1}),
                    dialogElement;

                // patch for an old workaround where people would make 100% height / width dialogs.
                if(mergedOptions.width === "100%" && mergedOptions.height === "100%"){
                    mergedOptions.size = "maximum";
                }

                mergedOptions.w = parseDimension(mergedOptions.width, $global.width());
                mergedOptions.h = parseDimension(mergedOptions.height, $global.height());

                $nexus = $("<div />").addClass("ap-servlet-placeholder ap-container").attr('id', 'ap-' + options.ns)
                .bind("ra.dialog.close", closeDialog);

                if(options.chrome){
                    dialogElement = createDialogElement(mergedOptions, $nexus);

                } else {
                    dialogElement = createDialogElement(mergedOptions, $nexus, true);
                }

                if(options.size){
                    mergedOptions.w = "100%";
                    mergedOptions.h = "100%";
                } else {
                    AJS.layer(dialogElement).changeSize(mergedOptions.w, mergedOptions.h);
                    dialogElement.removeClass('aui-dialog2-medium'); // this class has a min-height so must be removed.
                }

                dialog = AJS.dialog2(dialogElement);
                dialog.on("hide", closeDialog);
                // ESC key closes the dialog
                $(document).on("keydown", keyPressListener);

                $.each(buttons, function(name, button) {
                    button.click(function () {
                        button.dispatch(true);
                    });
                });

                displayDialogContent($nexus, mergedOptions);

                if(showLoadingIndicator !== false){
                    $nexus.append(connect._statusHelper.createStatusMessages());
                }

                //difference between a webitem and opening from js.
                if(options.src){
                    _AP.create(mergedOptions);
                }

                // give the dialog iframe focus so it can capture keypress events, etc.
                // the 'iframe' selector needs to be specified, otherwise Firefox won't focus the iframe
                dialogElement.on('ra.iframe.create', 'iframe', function () {
                    this.focus();
                });

                dialog.show();

            },
            close: closeDialog
        };

    });
})(define, require, AJS, AJS.$);

AJS.toInit(function ($) {

    (function(require, AJS){
        if(typeof window._AP !== "undefined"){
            //_AP.dialog global fallback.
            require(['ac/dialog'], function(dialog){
                _AP.Dialog = dialog;
            });
        }
    })(require, AJS);
});

(function(define, $){
    "use strict";
    define("ac/dialog/dialog-factory", ["ac/dialog"], function(dialog) {

        //might rename this, it opens a dialog by first working out the url (used for javascript opening a dialog).
        /**
        * opens a dialog by sending the add-on and module keys back to the server for signing.
        * Used by dialog-pages, confluence macros and opening from javascript.
        * @param {Object} options for passing to AP.create
        * @param {Object} dialog options (width, height, etc)
        * @param {String} productContextJson pass context back to the server
        */
        return function(options, dialogOptions, productContext) {
            var promise,
            container,
            uiParams = $.extend({isDialog: 1}, options.uiParams);

            dialog.create({
                id: options.id,
                ns: options.moduleKey || options.key,
                chrome: dialogOptions.chrome || options.chrome,
                header: dialogOptions.header,
                width: dialogOptions.width,
                height: dialogOptions.height,
                size: dialogOptions.size,
                submitText: dialogOptions.submitText,
                cancelText: dialogOptions.cancelText
            }, false);

            container = $('.ap-dialog-container');
            if(options.url){
                throw new Error('Cannot retrieve dialog content by URL');
            }

            promise = window._AP.contentResolver.resolveByParameters({
                addonKey: options.key,
                moduleKey: options.moduleKey,
                productContext: productContext,
                uiParams: uiParams
            });

            promise
                .done(function(data) {
                    var dialogHtml = $(data);
                    dialogHtml.addClass('ap-dialog-container');
                    container.replaceWith(dialogHtml);
                })
                .fail(function(xhr, status, ex) {
                    var title = $("<p class='title' />").text("Unable to load add-on content. Please try again later.");
                    var msg = status + (ex ? ": " + ex.toString() : "");
                    container.html("<div class='aui-message error ap-aui-message'></div>");
                    container.find(".error").text(msg);
                    container.find(".error").prepend(title);
                    AJS.log(msg);
                });

            return dialog;
        };
    });
})(define, AJS.$);

(function(require, $){
    "use strict";
    require(["connect-host", "ac/dialog/dialog-factory", "ac/dialog"], function (connect, dialogFactory, dialogMain) {

        connect.extend(function () {
            return {
                stubs: ["dialogMessage"],
                init: function(state, xdm){
                    // fallback for old connect p2 plugin.
                    if(state.dlg === "1"){
                        xdm.uiParams.isDialog = true;
                    }

                    if(xdm.uiParams.isDialog){
                        var buttons = dialogMain.getButton();
                        if(buttons){
                            $.each(buttons, function(name, button) {
                                button.click(function (e, callback) {
                                    if(xdm.isActive() && xdm.buttonListenerBound){
                                        xdm.dialogMessage(name, callback);
                                    } else {
                                        callback(true);
                                    }
                                });
                            });
                        }
                    }
                },
                internals: {
                    dialogListenerBound: function(){
                        this.buttonListenerBound = true;
                    },
                    setDialogButtonEnabled: function (name, enabled) {
                        dialogMain.getButton(name).setEnabled(enabled);
                    },
                    isDialogButtonEnabled: function (name, callback) {
                        var button =  dialogMain.getButton(name);
                        callback(button ? button.isEnabled() : void 0);
                    },
                    createDialog: function (dialogOptions) {
                        var xdmOptions = {
                            key: this.addonKey
                        };

                        //open by key or url. This can be simplified when opening via url is removed.
                        if(dialogOptions.key) {
                            xdmOptions.moduleKey = dialogOptions.key;
                        } else if(dialogOptions.url) {
                            throw new Error('Cannot open dialog by URL, please use module key');
                        }

                        if($(".aui-dialog2 :visible").length !== 0) {
                            throw new Error('Cannot open dialog when a layer is already visible');
                        }

                        dialogFactory(xdmOptions, dialogOptions, this.productContext);

                    },
                    closeDialog: function() {
                        this.events.emit('ra.iframe.destroy');
                        dialogMain.close();
                    }
                }
            };
        });

    });
})(require, AJS.$);

/**
 * Binds all elements with the class "ap-dialog" to open dialogs.
 * TODO: document options
 */
AJS.toInit(function ($) {

    (function(require, AJS){
        "use strict";
        require(["ac/dialog", "ac/dialog/dialog-factory", "connect-host"], function(dialog, dialogFactory, connect) {

            var action = "click",
                selector = ".ap-dialog",
                callback = function(href, options){

                    var webItemOptions = connect.webItemHelper.getOptionsForWebItem(options.bindTo),
                    moduleKey = connect.webItemHelper.getWebItemModuleKey(options.bindTo),
                    addonKey = connect.webItemHelper.getWebItemPluginKey(options.bindTo);

                    $.extend(options, webItemOptions);

                    if (!options.ns) {
                        options.ns = moduleKey;
                    }
                    if(!options.container){
                        options.container = options.ns;
                    }

                    // webitem target options can sometimes be sent as strings.
                    if(typeof options.chrome === "string"){
                        options.chrome = (options.chrome.toLowerCase() === "false") ? false : true;
                    }

                    //default chrome to be true for backwards compatibility with webitems
                    if(options.chrome === undefined){
                      options.chrome = true;
                    }

                    dialogFactory({
                        key: addonKey,
                        moduleKey: moduleKey
                    }, options,
                    options.productContext);
                };

            connect.webItemHelper.eventHandler(action, selector, callback);
        });
    })(require, AJS);
});

;
(function(define, $){
    "use strict";
    define("ac/inline-dialog", ["connect-host"], function (connect) {

        function getInlineDialog($content){
            return $content.closest('.contents').data('inlineDialog');
        }

        function showInlineDialog($content) {
            getInlineDialog($content).show();
        }

        function resizeInlineDialog($content, width, height) {
            $content.closest('.contents').css({width: width, height: height});
            refreshInlineDialog($content);
        }

        function refreshInlineDialog($content) {
            getInlineDialog($content).refresh();
        }

        function hideInlineDialog($content){
            getInlineDialog($content).hide();
        }

        connect.extend(function () {
            return {
                init: function(state, xdm){
                    if(xdm.uiParams.isInlineDialog){
                        $(xdm.iframe).closest(".ap-container").on("resized", function(e, dimensions){
                            resizeInlineDialog($(xdm.iframe), dimensions.width, dimensions.height);
                        });
                    }
                },
                internals: {
                    hideInlineDialog: function(){
                        hideInlineDialog($(this.iframe));
                    }
                }
            };
        });

    });

})(define, AJS.$);
(function(define, AJS, $){
    "use strict";
    define("ac/inline-dialog/simple", ["connect-host"], function(connect) {

        return function (contentUrl, options) {
            var $inlineDialog;

            // Find the web-item that was clicked, we'll be needing its ID.
            if (!options.bindTo || !options.bindTo.jquery) {
                return;
            }

            var webItem = options.bindTo.hasClass("ap-inline-dialog") ? options.bindTo : options.bindTo.closest(".ap-inline-dialog");
            var itemId = webItem.attr("id");
            if (!itemId) {
                return;
            }

            var displayInlineDialog = function(content, trigger, showInlineDialog) {

                trigger = $(trigger); // sometimes it's not jQuery. Lets make it jQuery.
                content.data('inlineDialog', $inlineDialog);
                var pluginKey = connect.webItemHelper.getWebItemPluginKey(trigger),
                    moduleKey = connect.webItemHelper.getWebItemModuleKey(trigger),
                    promise = window._AP.contentResolver.resolveByParameters({
                        addonKey: pluginKey,
                        moduleKey: moduleKey,
                        isInlineDialog: true,
                        productContext: options.productContext,
                        uiParams: {
                            isInlineDialog: true
                        }
                    });

                promise.done(function(data) {
                    content.empty().append(data);
                    // if target options contain width and height. set it.
                    if(options.width || options.height){
                        content.css({width: options.width, height: options.height});
                    }
                })
                .fail(function(xhr, status, ex) {
                    var title = $("<p class='title' />").text("Unable to load add-on content. Please try again later.");
                    content.html("<div class='aui-message error ap-aui-message'></div>");
                    content.find(".error").append(title);
                    var msg = status + (ex ? ": " + ex.toString() : "");
                    content.find(".error").text(msg);
                    AJS.log(msg);
                })
                .always(function(){
                    showInlineDialog();
                });

            };

            var dialogElementIdentifier = "ap-inline-dialog-content-" + itemId;

            $inlineDialog = $(document.getElementById("inline-dialog-" + dialogElementIdentifier));

            if($inlineDialog.length !== 0){
                $inlineDialog.remove();
            }

            //Create the AUI inline dialog with a unique ID.
            $inlineDialog = AJS.InlineDialog(
                options.bindTo,
                //assign unique id to inline Dialog
                dialogElementIdentifier,
                displayInlineDialog,
                options
            );

            return {
                id: $inlineDialog.attr('id'),
                show: function() {
                    $inlineDialog.show();
                },
                hide: function() {
                    $inlineDialog.hide();
                }
            };

        };

    });
})(define, AJS, AJS.$);
;
AJS.toInit(function ($) {
    (function(require, AJS){
        "use strict";
            require(["ac/inline-dialog/simple", "connect-host"], function(simpleInlineDialog, _AP) {

            var inlineDialogTrigger = '.ap-inline-dialog';
            var action = "click mouseover mouseout",
                callback = function(href, options, eventType){
                    var webItemOptions = _AP.webItemHelper.getOptionsForWebItem(options.bindTo);
                    $.extend(options, webItemOptions);
                    if(options.onHover !== "true" && eventType !== 'click'){
                        return;
                    }

                    // don't repeatedly open if already visible as dozens of mouse-over events are fired in quick succession
                    if (options.onHover === true && options.bindTo.hasClass('active')) {
                        return;
                    }
                    simpleInlineDialog(href, options).show();
                };
            _AP.webItemHelper.eventHandler(action, inlineDialogTrigger, callback);
        });
    })(require, AJS);
});

