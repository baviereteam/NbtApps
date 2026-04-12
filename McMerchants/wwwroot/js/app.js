const menuButton = document.getElementById('main-menu-button');

const setMenuOpened = (open) => {
    if (open) {
        document.body.classList.add('menu-opened');
        menuButton.classList.add('menu-opened');
    } else {
        document.body.classList.remove('menu-opened');
        menuButton.classList.remove('menu-opened');
    }
}

const toggleMenu = () => {
    setMenuOpened(!document.body.classList.contains('menu-opened'))
}

// init
const initializeMenu = () => {
    menuButton.addEventListener('click', () => toggleMenu(), false);
}