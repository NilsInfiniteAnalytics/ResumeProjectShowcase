function getWindowSize() {
    window.DotNet.invokeMethodAsync("WebApp",
        "UpdateWindowSize",
        `${window.innerWidth} x ${window.innerHeight}`);
}

function getClientTimezoneOffset() {
    var offset = new Date().getTimezoneOffset();
    DotNet.invokeMethodAsync('WebApp', 'SetTimezoneOffset', offset);
}

function triggerResize() {
    window.dispatchEvent(new Event('resize'));
}

window.addEventListener("resize", getWindowSize);