export function initHeatmap(allGridsData, allMetricsData) {
    if (!allGridsData || allGridsData.length === 0) return;

    const canvas = document.getElementById('heatmapCanvas');
    const ctx = canvas.getContext('2d');
    const slider = document.getElementById('chunkSlider');
    const playPauseBtn = document.getElementById('playPauseBtn');
    const chunkIndicator = document.getElementById('chunkIndicator');
    const peakPressureEl = document.getElementById('peakPressure');
    const contactAreaEl = document.getElementById('contactArea');

    const cellSize = 10;
    let isPlaying = false;
    let currentChunkIndex = 0;
    let playInterval;

    function drawGrid(chunkIndex) {
        if (chunkIndex >= allGridsData.length) return;

        const gridData = allGridsData[chunkIndex];
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        for (let row = 0; row < gridData.length; row++) {
            for (let col = 0; col < gridData[row].length; col++) {
                const value = gridData[row][col];
                ctx.fillStyle = getColourForValue(value);
                ctx.fillRect(col * cellSize, row * cellSize, cellSize, cellSize);
            }
        }

        updateUI(chunkIndex);

        const metrics = allMetricsData[chunkIndex];
        if (metrics) {
            peakPressureEl.textContent = metrics.PeakPressure.toFixed(2);
            contactAreaEl.textContent = metrics.ContactArea.toFixed(2);
        }
    }

    function getColourForValue(value) {
        if (value === 0) return "#f0f0f0";
        const hue = 240 - (value / 255) * 240;
        return `hsl(${hue}, 100%, 50%)`;
    }

    function updateUI(chunkIndex) {
        slider.value = chunkIndex;
        chunkIndicator.textContent = `Chunk ${chunkIndex + 1} / ${allGridsData.length}`;
    }

    function playNextChunk() {
        currentChunkIndex++;
        if (currentChunkIndex >= allGridsData.length) {
            currentChunkIndex = 0;
        }
        drawGrid(currentChunkIndex);
    }

    function startPlayback() {
        isPlaying = true;
        playPauseBtn.textContent = "Pause";
        playInterval = setInterval(playNextChunk, 100);
    }

    function stopPlayback() {
        isPlaying = false;
        playPauseBtn.textContent = "Play";
        clearInterval(playInterval);
    }

    playPauseBtn.addEventListener('click', () => {
        if (isPlaying) {
            stopPlayback();
        } else {
            startPlayback();
        }
    });

    slider.addEventListener('input', (e) => {
        if (isPlaying) stopPlayback();
        currentChunkIndex = parseInt(e.target.value, 10);
        drawGrid(currentChunkIndex);
    });

    drawGrid(0);
}
