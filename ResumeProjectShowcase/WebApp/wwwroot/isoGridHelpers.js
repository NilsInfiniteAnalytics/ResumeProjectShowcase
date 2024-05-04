var isoScaleX = 0.866;
var isoScaleY = 0.5;
var rotationAngleDegrees = 12;
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
function drawGridLines(ctx, canvas, rows, cols, centerX, centerY, cellSize) {
    var longDashPattern = [5, 10];
    var shortDashPattern = [2, 10];
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.strokeStyle = "rgba(0, 0, 0, 0.33)"; // Light grey lines
    // Draw horizontal lines
    var start;
    var end;
    for (let j = -rows; j <= rows; j++) {
        start = toIso(-cols, j, centerX, centerY, cellSize);
        end = toIso(cols, j, centerX, centerY, cellSize);
        ctx.beginPath();
        ctx.setLineDash(longDashPattern);
        ctx.moveTo(start.isoX, start.isoY);
        ctx.lineTo(end.isoX, end.isoY);
        ctx.stroke();
    }
    // Draw vertical lines
    for (let i = -cols; i <= cols; i++) {
        start = toIso(i, -rows, centerX, centerY, cellSize);
        end = toIso(i, rows, centerX, centerY, cellSize);
        ctx.beginPath();
        ctx.setLineDash(shortDashPattern);
        ctx.moveTo(start.isoX, start.isoY);
        ctx.lineTo(end.isoX, end.isoY);
        ctx.stroke();
    }
}