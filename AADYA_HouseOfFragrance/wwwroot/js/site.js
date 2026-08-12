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
        window.location.href = '/';
    }
}