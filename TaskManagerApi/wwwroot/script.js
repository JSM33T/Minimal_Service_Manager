// Toast initialization
const toastElement = document.getElementById('toast');
const toast = new bootstrap.Toast(toastElement);

function showToast(message) {
    document.getElementById('toastMessage').textContent = message;
    toast.show();
}

// API endpoints
const API_BASE = '/api/service';

// Fetch and display services
async function refreshServices() {
    try {
        const response = await fetch(`${API_BASE}/list`);
        const serviceIds = await response.json();

        const servicesList = document.getElementById('servicesList');
        servicesList.innerHTML = '';

        for (const serviceId of serviceIds) {
            const statusResponse = await fetch(`${API_BASE}/status/${serviceId}`);
            const statusData = await statusResponse.json();

            const row = document.createElement('tr');
            row.innerHTML = `
                        <td>${serviceId}</td>
                        <td>
                            <span class="badge bg-${statusData.isRunning ? 'success' : 'danger'}">
                                ${statusData.isRunning ? 'Running' : 'Stopped'}
                            </span>
                        </td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="stopService('${serviceId}')">
                                Stop
                            </button>
                        </td>
                    `;
            servicesList.appendChild(row);
        }
    } catch (error) {
        showToast('Error refreshing services: ' + error.message);
    }
}

// Create new service
document.getElementById('createServiceForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const serviceId = document.getElementById('serviceId').value;

    try {
        const response = await fetch(`${API_BASE}/start/${serviceId}`, {
            method: 'POST'
        });

        if (response.ok) {
            showToast(`Service ${serviceId} started successfully`);
            document.getElementById('serviceId').value = '';
            refreshServices();
        } else {
            const error = await response.text();
            showToast(`Error starting service: ${error}`);
        }
    } catch (error) {
        showToast('Error starting service: ' + error.message);
    }
});

// Stop service
async function stopService(serviceId) {
    try {
        const response = await fetch(`${API_BASE}/stop/${serviceId}`, {
            method: 'POST'
        });

        if (response.ok) {
            showToast(`Service ${serviceId} stopped successfully`);
            refreshServices();
        } else {
            const error = await response.text();
            showToast(`Error stopping service: ${error}`);
        }
    } catch (error) {
        showToast('Error stopping service: ' + error.message);
    }
}

// Initial load
refreshServices();

// Auto-refresh every 5 seconds
setInterval(refreshServices, 5000);