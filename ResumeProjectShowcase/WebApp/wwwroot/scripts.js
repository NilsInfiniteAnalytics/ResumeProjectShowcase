function getWindowSize() {
    console.log("Resizing: " + `${window.innerWidth} x ${window.innerHeight}`);
    window.DotNet.invokeMethodAsync("WebApp",
        "UpdateWindowSize",
        `${window.innerWidth} x ${window.innerHeight}`);
}

window.addEventListener("resize", getWindowSize);