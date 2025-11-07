document.getElementById("loginForm").addEventListener("submit", function (e) {
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const errorBox = document.getElementById("clientError");

    let errors = [];

    if (!email) {
        errors.push("Email không được để trống.");
    } else if (!/^\S+@\S+\.\S+$/.test(email)) {
        errors.push("Email không đúng định dạng.");
    }

    if (!password) {
        errors.push("Mật khẩu không được để trống.");
    } else if (password.length < 6) {
        errors.push("Mật khẩu phải có ít nhất 6 ký tự.");
    }

    if (errors.length > 0) {
        e.preventDefault();
        errorBox.innerHTML = errors.map(err => `<div>${err}</div>`).join("");
    }
});
