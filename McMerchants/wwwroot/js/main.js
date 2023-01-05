const lightSwitch = document.querySelector('input[name="light-switch"]');

const changeTheme = function (isDarkTheme) {
    if (isDarkTheme) {
        document.documentElement.setAttribute('data-theme', 'dark');
        localStorage.setItem('theme', 'dark');
        lightSwitch.checked = true;
    } else {
        document.documentElement.setAttribute('data-theme', 'light');
        localStorage.setItem('theme', 'light');
        lightSwitch.checked = false;
    }
}

const onLoad = function (e) {
    const currentTheme = localStorage.getItem('theme');
    if (currentTheme != null) {
        changeTheme(currentTheme === 'dark');
        return;
    }

    const mqMatcher = window.matchMedia("(prefers-color-scheme: dark)");
    if (mqMatcher.matches) {
        changeTheme(true);
        return;
    }

    changeTheme(false);
}

const onLightSwitchChange = function (e) {
    changeTheme(e.target.checked);
    console.log(`checkbox checked: ${e.target.checked}`);
}

lightSwitch.addEventListener('change', onLightSwitchChange, false);
onLoad();