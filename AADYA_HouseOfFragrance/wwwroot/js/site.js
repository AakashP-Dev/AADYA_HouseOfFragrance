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

// --- NAVBAR STATE & DROPDOWN ---
document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');
    const userName = localStorage.getItem('userName');

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

    // --- DROPDOWN TOGGLE ---
    const btn = document.getElementById('userMenuBtn');
    const dropdown = document.getElementById('userDropdown');

    if (btn && dropdown) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const isOpen = dropdown.style.display === 'block';
            dropdown.style.display = isOpen ? 'none' : 'block';
            // Rotate arrow
            const arrow = btn.querySelector('.dropdown-arrow');
            if (arrow) {
                arrow.style.transform = isOpen ? 'rotate(0deg)' : 'rotate(180deg)';
            }
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function () {
            dropdown.style.display = 'none';
            const arrow = btn.querySelector('.dropdown-arrow');
            if (arrow) {
                arrow.style.transform = 'rotate(0deg)';
            }
        });

        // Prevent closing when clicking inside dropdown
        dropdown.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    }
});

// --- LOGOUT ---
function logoutUser() {
    if (confirm('Are you sure you want to sign out?')) {
        localStorage.removeItem('token');
        localStorage.removeItem('userName');
        localStorage.removeItem('userId');
        showToast('You have been signed out.', 'success');
        window.location.href = '/';
    }
}

// --- CART BADGE ---
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