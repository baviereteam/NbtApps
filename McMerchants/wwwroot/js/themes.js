const lightOnButton = document.querySelector('button#light-on');
const lightOffButton = document.querySelector('button#light-off');

const changeTheme = function (isDarkTheme) {
    if (isDarkTheme) {
        document.documentElement.dataset.theme = 'dark';
        localStorage.setItem('theme', 'dark');
        lightOffButton.classList.add('hidden');
        lightOnButton.classList.remove('hidden');
    } else {
        document.documentElement.dataset.theme = 'light';
        localStorage.setItem('theme', 'light');
        lightOnButton.classList.add('hidden');
        lightOffButton.classList.remove('hidden');
    }
}

const initThemes = function (e) {
    const currentTheme = localStorage.getItem('theme');
    if (currentTheme != null) {
        changeTheme(currentTheme === 'dark');
        return;
    }

    const mqMatcher = globalThis.matchMedia("(prefers-color-scheme: dark)");
    if (mqMatcher.matches) {
        changeTheme(true);
        return;
    }

    changeTheme(false);
}

lightOnButton.addEventListener('click', () => changeTheme(false), false);
lightOffButton.addEventListener('click', () => changeTheme(true), false);

initThemes();