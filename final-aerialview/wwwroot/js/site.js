
function sidebarToggleHandling() {

let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");

sidebarBtn.addEventListener("click", () => {
    var toggleState = sidebar.classList.toggle("close");

    if (!toggleState) {
        sidebarBtn.style.transform = "rotate(180deg) translateX(5px)";
    } else {
        sidebarBtn.style.transform = "rotate(0deg) translateX(0px)";
    }
});
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



