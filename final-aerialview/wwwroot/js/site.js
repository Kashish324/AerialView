
function sidebarToggleHandling() {

    let sidebar = document.querySelector(".sidebar");
    let sidebarBtn = document.querySelector(".bx-menu");

    //const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarBtn) {

        sidebarBtn.addEventListener("click", () => {
            var toggleState = sidebar.classList.toggle("close");

            if (!toggleState) {
                sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
            } else {
                sidebarBtn.style.transform = "rotate(0deg) translateX(0px)";
            }
        });
    }

}

function menuToggleHandling() {
    let menuItems = document.querySelectorAll('.nav-links > li');

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
    let subMenuItems = document.querySelectorAll('.sub-menu > .subMenu-link');
    let childMenuItems = document.querySelectorAll('.childSubMenu .icon-link');

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

sidebarToggleHandling();
menuToggleHandling();
subMenuToggleHandling();

//reset state persistence
function onStateResetClick() {
    const dataGrid = $("#dataGridContainer").dxDataGrid("instance");
    dataGrid.state(null);
}



//animating login page
document.addEventListener("DOMContentLoaded", function () {
    gsap.from(".login-wrapper", { duration: 1, opacity: 0, y: -50, ease: "power2.out" });

    gsap.from(".logo-img", { duration: 1, opacity: 0, x: -100, scale: 0.5, ease: "power2.out" });

    gsap.from(".img-part img", { duration: 1, opacity: 0, x: 100, ease: "power2.out" });

    gsap.from(".login-font", { duration: 1, opacity: 0, x: -100, ease: "power2.out" });

    gsap.from(".form-floating", { duration: 1, opacity: 0, x: -100, stagger: 0.2, ease: "power2.out" });

    gsap.from("#login-submit", { duration: 1, opacity: 0, x: -100, ease: "power2.out", delay: .2 });

    gsap.from(".copyright-text", { duration: 1, opacity: 0, x: -100, ease: "power2.out", delay: .2 });



    //gsap.from(".login-font", { duration: .5, opacity: 0, x: -100, delay: 1, ease: "power2.out" });
    //gsap.from(".form-floating", { duration: .6, opacity: 0, x: 20, stagger: 0.2, delay: 1.5 });
    //gsap.from("#login-submit", { duration: .6, opacity: 0, scale: 0.8, delay: 2 });

    //gsap.from(".copyright-text", { duration: .5, opacity: 0, delay: 2, ease: "easeInOut" })
});

