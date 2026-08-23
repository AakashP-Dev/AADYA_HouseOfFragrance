// --- TOGGLE PASSWORD VISIBILITY ---
function togglePasswordVisibility(inputId, btn) {
    const input = document.getElementById(inputId);
    const eyeOff = btn.querySelector('.eye-off');
    const eyeOn = btn.querySelector('.eye-on');

    if (input.type === 'password') {
        input.type = 'text';
        if (eyeOff) eyeOff.style.display = 'none';
        if (eyeOn) eyeOn.style.display = 'block';
    } else {
        input.type = 'password';
        if (eyeOff) eyeOff.style.display = 'block';
        if (eyeOn) eyeOn.style.display = 'none';
    }
}

// --- GLOBAL CONFIRMATION MODAL (supports HTML icons) ---
function showGlobalConfirmModal(iconHtml, title, message, onConfirm, onCancel) {
    const modal = document.getElementById('globalConfirmModal');
    const iconEl = document.getElementById('globalModalIcon');
    const titleEl = document.getElementById('globalModalTitle');
    const msgEl = document.getElementById('globalModalMessage');
    const confirmBtn = document.getElementById('globalModalConfirmBtn');
    const cancelBtn = document.getElementById('globalModalCancelBtn');

    // Set content – use innerHTML to allow SVG
    iconEl.innerHTML = iconHtml || '⚠️';
    titleEl.textContent = title || 'Confirm Action';
    msgEl.innerHTML = message;

    modal.style.display = 'flex';

    confirmBtn.onclick = function () {
        modal.style.display = 'none';
        if (typeof onConfirm === 'function') onConfirm();
    };

    cancelBtn.onclick = function () {
        modal.style.display = 'none';
        if (typeof onCancel === 'function') onCancel();
    };

    modal.onclick = function (e) {
        if (e.target === modal) {
            modal.style.display = 'none';
            if (typeof onCancel === 'function') onCancel();
        }
    };
}

// --- LOGOUT (uses themed modal with door SVG) ---
function logoutUser() {
    const doorIcon = `
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="margin-bottom:0.5rem;">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
            <polyline points="16 17 21 12 16 7" />
            <line x1="21" y1="12" x2="9" y2="12" />
        </svg>
    `;
    showGlobalConfirmModal(
        doorIcon,
        'Sign Out',
        'Are you sure you want to sign out?',
        function () {
            fetch('/Account/Logout', { method: 'POST' })
                .finally(() => {
                    localStorage.removeItem('token');
                    localStorage.removeItem('userName');
                    localStorage.removeItem('userId');
                    localStorage.removeItem('isAdmin');
                    showToast('You have been signed out.', 'success');
                    window.location.href = '/';
                });
        },
        function () { }
    );
}

// --- TOAST NOTIFICATION ---
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#2e7d32' : type === 'error' ? '#d32f2f' : '#856404'};
        color: white;
        padding: 12px 24px;
        border-radius: 4px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);
        z-index: 9999;
        font-family: var(--font-body);
        font-size: 0.9rem;
        max-width: 400px;
        animation: slideInTop 0.5s ease;
        transition: opacity 0.5s ease;
    `;
    document.body.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 500);
    }, 3000);
}

// --- CART BADGE UPDATE ---
async function updateCartBadge() {
    const token = localStorage.getItem('token');
    if (!token) {
        const badge = document.getElementById('cartCountBadge');
        if (badge) badge.innerText = '0';
        return;
    }
    try {
        const res = await fetch('/api/cart', {
            headers: { 'Authorization': 'Bearer ' + token }
        });
        const data = await res.json();
        const total = (data.items || []).reduce((s, i) => s + i.quantity, 0);
        const badge = document.getElementById('cartCountBadge');
        if (badge) badge.innerText = total;
    } catch (e) {
        console.error('Badge update error:', e);
    }
}

// --- NAVBAR STATE & DROPDOWN ---
document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');
    const userName = localStorage.getItem('userName');
    const isAdmin = localStorage.getItem('isAdmin') === 'true';

    const loggedOut = document.getElementById('navLoggedOut');
    const loggedIn = document.getElementById('navLoggedIn');
    const userNameSpan = document.getElementById('navUserName');

    if (token && userName) {
        loggedOut.style.display = 'none';
        loggedIn.style.display = 'block';
        userNameSpan.textContent = userName;
        updateCartBadge();
    } else {
        loggedOut.style.display = 'flex';
        loggedIn.style.display = 'none';
    }

    // Show/hide admin dashboard link in dropdown
    const dashboardLink = document.getElementById('adminDashboardLink');
    if (dashboardLink) {
        dashboardLink.style.display = isAdmin ? 'flex' : 'none';
    }

    // Show/hide admin button in nav (if using the separate button)
    const adminNavLink = document.getElementById('adminNavLink');
    if (adminNavLink) {
        adminNavLink.style.display = isAdmin ? 'inline-block' : 'none';
    }

    // --- DROPDOWN TOGGLE ---
    const btn = document.getElementById('userMenuBtn');
    const dropdown = document.getElementById('userDropdown');

    if (btn && dropdown) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const isOpen = dropdown.style.display === 'block';
            dropdown.style.display = isOpen ? 'none' : 'block';
            const arrow = btn.querySelector('.dropdown-arrow');
            if (arrow) {
                arrow.style.transform = isOpen ? 'rotate(0deg)' : 'rotate(180deg)';
            }
        });

        document.addEventListener('click', function () {
            dropdown.style.display = 'none';
            const arrow = btn.querySelector('.dropdown-arrow');
            if (arrow) {
                arrow.style.transform = 'rotate(0deg)';
            }
        });

        dropdown.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    }
});