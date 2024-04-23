var isoScaleX = 0.866;
var isoScaleY = 0.5;
var rotationAngleDegrees = 20;
var rotationAngleRadians = rotationAngleDegrees * (Math.PI / 180);
function toIso(x, y, centerX, centerY, cellSize) {
    var rotatedX = x * Math.cos(rotationAngleRadians) - y * Math.sin(rotationAngleRadians);
    var rotatedY = x * Math.sin(rotationAngleRadians) + y * Math.cos(rotationAngleRadians);
    return {
        isoX: centerX + rotatedX * cellSize * isoScaleX,
        isoY: centerY + rotatedY * cellSize * isoScaleY
    };
}
function fromIso(isoX, isoY, centerX, centerY, cellSize) {
    var unscaledX = (isoX - centerX) / (cellSize * isoScaleX);
    var unscaledY = (isoY - centerY) / (cellSize * isoScaleY);
    var rad = -rotationAngleRadians;
    var cosAngle = Math.cos(rad);
    var sinAngle = Math.sin(rad);
    var x = unscaledX * cosAngle - unscaledY * sinAngle;
    var y = unscaledX * sinAngle + unscaledY * cosAngle;
    return {
        x: x,
        y: y
    };
}
function initGameOfLife() {
    var canvas = document.getElementById('gameCanvas');
    if (!canvas) return;
    var ctx = canvas.getContext('2d');
    var cellSizeControl = 10;
    var cols, rows, centerX, centerY, maxDistance;
    var grid;
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        cellSize = Math.floor(Math.min(window.innerWidth, window.innerHeight) / cellSizeControl);
        cols = Math.floor(canvas.width / cellSize);
        rows = Math.floor(canvas.height / cellSize);
        centerX = canvas.width / 2;
        centerY = canvas.height / 2;
        maxDistance = Math.sqrt(centerX * centerX + centerY * centerY);
        grid = new Array(cols).fill(null).map(() => new Array(rows).fill(false));
        drawGridLines();
    }
    window.onresize = resizeCanvas;
    resizeCanvas();
    function drawIsoSquare(ctx, gridX, gridY, centerX, centerY, cellSize) {
        var start = toIso(gridX, gridY, centerX, centerY, cellSize); // Get the top-left corner of the square
        // Calculate the other corners of the square based on cellSize and iso scaling
        var points = [
            start,
            toIso(gridX + 1, gridY, centerX, centerY, cellSize),
            toIso(gridX + 1, gridY + 1, centerX, centerY, cellSize),
            toIso(gridX, gridY + 1, centerX, centerY, cellSize)
        ];
        ctx.fillStyle = 'rgba(255, 0, 0, 0.5)'; // Semi-transparent red
        ctx.beginPath();
        ctx.moveTo(points[0].isoX, points[0].isoY);
        for (var i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].isoX, points[i].isoY);
        }
        ctx.closePath();
        ctx.fill();
    }
    function drawGridLines() {
        var longDashPattern = [5, 10];
        var shortDashPattern = [2, 10];
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.strokeStyle = 'rgba(0, 0, 0, 0.5)'; // Light grey lines
        // Draw horizontal lines
        for (let j = -rows; j <= rows; j++) {
            var start = toIso(-cols, j, centerX, centerY, cellSize);
            var end = toIso(cols, j, centerX, centerY, cellSize);
            ctx.beginPath();
            ctx.setLineDash(longDashPattern);
            ctx.moveTo(start.isoX, start.isoY);
            ctx.lineTo(end.isoX, end.isoY);
            ctx.stroke();
        }
        // Draw vertical lines
        for (let i = -cols; i <= cols; i++) {
            var start = toIso(i, -rows, centerX, centerY, cellSize);
            var end = toIso(i, rows, centerX, centerY, cellSize);
            ctx.beginPath();
            ctx.setLineDash(shortDashPattern);
            ctx.moveTo(start.isoX, start.isoY);
            ctx.lineTo(end.isoX, end.isoY);
            ctx.stroke();
        }
    }
    canvas.addEventListener('mousemove', function (e) {
        var rect = canvas.getBoundingClientRect();
        var mouseX = e.clientX - rect.left;
        var mouseY = e.clientY - rect.top;
        var gridCoords = fromIso(mouseX, mouseY, centerX, centerY, cellSize);
        var gridX = Math.floor(gridCoords.x);
        var gridY = Math.floor(gridCoords.y);
        drawIsoSquare(ctx, gridX, gridY, centerX, centerY, cellSize);
        console.log("Mouse at (" + mouseX + ", " + mouseY + ") is over grid square (" + gridX + ", " + gridY + ")");
        console.log("Grid Location: " + grid[gridX][gridY]);
    });
}