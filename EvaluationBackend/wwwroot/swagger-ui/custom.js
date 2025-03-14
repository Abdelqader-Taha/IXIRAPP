document.addEventListener("DOMContentLoaded", function () {
    // Create the button
    var button = document.createElement("button");
    button.id = "theme-toggle";
    button.innerText = "Toggle Theme";

    // Append button to Swagger UI
    document.body.appendChild(button);

    // Check local storage for the theme preference
    if (localStorage.getItem("swagger-theme") === "dark") {
        document.body.classList.add("dark-mode");
    }

    // Button click event
    button.addEventListener("click", function () {
        document.body.classList.toggle("dark-mode");

        // Save theme preference
        if (document.body.classList.contains("dark-mode")) {
            localStorage.setItem("swagger-theme", "dark");
        } else {
            localStorage.setItem("swagger-theme", "light");
        }
    });
});