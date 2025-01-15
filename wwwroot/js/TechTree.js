// Grid Configuration
const grid = document.getElementById('grid');
const coords = document.getElementById('coordinates');
let isDragging = false;
let currentX;
let currentY;
let initialX;
let initialY;
let xOffset = 0;
let yOffset = 0;

// Initialize grid position
function initializeGrid() {
    xOffset = -1000;
    yOffset = -500;
    setTranslate(xOffset, yOffset, grid);
}

// Event Listeners
function setupEventListeners() {
    grid.addEventListener('mousedown', dragStart);
    document.addEventListener('mousemove', drag);
    document.addEventListener('mouseup', dragEnd);

    document.querySelectorAll('.tech-node').forEach(node => {
        node.addEventListener('click', () => {
            if (node.classList.contains('available')) {
                node.classList.toggle('active');
            }
        });
    });
}

// Drag Handlers
function dragStart(e) {
    initialX = e.clientX - xOffset;
    initialY = e.clientY - yOffset;
    if (e.target === grid) {
        isDragging = true;
    }
}

function drag(e) {
    if (!isDragging) return;

    e.preventDefault();
    currentX = e.clientX - initialX;
    currentY = e.clientY - initialY;

    // Get container boundaries
    const container = document.querySelector('.grid-container');
    const containerWidth = container.offsetWidth;
    const containerHeight = container.offsetHeight;

    // Calculate movement limits
    const minX = containerWidth - 3000;
    const minY = containerHeight - 2000;

    // Constrain movement
    currentX = Math.min(0, Math.max(minX, currentX));
    currentY = Math.min(0, Math.max(minY, currentY));

    xOffset = currentX;
    yOffset = currentY;

    setTranslate(currentX, currentY, grid);
    updateCoordinates(currentX, currentY);
}

function dragEnd() {
    initialX = currentX;
    initialY = currentY;
    isDragging = false;
}

// Helper Functions
function setTranslate(xPos, yPos, el) {
    el.style.transform = `translate(${xPos}px, ${yPos}px)`;
}

function updateCoordinates(x, y) {
    const gridX = Math.round(-x / 50);
    const gridY = Math.round(-y / 50);
    coords.textContent = `Position: (${gridX}, ${gridY})`;
}

// Form Submission Handler
async function submitComplete(form) {
    event.preventDefault();
    try {
        const response = await fetch(form.action, {
            method: 'POST',
            body: new FormData(form)
        });

        if (!response.ok) return false;

        const node = form.closest('.tech-node');
        const nodeId = node.dataset.nodeId;

        // Update completed node
        node.classList.remove('available');
        node.classList.add('completed');
        form.remove();

        // Update connected paths
        const connectedPaths = document.querySelectorAll(`path[data-to="${nodeId}"]`);
        connectedPaths.forEach(path => {
            path.className.baseVal = 'connection-completed';
        });

        // Update node statuses and user progress
        await Promise.all([
            updateNodeStatuses(),
            updateUserProgress()
        ]);

    } catch (error) {
        console.error('Error submitting form:', error);
    }
    return false;
}

async function switchTree(treeId) {
    try {
        const response = await fetch(`/TechTree/Index?treeId=${treeId}`);
        const html = await response.text();

        // Update the grid content
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = html;

        const newGrid = tempDiv.querySelector('.draggable-grid').innerHTML;
        document.querySelector('.draggable-grid').innerHTML = newGrid;

        // Update active button
        document.querySelectorAll('.tree-button').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`.tree-button[onclick="switchTree(${treeId})"]`).classList.add('active');

        // Reinitialize grid and event listeners
        initializeGrid();
        setupEventListeners();

        // Update node statuses for the new tree
        await updateNodeStatuses(treeId);

    } catch (error) {
        console.error('Error switching tech tree:', error);
    }
}


// Update Functions
async function updateNodeStatuses() {
    try {
        // Get current tree ID from the active tree button
        const activeTreeButton = document.querySelector('.tree-button.active');
        const treeId = activeTreeButton.onclick.toString().match(/switchTree\((\d+)\)/)[1];

        const response = await fetch(`/TechTree/GetNodeStatuses?treeId=${treeId}`);
        const nodes = await response.json();

        // Update each node and its connections
        nodes.forEach(node => {
            const nodeElement = document.querySelector(`.tech-node[data-node-id="${node.id}"]`);
            if (!nodeElement) return;

            // Update node status
            nodeElement.className = `tech-node ${node.status.toLowerCase()}`;

            // Update node form if needed
            if (node.status === 'Available') {
                updateNodeForm(nodeElement, node);
            }

            // Update connected paths
            const nodeConnections = document.querySelectorAll(`path[data-to="${node.id}"]`);
            nodeConnections.forEach(connection => {
                connection.className.baseVal = `connection-${node.status.toLowerCase()}`;
            });
        });
    } catch (error) {
        console.error('Error updating node statuses:', error);
    }
}

async function updateUserProgress() {
    try {
        const response = await fetch('/TechTree/GetUserProgress');
        const progress = await response.json();

        document.querySelector('.level-display').textContent = `Level: ${progress.level}`;
        const progressBar = document.querySelector('.progress-bar');
        const percentage = (progress.currentXP * 100) / progress.xpToNext;
        progressBar.style.width = `${percentage}%`;
        progressBar.nextElementSibling.textContent = `${progress.currentXP} / ${progress.xpToNext}`;
    } catch (error) {
        console.error('Error updating user progress:', error);
    }
}



function updateNodeForm(nodeElement, node) {
    if (node.status !== 'Available') return;

    const popup = nodeElement.querySelector('.popup');
    if (!popup) return;

    const existingForm = popup.querySelector('form');
    if (existingForm) existingForm.remove();

    const newForm = document.createElement('form');
    newForm.method = 'post';
    newForm.action = '/TechTree/CompleteNode';
    newForm.innerHTML = `
        <input type="hidden" name="id" value="${node.id}" />
        <button type="submit">Complete</button>
    `;
    newForm.onsubmit = function() { return submitComplete(this); };
    popup.appendChild(newForm);
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    initializeGrid();
    setupEventListeners();
});