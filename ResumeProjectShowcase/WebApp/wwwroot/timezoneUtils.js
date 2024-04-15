function getClientTimezoneOffset() {
    var offset = new Date().getTimezoneOffset();
    DotNet.invokeMethodAsync("WebApp", "SetTimezoneOffset", offset);
}