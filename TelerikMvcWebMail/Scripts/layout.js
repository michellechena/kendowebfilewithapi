var baseUrl = $("#BaseUrl").data("baseurl");
var APIBaseUrl = $("#APIBaseUrl").data("baseurl");

function onClick(e) {
    location.href = baseUrl + '/SignIn/Index';
    return false;
}
