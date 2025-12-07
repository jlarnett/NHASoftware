function setTheme(mode) {
    document.body.dataset.bsTheme = mode;
    localStorage.setItem('theme', mode);
}

document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        document.body.dataset.bsTheme = savedTheme;
    }
    else {
        document.body.dataset.bsTheme = 'light';
    }