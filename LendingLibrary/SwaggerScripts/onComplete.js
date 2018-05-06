/*
    Stolen from StackOverflow: 

    https://stackoverflow.com/questions/29275499/swagger-swashbuckle-oauth2-with-resource-owner-password-credentials-grant

    Hijacks the API Key field to pass a user name and password for password OAuth2 flow
*/

$('#explore').off();

$('#input_apiKey')[0].placeholder = 'username:password';

$('#explore').click(function () {
   var key = $('#input_apiKey')[0].value;
   var credentials = key.split(':'); //username:password expected

$.ajax({
    url: "/Token",
    type: "post",
    contenttype: 'x-www-form-urlencoded',
    data: "grant_type=password&username=" + credentials[0] + "&password=" + credentials[1],
    success: function (response) {
        var bearerToken = 'Bearer ' + response.access_token;

        window.swaggerUi.api.clientAuthorizations.add('Authorization', new SwaggerClient.ApiKeyAuthorization('Authorization', bearerToken, 'header'));
        window.swaggerUi.api.clientAuthorizations.remove("api_key");
        alert("Login successfull");
       },
       error: function (xhr, ajaxoptions, thrownerror) {
        alert("Login failed!");
       }
    });
});
