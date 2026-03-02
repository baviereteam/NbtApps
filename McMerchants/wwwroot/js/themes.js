const lightSwitch = document.querySelector('input[name="light-switch"]');

const changeTheme = function (isDarkTheme) {
    if (isDarkTheme) {
        document.documentElement.dataset.theme = 'dark';
        localStorage.setItem('theme', 'dark');
        lightSwitch.checked = true;
    } else {
        document.documentElement.dataset.theme = 'light';
        localStorage.setItem('theme', 'light');
        lightSwitch.checked = false;
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

const onLightSwitchChange = function (e) {
    changeTheme(e.target.checked);
}

lightSwitch.addEventListener('change', onLightSwitchChange, false);
initThemes();