// Global AJAX Setup
$.ajaxSetup({
    headers: {
        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
    }
});

// Handle Session Timeout
let sessionTimeout;
const warningTime = 5 * 60 * 1000; // 5 minutes before session expires
const redirectTime = 30 * 60 * 1000; // 30 minutes total session time

function startSessionTimer() {
    clearTimeout(sessionTimeout);
    sessionTimeout = setTimeout(showSessionWarning, warningTime);
}

function showSessionWarning() {
    Swal.fire({
        title: 'Session Expiring',
        text: 'Your session will expire in 5 minutes. Would you like to continue?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, keep me signed in',
        cancelButtonText: 'Sign out'
    }).then((result) => {
        if (result.isConfirmed) {
            refreshSession();
        } else {
            window.location.href = '/Account/Logout';
        }
    });
}

function refreshSession() {
    $.post('/Account/RefreshSession')
        .done(function () {
            startSessionTimer();
        })
        .fail(function () {
            window.location.href = '/Account/Login';
        });
}

// Form Validation
function validateForm(formSelector) {
    const $form = $(formSelector);
    if ($form.length === 0) return true;

    let isValid = true;
    $form.find('input[required], select[required], textarea[required]').each(function () {
        if (!$(this).val()) {
            isValid = false;
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    return isValid;
}

// Data Table Initialization
function initializeDataTable(tableId, options = {}) {
    const defaultOptions = {
        responsive: true,
        pageLength: 10,
        language: {
            search: "Search:",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        }
    };

    return $(tableId).DataTable({ ...defaultOptions, ...options });
}

// Toast Notifications
function showToast(message, type = 'success') {
    const toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });

    toast.fire({
        icon: type,
        title: message
    });
}

// File Upload Preview
function handleFileUpload(input, previewElement) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $(previewElement).attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Format Currency
function formatCurrency(amount, currency = 'USD') {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

// Format Date
function formatDate(date, format = 'short') {
    const options = format === 'short' 
        ? { year: 'numeric', month: 'short', day: 'numeric' }
        : { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    
    return new Date(date).toLocaleDateString('en-US', options);
}

// Handle AJAX Errors
$(document).ajaxError(function (event, jqXHR, settings, error) {
    if (jqXHR.status === 401) {
        window.location.href = '/Account/Login';
    } else if (jqXHR.status === 403) {
        window.location.href = '/Account/AccessDenied';
    } else {
        showToast('An error occurred. Please try again.', 'error');
    }
});

// Document Ready
$(document).ready(function () {
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-toggle="popover"]').popover();

    // Start session timer
    startSessionTimer();

    // Reset session timer on user activity
    $(document).on('click keypress', startSessionTimer);

    // Handle form validation
    $('form').on('submit', function (e) {
        if (!validateForm(this)) {
            e.preventDefault();
            showToast('Please fill in all required fields.', 'error');
        }
    });

    // Handle file upload previews
    $('.custom-file-input').on('change', function () {
        const fileName = $(this).val().split('\\').pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});

// Export functionality
function exportTable(tableId, format) {
    const table = $(tableId).DataTable();
    
    if (format === 'excel') {
        table.buttons('.buttons-excel').trigger();
    } else if (format === 'pdf') {
        table.buttons('.buttons-pdf').trigger();
    } else if (format === 'print') {
        table.buttons('.buttons-print').trigger();
    }
}

// Print functionality
function printElement(elementId) {
    const printContents = document.getElementById(elementId).innerHTML;
    const originalContents = document.body.innerHTML;

    document.body.innerHTML = printContents;
    window.print();
    document.body.innerHTML = originalContents;
}
