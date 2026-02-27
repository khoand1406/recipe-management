
function makeToggle(btnId, inputId, iconId) {
    document.getElementById(btnId).addEventListener('click', () => {
        const inp = document.getElementById(inputId);
        const icon = document.getElementById(iconId);
        const show = inp.type === 'password';
        inp.type = show ? 'text' : 'password';
        icon.className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
    });
}

makeToggle('togglePassword', 'password', 'eyeIcon1');
makeToggle('toggleConfirm', 'confirmPassword', 'eyeIcon2');

// ─── Strength meter ──────────────────────────────
const pwdInput = document.getElementById('password');
const strengthWrap = document.getElementById('strengthWrap');
const bars = [1, 2, 3, 4].map(n => document.getElementById('bar' + n));
const strengthLbl = document.getElementById('strengthLabel');

const rules = {
    length: { el: document.getElementById('rule-length'), test: v => v.length >= 8 },
    upper: { el: document.getElementById('rule-upper'), test: v => /[A-Z]/.test(v) },
    number: { el: document.getElementById('rule-number'), test: v => /[0-9]/.test(v) },
    special: { el: document.getElementById('rule-special'), test: v => /[^A-Za-z0-9]/.test(v) },
};

pwdInput.addEventListener('input', () => {
    const val = pwdInput.value;
    strengthWrap.style.display = val ? 'block' : 'none';

    // Evaluate rules
    let score = 0;
    Object.values(rules).forEach(r => {
        const pass = r.test(val);
        r.el.classList.toggle('pass', pass);
        if (pass) score++;
    });

    // Update bars
    const levels = ['', 'weak', 'fair', 'fair', 'strong'];
    const labels = ['', 'Yếu', 'Trung bình', 'Trung bình', 'Mạnh'];
    bars.forEach((b, i) => {
        b.className = 'strength-bar' + (i < score ? ' ' + levels[score] : '');
    });

    strengthLbl.textContent = labels[score] || '';
    strengthLbl.className = 'strength-label ' + (levels[score] || '');

    // Re-check confirm
    if (document.getElementById('confirmPassword').value)
        checkConfirm();
});

// ─── Confirm password match ──────────────────────
function checkConfirm() {
    const confirm = document.getElementById('confirmPassword');
    const match = confirm.value === pwdInput.value && confirm.value !== '';
    const checkIcon = document.getElementById('confirmCheck');

    confirm.classList.toggle('is-valid', match);
    confirm.classList.toggle('is-error', !match && confirm.value !== '');
    checkIcon.style.opacity = match ? '1' : '0';
}

document.getElementById('confirmPassword').addEventListener('input', checkConfirm);

// ─── Inline validation helpers ────────────────────
function setValid(id) {
    const el = document.getElementById(id);
    el.classList.remove('is-error');
    el.classList.add('is-valid');
    document.getElementById(id + 'Error').style.display = 'none';
}

function setError(id, msg) {
    const el = document.getElementById(id);
    el.classList.remove('is-valid');
    el.classList.add('is-error');
    const err = document.getElementById(id + 'Error');
    err.querySelector('span').textContent = msg;
    err.style.display = 'flex';
}

function clearField(id) {
    const el = document.getElementById(id);
    el.classList.remove('is-error', 'is-valid');
    document.getElementById(id + 'Error').style.display = 'none';
}

// Live validation on blur
document.getElementById('username').addEventListener('blur', function () {
    const v = this.value.trim();
    if (!v) setError('username', 'Vui lòng nhập tên người dùng.');
    else if (v.length < 3) setError('username', 'Tên phải có ít nhất 3 ký tự.');
    else if (/\s/.test(v)) setError('username', 'Tên không được chứa khoảng trắng.');
    else setValid('username');
});

document.getElementById('email').addEventListener('blur', function () {
    const v = this.value.trim();
    if (!v) setError('email', 'Vui lòng nhập email.');
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)) setError('email', 'Email không hợp lệ.');
    else setValid('email');
});

document.getElementById('password').addEventListener('blur', function () {
    if (!this.value) setError('password', 'Vui lòng nhập mật khẩu.');
    else if (this.value.length < 8) setError('password', 'Mật khẩu phải có ít nhất 8 ký tự.');
    else clearField('password');
});

// Clear on focus
['username', 'email', 'password', 'confirmPassword'].forEach(id => {
    document.getElementById(id).addEventListener('focus', () => clearField(id));
});

// ─── Form submit validation ───────────────────────
document.getElementById('registerForm').addEventListener('submit', function (e) {
    const username = document.getElementById('username');
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const confirm = document.getElementById('confirmPassword');
    const terms = document.getElementById('agreeTerms');
    const termsErr = document.getElementById('termsError');
    let valid = true;

    // Username
    if (!username.value.trim()) {
        setError('username', 'Vui lòng nhập tên người dùng.'); valid = false;
    } else if (username.value.trim().length < 3) {
        setError('username', 'Tên phải có ít nhất 3 ký tự.'); valid = false;
    }

    // Email
    if (!email.value.trim()) {
        setError('email', 'Vui lòng nhập email.'); valid = false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
        setError('email', 'Email không hợp lệ.'); valid = false;
    }

    // Password
    if (!password.value) {
        setError('password', 'Vui lòng nhập mật khẩu.'); valid = false;
    } else if (password.value.length < 8) {
        setError('password', 'Mật khẩu phải có ít nhất 8 ký tự.'); valid = false;
    }

    // Confirm
    if (!confirm.value) {
        setError('confirmPassword', 'Vui lòng xác nhận mật khẩu.'); valid = false;
    } else if (confirm.value !== password.value) {
        setError('confirmPassword', 'Mật khẩu không khớp.'); valid = false;
    }

    // Terms
    if (!terms.checked) {
        termsErr.querySelector('span').textContent = 'Bạn phải đồng ý với điều khoản.';
        termsErr.style.display = 'flex';
        valid = false;
    } else {
        termsErr.style.display = 'none';
    }

    if (!valid) {
        e.preventDefault();
        const wrap = document.querySelector('.auth-form-wrap');
        wrap.classList.remove('shake');
        void wrap.offsetWidth;
        wrap.classList.add('shake');
        return;
    }

    // Loading
    const btn = document.getElementById('btnSubmit');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner"></span> Đang tạo tài khoản...';
});
