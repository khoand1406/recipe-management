/* ── Check all ── */
const checkAll = document.getElementById('checkAll');
if (checkAll) {
    checkAll.addEventListener('change', () =>
        document.querySelectorAll('.row-check').forEach(cb => cb.checked = checkAll.checked)
    );
}

/* ── Search debounce ── */
let searchTimer;
const searchInput = document.getElementById('searchQuery');
if (searchInput) {
    searchInput.addEventListener('input', () => {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(() => document.getElementById('filterForm').submit(), 500);
    });
}

document.querySelector('select[name="status"]').addEventListener('change', function () {
    const form = document.getElementById('filterForm');
    form.querySelector('input[name="page"]').value = 1;
    form.submit();
});

document.querySelector('select[name="pageSize"]').addEventListener('change', function () {
    const form = document.getElementById('filterForm');
    form.querySelector('input[name="page"]').value = 1;
    form.submit();
});

/* ── Ban / Unban modal ── */
const banModal = new bootstrap.Modal(document.getElementById('banModal'));
const banForm = document.getElementById('banForm');
const banTitle = document.getElementById('banModalTitle');
const banBody = document.getElementById('banModalBody');
const banSubmit = document.getElementById('banSubmit');

window.confirmBan = (userId, name) => {
    banTitle.textContent = 'Cấm người dùng';
    banBody.innerHTML = `Bạn có chắc muốn <strong>cấm</strong> người dùng <strong>${name}</strong>?`;

    banSubmit.onclick = async () => {
        try {
            const res = await fetch(`/admin/users/${userId}/ban`, {
                method: 'PATCH'
            });

            if (!res.ok) throw new Error('Ban failed');

            location.reload();

        } catch (err) {
            alert('Có lỗi xảy ra khi cấm user');
        }
    };

    banSubmit.style.background = 'linear-gradient(135deg,#ef4444,#dc2626)';
    banSubmit.textContent = 'Cấm ngay';
    banModal.show();
}

window.confirmUnban = (userId, name) => {
    banTitle.textContent = 'Bỏ cấm người dùng';
    banBody.innerHTML = `Bạn có chắc muốn <strong>bỏ cấm</strong> người dùng <strong>${name}</strong>?`;

    banSubmit.onclick = async () => {
        try {
            const res = await fetch(`/admin/users/${userId}/unban`, {
                method: 'PATCH',
            });

            if (!res.ok) throw new Error('Unban failed');

            location.reload();

        } catch (err) {
            alert('Có lỗi xảy ra khi bỏ cấm user');
        }
    };

    banSubmit.style.background = 'linear-gradient(135deg,#22c55e,#16a34a)';
    banSubmit.textContent = 'Bỏ cấm';
    banModal.show();
};


/* ── Export stub ── */
window.exportUsers = () => {
    const params = new URLSearchParams(window.location.search);
    params.set('export', 'csv');
    window.location.href = `/admin/users/export?${params.toString()}`;
};

const createModal = new bootstrap.Modal(document.getElementById('createModal'));
const editModal = new bootstrap.Modal(document.getElementById('editModal'));
const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));

function setLoading(btnId, on) {
    const btn = document.getElementById(btnId);
    btn.classList.toggle('btn-loading', on);
    btn.disabled = on;
}
function showAlert(alertId, msg, type = 'error') {
    const el = document.getElementById(alertId);
    el.textContent = msg;
    el.className = `modal-alert ${type} show`;
}
function hideAlert(alertId) {
    const el = document.getElementById(alertId);
    el.className = 'modal-alert error';
}
function togglePwd(inputId, btn) {
    const inp = document.getElementById(inputId);
    const isText = inp.type === 'text';
    inp.type = isText ? 'password' : 'text';
    btn.querySelector('i').className = isText ? 'bi bi-eye' : 'bi bi-eye-slash';
}
function resetValidation(...ids) {
    ids.forEach(id => document.getElementById(id)?.classList.remove('is-invalid'));
}
document.querySelectorAll('.act-btn').forEach(btn => {
    btn.addEventListener('click', () => openEditModal(btn));
});
function openCreateModal() {
    hideAlert('createAlert');
    resetValidation('c_fullname', 'c_email', 'c_password');
    document.getElementById('c_fullname').value = '';
    document.getElementById('c_email').value = '';
    document.getElementById('c_password').value = '';
    document.getElementById('c_role').value = '1';
    document.getElementById('c_actived').checked = true;
    createModal.show();
}

async function submitCreate() {
    hideAlert('createAlert');
    const fullname = document.getElementById('c_fullname').value.trim();
    const email = document.getElementById('c_email').value.trim();
    const password = document.getElementById('c_password').value;
    const roleId = parseInt(document.getElementById('c_role').value);
    const isActived = document.getElementById('c_actived').checked;

    // Validate
    let valid = true;
    resetValidation('c_fullname', 'c_email', 'c_password');
    if (!fullname) { document.getElementById('c_fullname').classList.add('is-invalid'); valid = false; }
    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
        document.getElementById('c_email').classList.add('is-invalid'); valid = false;
    }
    if (password.length < 6) { document.getElementById('c_password').classList.add('is-invalid'); valid = false; }
    if (!valid) return;

    setLoading('createSubmit', true);
    try {
        const res = await fetch('/admin/users/create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName: fullname, email, password, roleId, isActived })
        });
        const data = await res.json();
        if (res.ok && data) {
            createModal.hide();
            location.reload();
        } else {
            showAlert('createAlert', data.message || data.errors?.[0] || 'Có lỗi xảy ra');
        }
    } catch {
        showAlert('createAlert', 'Không thể kết nối đến server');
    } finally {
        setLoading('createSubmit', false);
    }
}
function openEditModal(btn) {
    hideAlert('editAlert');
    const id = btn.dataset.id;
    const name = btn.dataset.name;
    const email = btn.dataset.email;
    

    document.getElementById('e_userId').value = id;
    document.getElementById('e_fullname').value = name;
    document.getElementById('e_email').value = email;
   

    editModal.show();
}

async function submitEdit() {
    hideAlert('editAlert');
    const userId = parseInt(document.getElementById('e_userId').value);
    const fullName = document.getElementById('e_fullname').value.trim() || null;
    const email = document.getElementById('e_email').value.trim() || null;
    setLoading('editSubmit', true);
    try {
        const res = await fetch(`/admin/users/${userId}/update`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId, fullName, email })
        });
        const data = await res.json();
        if (res.ok && data) {
            editModal.hide();
            location.reload();
        } else {
            showAlert('editAlert', data.message || data.errors?.[0] || 'Có lỗi xảy ra');
        }
    } catch {
        showAlert('editAlert', 'Không thể kết nối đến server');
    } finally {
        setLoading('editSubmit', false);
    }
}
function confirmDelete(btn) {
    document.getElementById('deleteUserId').value = btn.dataset.id;
    document.getElementById('deleteUserName').textContent = btn.dataset.name || '(chưa đặt tên)';
    deleteModal.show();
}

async function submitDelete() {
    const userId = document.getElementById('deleteUserId').value;
    setLoading('deleteSubmit', true);
    try {
        const res = await fetch(`/admin/users/${userId}/delete`, {
            method: 'DELETE'
        });
        const data = await res.json();
        if (res.ok && data) {
            deleteModal.hide();
            location.reload();
        } else {
            alert(data.message || data.errors?.[0] || 'Có lỗi xảy ra khi xoá');
        }
    } catch {
        alert('Không thể kết nối đến server');
    } finally {
        setLoading('deleteSubmit', false);
    }
}

document.getElementById('csvFile').addEventListener('change', uploadCsv);

async function uploadCsv(e) {
    const file = e.target.files[0];

    if (!file) return;

    // ✅ Validate extension
    if (!file.name.endsWith('.csv')) {
        alert("Chỉ chấp nhận file CSV (.csv)");
        e.target.value = '';
        return;
    }

    // ✅ Validate size (5MB)
    if (file.size > 5 * 1024 * 1024) {
        alert("File quá lớn (tối đa 5MB)");
        e.target.value = '';
        return;
    }

    const formData = new FormData();
    formData.append("file", file);

    try {
        // 🔄 loading (optional)
        const btns = document.querySelectorAll('button');
        btns.forEach(b => b.disabled = true);

        const res = await fetch('/admin/users/upload-csv', {
            method: 'POST',
            body: formData
        });

        const data = await res.json();

        if (res.ok) {
            let msg = `✔ Thành công: ${data.data.successCount}\n❌ Lỗi: ${data.data.failedCount}`;

            if (data.data.errors?.length) {
                msg += "\n\nChi tiết:\n" + data.data.errors.join("\n");
            }

            alert(msg);

            location.reload();
        } else {
            alert(data.message || "Import thất bại");
        }

    } catch {
        alert("Không thể kết nối server");
    } finally {
        // reset input
        e.target.value = '';

        // enable lại
        document.querySelectorAll('button').forEach(b => b.disabled = false);
    }
}