var sidebar = document.querySelector(".sidebar");
//var sidebarBtn = document.querySelector(".bx-menu");
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
//function sidebarToggleHandling() {
//    if (sidebarBtn) {
//        sidebarBtn.addEventListener("click", () => {
//            var isPinned = sidebar.classList.contains("pinned");

//            if (!isPinned) {
//                var toggleState = sidebar.classList.toggle("close");
//                sidebarBtn.style.transform = toggleState ? "rotate(0deg) translateX(0px)" : "rotate(180deg) translateX(5px)";
//            }
//        });
//    }
//}




//sidebar toggle handling on hover 
function sidebarHandlingOnHover() {
    if (sidebar) {
        sidebar.addEventListener("mouseenter", function () {
            //sidebar.classList.remove("close");
            //sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
            if (!sidebar.classList.contains("liActive")) {
                sidebar.classList.remove("close");
            }
        })
    }


    if (sidebar) {
        sidebar.addEventListener("mouseleave", function () {
            var isPinned = sidebar.classList.contains("pinned");

            let menuState = JSON.parse(localStorage.getItem('menuState'));

            //if any parent menu is open the sidebar won't close 
            if (menuState) {
                return;
            }

            if (!isPinned && !sidebar.classList.contains("liActive")) {
                sidebar.classList.add("close");
                //sidebarBtn.style.transform = "rotate(0deg) translateX(0px)";
            } else {
                sidebar.classList.remove("close");
                //sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
            }
        })

    }


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

    //if (sidebarBtn) {
    //    sidebarBtn.style.transform = isPinned ? "rotate(180deg) translateX(5px)" : "rotate(0deg) translateX(0px)";
    //}
    
}

function initializeMenuState() {
    //check each menu item and update local storage based on their state
    let anyMenuOpen = false;
    menuItems.forEach(menuItem => {
        if (menuItem.classList.contains("showMenu")) {
            anyMenuOpen = true;
        }
    });
    localStorage.setItem('menuState', anyMenuOpen ? 'true' : 'false');
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

            //const menuId = menuItem.getAttribute('data-menu-id');
            //const isOpen = menuItem.classList.contains("showMenu") ? 'true' : 'false';

            localStorage.setItem('menuState', menuItem.classList.contains("showMenu") ? 'true' : 'false');

            //localStorage.setItem(`menuState_${menuId}`, isOpen);


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

                //const subMenuId = subMenuItem.getAttribute('data-submenu-id');
                //const isOpen = subMenuItem.classList.contains("showChildMenu") ? 'true' : 'false';
                //localStorage.setItem(`submenuState_${subMenuId}`, isOpen);

            });
        }
    });

}

function activeSubMenu() {
    var currentUrl = new URL(window.location.href);

    var currentSearcAttr = currentUrl.search;
    

    var submenuLinks = document.querySelectorAll('.sub-menu li .subMenuLink');

    submenuLinks.forEach(item => {
        var itemUrl = new URL(item.href);
        var itemSearcAttr = itemUrl.search;

        if (currentSearcAttr === itemSearcAttr) {
            var parentLi = item.closest('li');
            parentLi.classList.add('activeLi');

            var toactiveLi = parentLi.closest('ul').closest('li');
            toactiveLi.classList.add('showMenu')

            if (sidebar.classList.contains("close")) {
                sidebar.classList.remove("close");
                //sidebar.classList.add("pinned");
                sidebar.classList.add("liActive");
            }
        }
           
    })
}

function activeChildMenu() {
    var currentUrl = window.location.href;
    var childSubmenuLinks = document.querySelectorAll('.childSubMenu li a');
    childSubmenuLinks.forEach(item => {
        if (currentUrl === item.href) {
            var parentLi = item.closest('li');
            parentLi.classList.add('activeLi');

            var showChildMenuLi = parentLi.closest('ul').closest('li');
            showChildMenuLi.classList.add('showChildMenu')

            var showMainMenuLi = showChildMenuLi.closest('ul').closest('li');
            showMainMenuLi.classList.add('showMenu')

            if (sidebar.classList.contains("close")) {
                sidebar.classList.remove("close");
                //sidebar.classList.add("pinned");
                sidebar.classList.add("liActive");
            }
        }
    }
    )
}

initializeSidebarState();
initializeMenuState();
sidebarPinHandling();
//sidebarToggleHandling();
sidebarHandlingOnHover();
menuToggleHandling();
subMenuToggleHandling();
activeSubMenu();
activeChildMenu();



//reset state of datagrid
function onStateResetClick() {
    const dataGrid = $("#dataGridContainer").dxDataGrid("instance");
    dataGrid.state(null);
}

