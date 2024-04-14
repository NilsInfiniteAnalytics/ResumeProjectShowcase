function getWindowSize() {
    window.DotNet.invokeMethodAsync("WebApp",
        "UpdateWindowSize",
        `${window.innerWidth} x ${window.innerHeight}`);
}

function getClientTimezoneOffset() {
    var offset = new Date().getTimezoneOffset();
    DotNet.invokeMethodAsync("WebApp", "SetTimezoneOffset", offset);
}

function triggerResize() {
    window.dispatchEvent(new Event("resize"));
}

window.addEventListener("resize", getWindowSize);

function initGameOfLife() {
    var canvas = document.getElementById("gameCanvas");
    if (!canvas) return;

    canvas.width = window.innerWidth * 0.96;
    canvas.height = window.innerHeight * 0.9;

    var ctx = canvas.getContext("2d");
    var cellSize = Math.floor(canvas.width / 50);
    var cols = Math.floor(canvas.width / cellSize);
    var rows = Math.floor(canvas.height / cellSize / 2);
    var centerX = canvas.width / 2;
    var centerY = rows * cellSize / 2;
    var maxDistance = Math.sqrt(centerX * centerX + centerY * centerY) / 1.2;

    function toIso(x, y) {
        return {
            isoX: centerX + (x - y) * cellSize * 0.5,
            isoY: centerY + (x + y) * cellSize * 0.25
        };
    }

    function fromIso(isoX, isoY) {
        var x = (isoX - centerX) / (cellSize * 0.5) + (isoY - centerY) / (cellSize * 0.25);
        var y = (isoY - centerY) / (cellSize * 0.25) - (isoX - centerX) / (cellSize * 0.5);
        return {
            x: Math.floor((x + y) / 2) + cols,
            y: Math.floor((y - x) / 2) + rows
        };
    }

    function drawGridLines() {
        for (let i = -cols; i <= cols; i++) {
            for (let j = -10*rows; j <= 10*rows; j++) {
                var start = toIso(i, j);
                var endRight = toIso(i + 1, j);
                var endDown = toIso(i, j + 1);

                var distance = Math.sqrt(Math.pow(start.isoX - centerX, 2) + Math.pow(start.isoY - centerY, 2));
                var fade = 1 - distance / maxDistance;

                ctx.globalAlpha = fade;

                ctx.beginPath();
                ctx.moveTo(start.isoX, start.isoY);
                ctx.lineTo(endRight.isoX, endRight.isoY);
                ctx.stroke();

                ctx.beginPath();
                ctx.moveTo(start.isoX, start.isoY);
                ctx.lineTo(endDown.isoX, endDown.isoY);
                ctx.stroke();
            }
        }
        ctx.globalAlpha = 1; // Reset alpha after drawing
    }

    var grid = new Array(cols * 2).fill(null).map(() => new Array(rows * 2).fill(null));

    function getRandomColor() {
        var r = Math.floor(Math.random() * 256);
        var g = Math.floor(Math.random() * 256);
        var b = Math.floor(Math.random() * 256);
        return "rgba(" + r + ", " + g + ", " + b + ", 1)";
    }

    canvas.addEventListener("mousemove", function (e) {
        var rect = canvas.getBoundingClientRect();
        var mouseIsoX = e.clientX - rect.left;
        var mouseIsoY = e.clientY;
        var gridPos = fromIso(mouseIsoX, mouseIsoY);
        if (gridPos.x >= 0 && gridPos.x < grid.length && gridPos.y >= 0 && gridPos.y < grid[0].length) {
            if (Math.random() < 0.25) {
                grid[gridPos.x][gridPos.y] = getRandomColor();
            }
        }
        draw();
    });

    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        drawGridLines();

        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                if (grid[x][y]) {
                    var pos = toIso(x - cols, y - rows); // Adjust for center
                    var distance = Math.sqrt(Math.pow(pos.isoX - centerX, 2) + Math.pow(pos.isoY - centerY, 2));
                    var fade = 1 - distance / maxDistance;
                    ctx.globalAlpha = fade; // Set the alpha level based on distance

                    // Use the color stored in the grid cell
                    ctx.fillStyle = grid[x][y];
                    ctx.beginPath();
                    ctx.moveTo(pos.isoX, pos.isoY - cellSize / 4);
                    ctx.lineTo(pos.isoX + cellSize / 2, pos.isoY);
                    ctx.lineTo(pos.isoX, pos.isoY + cellSize / 4);
                    ctx.lineTo(pos.isoX - cellSize / 2, pos.isoY);
                    ctx.closePath();
                    ctx.fill();
                }
            }
        }
        ctx.globalAlpha = 1; // Reset alpha after drawing
    }

    window.updateGrid = function () {
        var newGrid = new Array(cols * 2).fill(false).map(() => new Array(rows * 2).fill(false));

        for (let x = 0; x < grid.length; x++) {
            for (let y = 0; y < grid[x].length; y++) {
                var aliveNeighbors = 0;
                // Check neighbors
                for (let i = -1; i <= 1; i++) {
                    for (let j = -1; j <= 1; j++) {
                        if (i === 0 && j === 0) continue; // Skip the cell itself
                        var nx = x + i;
                        var ny = y + j;
                        // Wrap around the grid boundaries
                        if (nx < 0 || nx >= grid.length || ny < 0 || ny >= grid[x].length) continue;
                        if (grid[nx][ny]) aliveNeighbors++;
                    }
                }

                // Apply the Game of Life rules
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

    setInterval(window.updateGrid, 500);
    draw();
}