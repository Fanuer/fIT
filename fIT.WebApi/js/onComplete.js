$('#input_apiKey').change(function () {
    var key = $('#input_apiKey')[0].value;
    var credentials = key.split(':'); //username:password expected
    $.ajax({
        url: "/api/accounts/login",
        type: "post",
        contenttype: 'x-www-form-urlencoded',
        data: "grant_type=password&username=" + credentials[0] + "&password=" + credentials[1],
        success: function (response) {
            var bearerToken = 'Bearer ' + response.access_token;
            window.authorizations.add('key', new ApiKeyAuthorization('Authorization', bearerToken, 'header'));
        },
        error: function (xhr, ajaxoptions, thrownerror) {
            alert("Login failed!");
        }
    });
});