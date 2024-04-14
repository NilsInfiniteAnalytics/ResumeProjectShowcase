function getWindowSize() {
    window.DotNet.invokeMethodAsync("WebApp",
        "UpdateWindowSize",
        `${window.innerWidth} x ${window.innerHeight}`);
}

function triggerResize() {
    window.dispatchEvent(new Event('resize'));
}

window.addEventListener("resize", getWindowSize);