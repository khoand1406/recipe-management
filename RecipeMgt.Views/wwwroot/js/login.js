// ── Password toggle ──────────────────────────────
const toggleBtn = document.getElementById('togglePassword');
const pwdInput = document.getElementById('password');
const eyeIcon = document.getElementById('eyeIcon');

toggleBtn.addEventListener('click', () => {
    const isText = pwdInput.type === 'text';
    pwdInput.type = isText ? 'password' : 'text';
    eyeIcon.className = isText ? 'bi bi-eye' : 'bi bi-eye-slash';
});

// ── Clear error on input ─────────────────────────
document.getElementById('email').addEventListener('input', () => clearFieldError('email'));
document.getElementById('password').addEventListener('input', () => clearFieldError('password'));

function clearFieldError(field) {
    const el = document.getElementById(field);
    el.classList.remove('is-error');
    document.getElementById(field + 'Error').style.display = 'none';
}

function showFieldError(field, msg) {
    const el = document.getElementById(field);
    el.classList.add('is-error');
    const errEl = document.getElementById(field + 'Error');
    errEl.querySelector('span').textContent = msg;
    errEl.style.display = 'flex';
}

// ── Form validation & loading state ─────────────
document.getElementById('loginForm').addEventListener('submit', function (e) {
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const clientErr = document.getElementById('clientError');
    let valid = true;

    // Hide general error
    clientErr.style.display = 'none';

    // Validate email
    if (!email.value.trim()) {
        showFieldError('email', 'Vui lòng nhập địa chỉ email.');
        valid = false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
        showFieldError('email', 'Địa chỉ email không hợp lệ.');
        valid = false;
    }

    // Validate password
    if (!password.value) {
        showFieldError('password', 'Vui lòng nhập mật khẩu.');
        valid = false;
    } else if (password.value.length < 6) {
        showFieldError('password', 'Mật khẩu phải có ít nhất 6 ký tự.');
        valid = false;
    }

    if (!valid) {
        e.preventDefault();
        // Shake the card
        document.querySelector('.auth-form-wrap').classList.remove('shake');
        void document.querySelector('.auth-form-wrap').offsetWidth; // reflow
        document.querySelector('.auth-form-wrap').classList.add('shake');
        return;
    }

    // Loading state
    const btn = document.getElementById('btnSubmit');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner"></span> Đang đăng nhập...';
});