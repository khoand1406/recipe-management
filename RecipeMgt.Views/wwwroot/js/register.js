
document.getElementById("registerForm").addEventListener("submit", function (e) {
    const username = document.getElementById("fullName").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const errorBox = document.getElementById("clientError");

    let errors = [];

    // Validate Username
    if (!username) {
        errors.push("Username is required.");
    } else if (username.length < 3) {
        errors.push("Username must be at least 3 characters.");
    }

    // Validate Email
    if (!email) {
        errors.push("Email is required.");
    } else if (!/^\S+@\S+\.\S+$/.test(email)) {
        errors.push("Invalid email format.");
    }

    // Validate Password
    if (!password) {
        errors.push("Password is required.");
    } else if (password.length < 6) {
        errors.push("Password must be at least 6 characters.");
    }

    // Show errors if any
    if (errors.length > 0) {
        e.preventDefault();
        errorBox.innerHTML = errors.map(err => `<div>${err}</div>`).join("");
    }
});
