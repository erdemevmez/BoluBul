(function () {
    const storageKey = "bolubul-theme";
    const root = document.documentElement;

    function getStoredTheme() {
        const theme = localStorage.getItem(storageKey);
        return theme === "dark" || theme === "light" ? theme : "light";
    }

    function applyTheme(theme) {
        root.setAttribute("data-theme", theme);
        localStorage.setItem(storageKey, theme);

        document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
            const icon = button.querySelector("[data-theme-icon]");
            const text = button.querySelector("[data-theme-text]");

            if (icon) {
                icon.textContent = theme === "dark" ? "☀" : "☾";
            }

            if (text) {
                text.textContent = theme === "dark" ? "Açık" : "Koyu";
            }

            button.setAttribute("aria-label", theme === "dark" ? "Açık temaya geç" : "Koyu temaya geç");
            button.setAttribute("title", theme === "dark" ? "Açık tema" : "Koyu tema");
        });
    }

    applyTheme(getStoredTheme());

    document.addEventListener("DOMContentLoaded", function () {
        applyTheme(getStoredTheme());

        document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
            button.addEventListener("click", function () {
                const nextTheme = root.getAttribute("data-theme") === "dark" ? "light" : "dark";
                applyTheme(nextTheme);
            });
        });
    });
})();
