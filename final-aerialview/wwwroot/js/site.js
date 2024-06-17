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



function initializeSidebarState() {
    var isPinned = localStorage.getItem('sidebarPinned') === 'true';
    var pinIconState = localStorage.getItem('pinIconState');

    sidebar.classList.toggle("pinned", isPinned);
    sidebar.classList.toggle("close", !isPinned);

    if (pinIconState === 'fill') {
        sidebarPin.classList.remove("ri-pushpin-line");
        sidebarPin.classList.add("ri-pushpin-fill");
        sidebarPin.setAttribute("title", "Unpin Sidebar");
    } else {
        sidebarPin.classList.remove("ri-pushpin-fill");
        sidebarPin.classList.add("ri-pushpin-line");
        sidebarPin.setAttribute("title", "Pin Sidebar");
    }

    sidebarBtn.style.transform = isPinned ? "rotate(180deg) translateX(5px)" : "rotate(0deg) translateX(0px)";
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
}



//animating login page
//document.addEventListener("DOMContentLoaded", function () {
//    gsap.from(".login-wrapper", { duration: 1, opacity: 0, y: -50, ease: "power2.out" });

//    gsap.from(".logo-img", { duration: 1, opacity: 0, x: -100, scale: 0.5, ease: "power2.out" });

//    gsap.from(".img-part img", { duration: 1, opacity: 0, x: 100, ease: "power2.out" });

//    gsap.from(".login-font", { duration: 1, opacity: 0, x: -100, ease: "power2.out" });

//    gsap.from(".form-floating", { duration: 1, opacity: 0, x: -100, stagger: 0.2, ease: "power2.out" });

//    gsap.from("#login-submit", { duration: 1, opacity: 0, x: -100, ease: "power2.out", delay: .2 });

//    gsap.from(".copyright-text", { duration: 1, opacity: 0, x: -100, ease: "power2.out", delay: .2 });



//    //gsap.from(".login-font", { duration: .5, opacity: 0, x: -100, delay: 1, ease: "power2.out" });
//    //gsap.from(".form-floating", { duration: .6, opacity: 0, x: 20, stagger: 0.2, delay: 1.5 });
//    //gsap.from("#login-submit", { duration: .6, opacity: 0, scale: 0.8, delay: 2 });

//    //gsap.from(".copyright-text", { duration: .5, opacity: 0, delay: 2, ease: "easeInOut" })
//});

