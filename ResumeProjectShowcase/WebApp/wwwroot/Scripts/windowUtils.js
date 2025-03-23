function getWindowSize() {
    window.DotNet.invokeMethodAsync("WebApp",
        "UpdateWindowSize",
        `${window.innerWidth} x ${window.innerHeight}`);
}

window.getElementDimensions = (element) => {
    if (element) {
        const rect = element.getBoundingClientRect();
        return { width: rect.width, height: rect.height };
    }
    return { width: 0, height: 0 };
};

window.setupMapResizeListener = (dotNetReference) => {
    window.addEventListener("resize", () => {
        if (window.resizeTimeout) {
            clearTimeout(window.resizeTimeout);
        }
        window.resizeTimeout = setTimeout(() => {
            dotNetReference.invokeMethodAsync("WindowResized");
        }, 250);
    });
    return true;
};

function triggerResize() {
    window.dispatchEvent(new Event("resize"));
}

window.addEventListener("resize", getWindowSize);
