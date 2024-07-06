var sidebar = document.querySelector(".sidebar");
var sidebarBtn = document.querySelector(".bx-menu");
var sidebarPin = document.getElementById("sidebarPin");
var menuItems = document.querySelectorAll('.nav-links > li');
var subMenuItems = document.querySelectorAll('.sub-menu > .subMenu-link');
var childMenuItems = document.querySelectorAll('.childSubMenu .icon-link');

// Function to handle sidebar pinning
function sidebarPinHandling() {
    if (sidebar && sidebarPin) {
        sidebarPin.addEventListener("click", function () {
            var isPinned = sidebar.classList.contains("pinned");

            sidebar.classList.toggle("pinned");

            if (!isPinned) {
                sidebarPin.classList.remove("ri-pushpin-line");
                sidebarPin.classList.add("ri-pushpin-fill");
                sidebarPin.setAttribute("title", "Unpin Sidebar");
            } else {
                sidebarPin.classList.remove("ri-pushpin-fill");
                sidebarPin.classList.add("ri-pushpin-line");
                sidebarPin.setAttribute("title", "Pin Sidebar");
            }

            // Update local storage state
            localStorage.setItem('sidebarPinned', sidebar.classList.contains("pinned") ? 'true' : 'false');
            localStorage.setItem('pinIconState', sidebarPin.classList.contains('ri-pushpin-fill') ? 'fill' : 'line');
        });
    }
}

// Function to handle sidebar toggle
function sidebarToggleHandling() {
    if (sidebarBtn) {
        sidebarBtn.addEventListener("click", () => {
            var isPinned = sidebar.classList.contains("pinned");

            if (!isPinned) {
                var toggleState = sidebar.classList.toggle("close");
                sidebarBtn.style.transform = toggleState ? "rotate(0deg) translateX(0px)" : "rotate(180deg) translateX(5px)";
            }
        });
    }
}

if (sidebar) {
    sidebar.addEventListener("mouseenter", function () {
        sidebar.classList.remove("close");
        sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
})

}


if (sidebar) {
    var isPinned = sidebar.classList.contains("pinned");
    sidebar.addEventListener("mouseleave", function () {
        var isPinned = sidebar.classList.contains("pinned");

        if (!isPinned) {
            sidebar.classList.add("close");
            sidebarBtn.style.transform = "rotate(0deg) translateX(0px)";
        } else {
            sidebar.classList.remove("close");
            sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
        }
    })
   
}



function initializeSidebarState() {
    var isPinned = localStorage.getItem('sidebarPinned') === 'true';
    var pinIconState = localStorage.getItem('pinIconState');

    if (sidebar) {
        sidebar.classList.toggle("pinned", isPinned);
        sidebar.classList.toggle("close", !isPinned);
    }

    if (sidebarPin) {
        if (pinIconState === 'fill') {
            sidebarPin.classList.remove("ri-pushpin-line");
            sidebarPin.classList.add("ri-pushpin-fill");
            sidebarPin.setAttribute("title", "Unpin Sidebar");
        } else {
            sidebarPin.classList.remove("ri-pushpin-fill");
            sidebarPin.classList.add("ri-pushpin-line");
            sidebarPin.setAttribute("title", "Pin Sidebar");
        }
    }

    if (sidebarBtn) {
        sidebarBtn.style.transform = isPinned ? "rotate(180deg) translateX(5px)" : "rotate(0deg) translateX(0px)";
    }
}



function menuToggleHandling() {

    function toggleMenu(element) {
        let parentLi = element;
        parentLi.classList.toggle("showMenu");
    }


    menuItems.forEach(menuItem => {
        menuItem.addEventListener('click', function (e) {
            if (menuItem.querySelector('.sub-menu')) {
                toggleMenu(menuItem);
            }
        });
    });


}

function subMenuToggleHandling() {


    childMenuItems.forEach(item => {
        item.addEventListener('click', function (e) {
            //e.preventDefault(); 
            e.stopPropagation();
        });
    });
    function toggleSubMenu(element) {
        let parentLi = element;
        parentLi.classList.toggle("showChildMenu");
    }

    subMenuItems.forEach(subMenuItem => {
        if (!subMenuItem.closest('.childSubMenu')) {
            subMenuItem.addEventListener('click', function (e) {
                if (!e.target.classList.contains('link_name')) {
                    e.stopPropagation(); // Prevent the event from bubbling up to the parent li
                }
                if (subMenuItem.querySelector('.childSubMenu')) {
                    toggleSubMenu(subMenuItem);
                }
            });
        }
    });

}

initializeSidebarState();
sidebarPinHandling();
sidebarToggleHandling();
menuToggleHandling();
subMenuToggleHandling();



//reset state persistence
function onStateResetClick() {
    const dataGrid = $("#dataGridContainer").dxDataGrid("instance");
    dataGrid.state(null);
    console.log('reset')
}