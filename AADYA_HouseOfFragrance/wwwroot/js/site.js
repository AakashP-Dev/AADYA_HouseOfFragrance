function togglePasswordVisibility(inputId, btn) {
    const input = document.getElementById(inputId);
    const eyeOff = btn.querySelector('.eye-off');
    const eyeOn = btn.querySelector('.eye-on');

    if (input.type === 'password') {
        input.type = 'text';
        eyeOff.style.display = 'none';
        eyeOn.style.display = 'block';
    } else {
        input.type = 'password';
        eyeOff.style.display = 'block';
        eyeOn.style.display = 'none';
    }
}

// Script to handle dynamic navbar state
document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');
    const userName = localStorage.getItem('userName');

    const navActions = document.querySelector('.nav-actions');

    if (token && userName) {
        navActions.innerHTML = `
            <span style="color: var(--deep-burgundy); font-weight: 600; margin-right: 0.5rem;">👋 ${userName}</span>
            <a href="/Home/Cart" class="btn-outline nav-btn">🛒 CART</a>
            <a href="#" class="btn-gold nav-btn" onclick="logoutUser()">SIGN OUT</a>
        `;
    } else {
        navActions.innerHTML = `
            <a href="/Home/Cart" class="btn-outline nav-btn">🛒 CART</a>
            <a href="/Account/Login" class="nav-link-sign">SIGN IN</a>
            <a href="/Account/Register" class="btn-gold nav-btn">REGISTER</a>
        `;
    }
});

// Global logout function
function logoutUser() {
    if (confirm('Are you sure you want to sign out?')) {
        localStorage.removeItem('token');
        localStorage.removeItem('userName');
        localStorage.removeItem('userId');
        showToast('You have been signed out.', 'success');
        window.location.href = '/';
    }
}

// Toast notification
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