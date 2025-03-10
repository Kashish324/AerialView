﻿class DeleteDashboardExtension {
    toolbox;
    menuItem;
    dashboardControl;
    name = "dxdde-delete-dashboard";

    constructor(dashboardControl) {
        this.dashboardControl = dashboardControl;
        this.menuItem = {
            id: this.name,
            title: "Delete",
            click: this.deleteDashboard.bind(this),
            selected: ko.observable(false),
            disabled: ko.computed(function () { return !this.dashboardControl.dashboard(); }, this),
            index: 113,
            hasSeparator: true,
            data: this
        };
    }

    deleteDashboard() {
        if (this.toolbox) {
            var dashboardTitle = this.dashboardControl.dashboardContainer().dashboard.title.text();
            console.log(dashboardTitle);
            if (confirm(`Are you sure you want to delete the "${dashboardTitle}" dashboard?`)) {
                var dashboardid = this.dashboardControl.dashboardContainer().id;
               

                
                this.toolbox.menuVisible(false);

                $.ajax({
                    //url: 'Home/DeleteDashboard',
                    url: '/Submenu/DeleteDashboard',

                    data: { DashboardID: dashboardid },
                    type: 'POST',
                    success: (function () { this.dashboardControl.unloadDashboard(); }).bind(this)
                });
            }
        }
    }

    start() {
        this.toolbox = this.dashboardControl.findExtension("toolbox");
        this.toolbox && this.toolbox.menuItems.push(this.menuItem);
    }

    stop() {
        this.toolbox && this.toolbox.menuItems.remove(this.menuItem);
    }
}