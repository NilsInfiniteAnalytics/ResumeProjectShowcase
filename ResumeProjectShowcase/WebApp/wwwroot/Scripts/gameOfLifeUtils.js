﻿var currentInterval;

function clearGrid() {
    if (currentInterval) {
        clearInterval(currentInterval);
        currentInterval = null;
    }
    var canvas = document.getElementById("gameCanvas");
    if (canvas) {
        var ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        canvas.removeEventListener("mousemove", handleMouseMove);
    }
}

function handleMouseMove(e) { }

function getRandomColor(alpha = 0.666) {
    var colors = [
        `rgba(255, 243, 199, ${alpha})`,
        `rgba(254, 199, 180, ${alpha})`,
        `rgba(252, 129, 158, ${alpha})`,
        `rgba(247, 65, 143, ${alpha})`
    ];
    return colors[Math.floor(Math.random() * colors.length)];
}

function initGameOfLife() {
    clearGrid();
    var canvas = document.getElementById("gameCanvas");
    if (!canvas) return;
    var ctx = canvas.getContext("2d");
    var cellSizeControl = 12;
    var cols, rows, centerX, centerY, maxDistance;
    var grid;
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        window.cellSize = Math.floor(Math.min(window.innerWidth, window.innerHeight) / cellSizeControl);
        centerX = 50;
        centerY = -400;
        maxDistance = Math.sqrt(centerX * centerX + centerY * centerY);
        // Sample the corners of the canvas
        var topLeft = fromIso(0, 0, centerX, centerY, window.cellSize);
        var topRight = fromIso(canvas.width, 0, centerX, centerY, window.cellSize);
        var bottomLeft = fromIso(0, canvas.height, centerX, centerY, window.cellSize);
        var bottomRight = fromIso(canvas.width, canvas.height, centerX, centerY, window.cellSize);
        // Find the minimum and maximum grid coordinates
        var minX = Math.floor(Math.min(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x));
        var maxX = Math.ceil(Math.max(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x));
        var minY = Math.floor(Math.min(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y));
        var maxY = Math.ceil(Math.max(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y));
        // Define the number of columns and rows based on the sampled corners
        cols = maxX - minX + 1;
        rows = maxY - minY + 1;
        window.gridMinX = minX;
        window.gridMinY = minY;
        // Create the grid array based on the calculated number of columns and rows
        grid = new Array(cols).fill(null).map(() => new Array(rows).fill(null).map(() => {
            return { alive: false, color: getRandomColor(), opacity: 1.0 };
        }));
        drawGridLines(ctx, canvas, rows, cols, centerX, centerY, window.cellSize)
    }
    window.onresize = resizeCanvas;
    resizeCanvas();
    function drawIsoSquare(ctx, gridX, gridY, centerX, centerY, cellSize) {
        var start = toIso(gridX, gridY, centerX, centerY, cellSize);
        var points = [
            start,
            toIso(gridX + 1, gridY, centerX, centerY, cellSize),
            toIso(gridX + 1, gridY + 1, centerX, centerY, cellSize),
            toIso(gridX, gridY + 1, centerX, centerY, cellSize)
        ];
        var cell = grid[gridX][gridY];
        var color = cell.color;
        var alpha = cell.opacity;
        var rgba = color.replace(/\d+\.?\d*\)$/g, `${alpha})`);
        ctx.fillStyle = rgba;
        ctx.shadowBlur = 10;
        ctx.shadowOffsetX = 5;
        ctx.shadowOffsetY = 5;
        ctx.strokeStyle = "rgba(0,0,0,0.5)";
        ctx.lineWidth = 1;
        ctx.shadowColor = "rgba(0,0,0,0.5)";
        ctx.shadowBlur = 10;
        ctx.shadowOffsetX = 5;
        ctx.shadowOffsetY = 5;
        ctx.beginPath();
        ctx.moveTo(points[0].isoX, points[0].isoY);
        for (var i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].isoX, points[i].isoY);
        }
        ctx.closePath();
        ctx.fill();
        ctx.stroke();
        ctx.shadowColor = "transparent";
    }
    canvas.addEventListener("mousemove", function (e) {
        var rect = canvas.getBoundingClientRect();
        var mouseX = e.clientX - rect.left;
        var mouseY = e.clientY - rect.top;
        var gridCoords = fromIso(mouseX, mouseY, centerX, centerY, cellSize);
        var gridX = Math.floor(gridCoords.x);
        var gridY = Math.floor(gridCoords.y);
        if (gridX >= 0 && gridX < cols && gridY >= 0 && gridY < rows) {
            if (grid[gridX][gridY].alive) return;
            grid[gridX][gridY].alive = !grid[gridX][gridY].alive;
            drawIsoSquare(ctx, gridX, gridY, centerX, centerY, cellSize);
        }
    });
    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        drawGridLines(ctx, canvas, rows, cols, centerX, centerY, cellSize)
        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                if (grid[x][y].alive) {
                    drawIsoSquare(ctx, x, y, centerX, centerY, cellSize);
                }
            }
        }
        ctx.globalAlpha = 1;
    }
    window.updateGrid = function () {
        var newGrid = new Array(cols).fill(null).map(() => new Array(rows).fill(null));
        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                var cell = grid[x][y];
                var aliveNeighbors = 0;
                for (let i = -1; i <= 1; i++) {
                    for (let j = -1; j <= 1; j++) {
                        if (i === 0 && j === 0) continue;
                        var nx = x + i;
                        var ny = y + j;
                        if (nx < 0 || nx >= grid.length || ny < 0 || ny >= grid[x].length) continue;
                        if (grid[nx][ny].alive) aliveNeighbors++;
                    }
                }
                newGrid[x][y] = {
                    color: cell.color,
                    alive: (cell.alive && (aliveNeighbors === 2 || aliveNeighbors === 3)) ||
                        (!cell.alive && aliveNeighbors === 3),
                    opacity: cell.alive ? Math.max(0, cell.opacity - 0.4) : 1.0
                };
            }
        }
        grid = newGrid;
        draw();
    };
    currentInterval = setInterval(updateGrid, 500);
    document.getElementById("gameCanvas").addEventListener("mousemove", handleMouseMove);
    draw();
}

function calculateColor(temperature) {
    // Define the temperature ranges and corresponding colors
    var colorStops = [
        { temp: 0, color: "rgba(0, 0, 255, 0.7)" },    // Blue for cold
        { temp: 20, color: "rgba(0, 255, 0, 0.7)" },   // Green for mild
        { temp: 40, color: "rgba(255, 165, 0, 0.7)" }, // Orange for warm
        { temp: 60, color: "rgba(255, 0, 0, 0.7)" }    // Red for hot
    ];

    // Find where the current temperature fits in the range
    for (let i = 0; i < colorStops.length - 1; i++) {
        var low = colorStops[i];
        var high = colorStops[i + 1];
        if (temperature >= low.temp && temperature <= high.temp) {
            var ratio = (temperature - low.temp) / (high.temp - low.temp);
            return interpolateColor(low.color, high.color, ratio);
        }
    }

    // Return the color for the maximum temperature if it exceeds the range
    return colorStops[colorStops.length - 1].color;
}

function interpolateColor(color1, color2, ratio) {
    var color1RGB = color1.match(/\d+/g).map(Number);
    var color2RGB = color2.match(/\d+/g).map(Number);
    var interpolatedRGB = color1RGB.map((c, i) => {
        return Math.floor(c + (color2RGB[i] - c) * ratio);
    });
    return `rgba(${interpolatedRGB[0]}, ${interpolatedRGB[1]}, ${interpolatedRGB[2]}, 0.7)`;
}

function initHeatEquationSolver() {
    var canvas = document.getElementById("gameCanvas");
    if (!canvas) return;
    var ctx = canvas.getContext("2d");
    var cellSizeControl = 12;
    var cols, rows, centerX, centerY;
    var grid;

    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        window.cellSize = Math.floor(Math.min(window.innerWidth, window.innerHeight) / cellSizeControl);
        centerX = 50;
        centerY = -400;
        cols = Math.floor(canvas.width / cellSize);
        rows = Math.floor(canvas.height / cellSize);
        grid = new Array(cols).fill().map(() => new Array(rows).fill().map(() => ({
            temp: 20,
            color: calculateColor(this.temp)
        })));
    }

    window.onresize = resizeCanvas;
    resizeCanvas();

    window.updateGrid = function () {
        var newGrid = grid.map(row => row.slice());
        for (let x = 1; x < cols - 1; x++) {
            for (let y = 1; y < rows - 1; y++) {
                newGrid[x][y].temp = 0.25 * (grid[x + 1][y].temp + grid[x - 1][y].temp + grid[x][y + 1].temp + grid[x][y - 1].temp);
                newGrid[x][y].color = calculateColor(newGrid[x][y].temp);
            }
        }
        grid = newGrid;
        draw();
    }

    function drawIsoSquare(ctx, gridX, gridY, centerX, centerY, cellSize) {
        var start = toIso(gridX, gridY, centerX, centerY, cellSize);
        var points = [
            start,
            toIso(gridX + 1, gridY, centerX, centerY, cellSize),
            toIso(gridX + 1, gridY + 1, centerX, centerY, cellSize),
            toIso(gridX, gridY + 1, centerX, centerY, cellSize)
        ];
        var cell = grid[gridX][gridY];
        var color = cell.color;
        var alpha = cell.opacity;
        var rgba = color.replace(/\d+\.?\d*\)$/g, `${alpha})`);
        ctx.fillStyle = rgba;
        ctx.shadowBlur = 10;
        ctx.shadowOffsetX = 5;
        ctx.shadowOffsetY = 5;
        ctx.strokeStyle = "rgba(0,0,0,0.5)";
        ctx.lineWidth = 1;
        ctx.shadowColor = "rgba(0,0,0,0.5)";
        ctx.shadowBlur = 10;
        ctx.shadowOffsetX = 5;
        ctx.shadowOffsetY = 5;
        ctx.beginPath();
        ctx.moveTo(points[0].isoX, points[0].isoY);
        for (var i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].isoX, points[i].isoY);
        }
        ctx.closePath();
        ctx.fill();
        ctx.stroke();
        ctx.shadowColor = "transparent";
    }

    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        for (let x = 0; x < cols; x++) {
            for (let y = 0; y < rows; y++) {
                drawIsoSquare(ctx, x, y, centerX, centerY, cellSize);
            }
        }
    }

    function handleMouseMove(e) {
        var rect = canvas.getBoundingClientRect();
        var mouseX = e.clientX - rect.left;
        var mouseY = e.clientY - rect.top;
        var gridX = Math.floor(mouseX / cellSize);
        var gridY = Math.floor(mouseY / cellSize);
        if (gridX >= 0 && gridX < cols && gridY >= 0 && gridY < rows) {
            grid[gridX][gridY].temp += 5;
            grid[gridX][gridY].color = calculateColor(grid[gridX][gridY].temp);
        }
    }

    currentInterval = setInterval(updateGrid, 100);
    document.getElementById("gameCanvas").addEventListener("mousemove", handleMouseMove);
    draw();
}