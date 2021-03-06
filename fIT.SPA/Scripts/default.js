﻿// seen at HTML5 rocks: http://www.html5rocks.com/de/tutorials/appcache/beginner/ (28.08.2015)

// Check if a new cache is available on page load.
window.addEventListener('load', function (e) {

  window.applicationCache.addEventListener('updateready', function (e) {
    if (window.applicationCache.status === window.applicationCache.UPDATEREADY) {
      // Browser downloaded a new app cache.
      // Swap it in and reload the page to get the new hotness.
      window.applicationCache.swapCache();
      if (confirm('A new version of this site is available. Load it?')) {
        window.location.reload();
      }
    } else {
      // Manifest didn't changed. Nothing new to server.
    }
  }, false);

}, false);

//fix fuer Anzeige Bug im Andriod
$(function () {
  var nua = navigator.userAgent;
    var isAndroid = (nua.indexOf('Mozilla/5.0') > -1 && nua.indexOf('Android ') > -1 && nua.indexOf('AppleWebKit') > -1 && nua.indexOf('Chrome') === -1);
    if (isAndroid) {
        $('select.form-control').removeClass('form-control').css('width', '100%');
    }
})

$('#this-year').val((new Date).getYear());