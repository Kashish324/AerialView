var dashboardControl; // Declare globally
var refreshIntervalId;


function onBeforeRender() {
    // Initialize or capture the dashboard control
    dashboardControl = clientDashboardControl;
    var refreshRate = document.getElementById('dashboard-wrapper').getAttribute('data-refresh-rate');

    if (isNaN(refreshRate) || refreshRate <= 0) {
        console.log("Auto-refresh disabled (refreshRate is 0 or invalid).");
        return; // Exit early
    }

    if (refreshRate < 1000) {
        refreshRate *= 1000;
    }

    if (refreshIntervalId) {
        clearInterval(refreshIntervalId)
    }

    refreshIntervalId = setInterval(() => {
        if (dashboardControl) {
            dashboardControl.reloadData();
            console.log("Dashboard refreshed");
        }
    }, refreshRate);
}