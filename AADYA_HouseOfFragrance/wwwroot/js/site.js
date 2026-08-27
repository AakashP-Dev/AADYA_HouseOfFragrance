// ==========================================================================
// UTILITY FUNCTIONS – AADYA: The House of Fragrance
// ==========================================================================

// ==========================================================================
// 1. PASSWORD VISIBILITY TOGGLE
// ==========================================================================

/**
 * Toggles the visibility of a password input field.
 * Swaps between text and password types, and toggles eye icon visibility.
 * @@param {string} inputId - The ID of the input element.
 * @@param {HTMLElement} btn - The toggle button containing eye icons.
 */
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

// ==========================================================================
// 2. GLOBAL CONFIRMATION MODAL
// ==========================================================================

/**
 * Displays a customizable confirmation modal with icon, title, and message.
 * Supports HTML/SVG content in the icon and message for theming.
 * @@param {string} iconHtml - HTML/SVG string for the modal icon.
 * @@param {string} title - Modal title text.
 * @@param {string} message - Modal message (HTML allowed).
 * @@param {Function} onConfirm - Callback when confirm button is clicked.
 * @@param {Function} onCancel - Callback when cancel button is clicked.
 */
function showGlobalConfirmModal(iconHtml, title, message, onConfirm, onCancel) {
    const modal = document.getElementById('globalConfirmModal');
    const iconEl = document.getElementById('globalModalIcon');
    const titleEl = document.getElementById('globalModalTitle');
    const msgEl = document.getElementById('globalModalMessage');
    const confirmBtn = document.getElementById('globalModalConfirmBtn');
    const cancelBtn = document.getElementById('globalModalCancelBtn');

    // Set content (innerHTML allows SVG icons)
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

    // Close modal when clicking outside the content
    modal.onclick = function (e) {
        if (e.target === modal) {
            modal.style.display = 'none';
            if (typeof onCancel === 'function') onCancel();
        }
    };
}

// ==========================================================================
// 3. LOGOUT
// ==========================================================================

/**
 * Logs out the current user with a themed confirmation modal.
 * Clears localStorage and session, then redirects to home.
 */
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
                    // Clear local storage
                    localStorage.removeItem('token');
                    localStorage.removeItem('userName');
                    localStorage.removeItem('userId');
                    localStorage.removeItem('isAdmin');

                    showToast('You have been signed out.', 'success');
                    window.location.href = '/';
                });
        },
        function () { /* User cancelled – do nothing */ }
    );
}

// ==========================================================================
// 4. TOAST NOTIFICATIONS
// ==========================================================================

/**
 * Displays a temporary toast notification at the top-right corner.
 * Automatically disappears after 3 seconds.
 * @@param {string} message - The message to display.
 * @@param {string} type - Notification type: 'success', 'error', or 'warning'.
 */
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.textContent = message;

    // Map type to background color
    const bgColors = {
        success: '#2e7d32',
        error: '#d32f2f',
        warning: '#856404'
    };
    const bgColor = bgColors[type] || bgColors.success;

    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${bgColor};
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

    // Auto-remove after 3 seconds
    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 500);
    }, 3000);
}

// ==========================================================================
// 5. CART BADGE
// ==========================================================================

/**
 * Fetches the current user's cart count and updates the badge in the navbar.
 * If the user is not logged in, resets the badge to 0.
 */
async function updateCartBadge() {
    const token = localStorage.getItem('token');
    const badge = document.getElementById('cartCountBadge');

    if (!token) {
        if (badge) badge.innerText = '0';
        return;
    }

    try {
        const res = await fetch('/api/cart', {
            headers: { 'Authorization': 'Bearer ' + token }
        });
        const data = await res.json();

        const total = (data.items || []).reduce((sum, item) => sum + item.quantity, 0);
        if (badge) badge.innerText = total;
    } catch (error) {
        console.error('Badge update error:', error);
    }
}

// ==========================================================================
// 6. NAVBAR STATE & DROPDOWN (Initialization)
// ==========================================================================

/**
 * Initializes the navbar state on page load:
 * - Shows/hides login/register vs user dropdown based on token presence.
 * - Toggles admin dashboard link visibility.
 * - Sets up dropdown toggle behavior.
 */
document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');
    const userName = localStorage.getItem('userName');
    const isAdmin = localStorage.getItem('isAdmin') === 'true';

    const loggedOut = document.getElementById('navLoggedOut');
    const loggedIn = document.getElementById('navLoggedIn');
    const userNameSpan = document.getElementById('navUserName');

    // Show/hide auth sections based on login status
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

    // (Optional) Admin button in nav – if the element exists
    const adminNavLink = document.getElementById('adminNavLink');
    if (adminNavLink) {
        adminNavLink.style.display = isAdmin ? 'inline-block' : 'none';
    }

    // --- DROPDOWN TOGGLE ---
    const btn = document.getElementById('userMenuBtn');
    const dropdown = document.getElementById('userDropdown');

    if (btn && dropdown) {
        // Toggle dropdown on button click
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const isOpen = dropdown.style.display === 'block';
            dropdown.style.display = isOpen ? 'none' : 'block';

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

        // Prevent clicks inside dropdown from closing it
        dropdown.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    }
});