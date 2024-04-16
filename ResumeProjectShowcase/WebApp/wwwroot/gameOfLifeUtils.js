function toIso(x, y, centerX, centerY, cellSize) {
    return {
        isoX: centerX + (x - y) * cellSize * 0.5,
        isoY: centerY + (x + y) * cellSize * 0.25
    };
}

function fromIso(isoX, isoY, centerX, centerY, cellSize) {
    var x = (2 * (isoX - centerX) + 4 * (isoY - centerY)) / cellSize;
    var y = (4 * (isoY - centerY) - 2 * (isoX - centerX)) / cellSize;
    return {
        x: Math.floor(x / 2),
        y: Math.floor(y / 2)
    };
}

function getRandomColor() {
    var colors = [
        "rgba(255, 243, 199, 1)",
        "rgba(254, 199, 180, 1)",
        "rgba(252, 129, 158, 1)",
        "rgba(247, 65, 143, 1)"
    ];
    return colors[Math.floor(Math.random() * colors.length)];
}

function initGameOfLife() {
    var canvas = document.getElementById("gameCanvas");
    if (!canvas) return;
    var ctx = canvas.getContext("2d");
    var cellSizeControl = 10;
    var cellSize = Math.floor(Math.min(window.innerWidth, window.innerHeight) / cellSizeControl);
    var cols, rows, centerX, centerY, maxDistance;
    var grid;
    var intervalId;
    var minAlpha = 0.1;
    var maxAlpha = 0.666;
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        cellSize = Math.floor(Math.min(canvas.width, canvas.height) / cellSizeControl);
        cols = Math.floor(canvas.width / cellSize);
        rows = Math.floor(canvas.height / cellSize);
        centerX = canvas.width / 2;
        centerY = canvas.height / 2;
        maxDistance = Math.sqrt(centerX * centerX + centerY * centerY) / 1.1;
        grid = new Array(cols * 10).fill(null).map(() => new Array(rows * 10).fill(null));
        if (intervalId) clearInterval(intervalId);
        intervalId = setInterval(window.updateGrid, 1000);
        draw();
    }
    window.onresize = resizeCanvas;
    resizeCanvas();
    function drawGridLines() {
        var longDashPattern = [15, 20];
        var shortDashPattern = [15, 20];
        for (let i = -10 * cols; i <= 10 * cols; i++) {
            for (let j = -10 * rows; j <= 10 * rows; j++) {
                var start = toIso(i, j, centerX, centerY, cellSize);
                var endRight = toIso(i + 1, j, centerX, centerY, cellSize);
                var endDown = toIso(i, j + 1, centerX, centerY, cellSize);
                var distance = Math.sqrt(Math.pow(start.isoX - centerX, 2) + Math.pow(start.isoY - centerY, 2));
                var normalizedDistance = distance / maxDistance;
                var alpha = maxAlpha * (1 - normalizedDistance) + minAlpha * normalizedDistance;
                ctx.globalAlpha = alpha;
                // Set the dash pattern for one direction of lines
                ctx.setLineDash(longDashPattern);
                ctx.beginPath();
                ctx.moveTo(start.isoX, start.isoY);
                ctx.lineTo(endRight.isoX, endRight.isoY);
                ctx.stroke();
                // Set a different dash pattern for the other direction
                ctx.setLineDash(shortDashPattern);
                ctx.beginPath();
                ctx.moveTo(start.isoX, start.isoY);
                ctx.lineTo(endDown.isoX, endDown.isoY);
                ctx.stroke();
            }
        }
        // Reset the dash pattern to solid
        ctx.setLineDash([]);
        ctx.globalAlpha = 1;
    }
    canvas.addEventListener("mousemove", function (e) {
        var rect = canvas.getBoundingClientRect();
        var mouseIsoX = e.clientX - rect.left;
        var mouseIsoY = e.clientY - rect.top;
        var gridPos = fromIso(mouseIsoX, mouseIsoY, centerX, centerY, cellSize);
        var gridX = gridPos.x + cols;
        var gridY = gridPos.y + rows;
        grid[gridX][gridY] = getRandomColor();
        draw();
    });
    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        drawGridLines();
        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                if (grid[x][y]) {
                    var pos = toIso(x - cols, y - rows, centerX, centerY, cellSize);
                    ctx.fillStyle = grid[x][y];
                    ctx.beginPath();
                    ctx.moveTo(pos.isoX, pos.isoY - cellSize / 4);
                    ctx.lineTo(pos.isoX + cellSize / 2, pos.isoY);
                    ctx.lineTo(pos.isoX, pos.isoY + cellSize / 4);
                    ctx.lineTo(pos.isoX - cellSize / 2, pos.isoY);
                    ctx.closePath();
                    var distance = Math.sqrt(Math.pow((pos.isoX / 2) - centerX, 2) + Math.pow((pos.isoY / 2) - centerY, 2));
                    var normalizedDistance = distance / maxDistance;
                    var alpha = maxAlpha * (1 - normalizedDistance) + minAlpha * normalizedDistance;
                    ctx.globalAlpha = alpha;
                    ctx.fill();
                }
            }
        }
        ctx.globalAlpha = 1;
    }
    window.updateGrid = function () {
        var newGrid = new Array(cols * 10).fill(false).map(() => new Array(rows * 10).fill(false));
        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                var aliveNeighbors = 0;
                for (let i = -1; i <= 1; i++) {
                    for (let j = -1; j <= 1; j++) {
                        if (i === 0 && j === 0) continue;
                        var nx = x + i;
                        var ny = y + j;
                        if (nx < 0 || nx >= grid.length || ny < 0 || ny >= grid[x].length) continue;
                        if (grid[nx][ny]) aliveNeighbors++;
                    }
                }
                if (grid[x][y] && (aliveNeighbors === 2 || aliveNeighbors === 3)) {
                    newGrid[x][y] = true;
                } else if (!grid[x][y] && aliveNeighbors === 3) {
                    newGrid[x][y] = true;
                }
            }
        }
        grid = newGrid;
        draw();
    };
    setInterval(window.updateGrid, 1000);
    draw();
}