const menuButton = document.getElementById('main-menu-button');
const spinner = document.getElementById('spinner');
const alertBanner = document.getElementById('alert-banner');
const infoBanner = document.getElementById('info-banner');

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

const initializeMenu = () => {
    menuButton.addEventListener('click', () => toggleMenu(), false);
}

const setSpinnerDisplayed = displayed => {
    spinner.style.display = (displayed ? 'block' : 'none');
};

const showAlert = (message) => {
    alertBanner.textContent = message;
    alertBanner.classList.remove('hidden');
}

const setInfoNodes = (node1, node2) => {
    infoBanner.textContent = '';
    // infoBanner.append(nodes[]) does not work, converts the non-text nodes to text ([object HTMLElement]) ??
    infoBanner.append(node1, node2);
    infoBanner.classList.remove('hidden');
}