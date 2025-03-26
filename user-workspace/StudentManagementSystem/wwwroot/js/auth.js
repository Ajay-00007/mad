// Enrollment ID validation (Format: I + 8 digits)
function validateEnrollmentId(enrollmentId) {
    const pattern = /^I\d{8}$/;
    return pattern.test(enrollmentId);
}

// Date of Birth validation (must be in the past and not too old)
function validateDateOfBirth(dob) {
    const dobDate = new Date(dob);
    const today = new Date();
    const minAge = new Date();
    minAge.setFullYear(today.getFullYear() - 100); // Max age 100 years
    
    return dobDate < today && dobDate > minAge;
}

// Registration form validation
document.addEventListener('DOMContentLoaded', function() {
    const registerForm = document.querySelector('form[action="/Register"]');
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const fullName = registerForm.querySelector('[name="Register.FullName"]').value;
            const email = registerForm.querySelector('[name="Register.Email"]').value;
            const phone = registerForm.querySelector('[name="Register.PhoneNumber"]').value;
            const dob = registerForm.querySelector('[name="Register.DateOfBirth"]').value;

            let isValid = true;
            let errorMessages = [];

            // Full Name validation
            if (fullName.trim().length < 2) {
                isValid = false;
                errorMessages.push('Full name must be at least 2 characters long');
                registerForm.querySelector('[name="Register.FullName"]').classList.add('is-invalid');
            }

            // Email validation
            if (!validateEmail(email)) {
                isValid = false;
                errorMessages.push('Please enter a valid email address');
                registerForm.querySelector('[name="Register.Email"]').classList.add('is-invalid');
            }

            // Phone validation
            if (!validatePhone(phone)) {
                isValid = false;
                errorMessages.push('Please enter a valid phone number');
                registerForm.querySelector('[name="Register.PhoneNumber"]').classList.add('is-invalid');
            }

            // Date of Birth validation
            if (!validateDateOfBirth(dob)) {
                isValid = false;
                errorMessages.push('Please enter a valid date of birth');
                registerForm.querySelector('[name="Register.DateOfBirth"]').classList.add('is-invalid');
            }

            if (!isValid) {
                e.preventDefault();
                // Display error messages
                const errorDiv = document.createElement('div');
                errorDiv.className = 'alert alert-danger';
                errorDiv.innerHTML = errorMessages.map(msg => `<div>${msg}</div>`).join('');
                
                const existingError = registerForm.querySelector('.alert-danger');
                if (existingError) {
                    existingError.remove();
                }
                registerForm.insertBefore(errorDiv, registerForm.firstChild);
            }
        });

        // Real-time validation
        registerForm.querySelectorAll('input').forEach(input => {
            input.addEventListener('input', function() {
                this.classList.remove('is-invalid');
                const errorDiv = registerForm.querySelector('.alert-danger');
                if (errorDiv) {
                    errorDiv.remove();
                }
            });
        });
    }
});

// Login form validation
document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.querySelector('form[action="/"]');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const enrollmentId = loginForm.querySelector('[name="Login.EnrollmentId"]').value;
            const dob = loginForm.querySelector('[name="Login.DateOfBirth"]').value;

            let isValid = true;
            let errorMessages = [];

            // Enrollment ID validation
            if (!validateEnrollmentId(enrollmentId)) {
                isValid = false;
                errorMessages.push('Please enter a valid Enrollment ID (Format: I followed by 8 digits)');
                loginForm.querySelector('[name="Login.EnrollmentId"]').classList.add('is-invalid');
            }

            // Date of Birth validation
            if (!validateDateOfBirth(dob)) {
                isValid = false;
                errorMessages.push('Please enter a valid date of birth');
                loginForm.querySelector('[name="Login.DateOfBirth"]').classList.add('is-invalid');
            }

            if (!isValid) {
                e.preventDefault();
                // Display error messages
                const errorDiv = document.createElement('div');
                errorDiv.className = 'alert alert-danger';
                errorDiv.innerHTML = errorMessages.map(msg => `<div>${msg}</div>`).join('');
                
                const existingError = loginForm.querySelector('.alert-danger');
                if (existingError) {
                    existingError.remove();
                }
                loginForm.insertBefore(errorDiv, loginForm.firstChild);
            }
        });

        // Real-time validation
        loginForm.querySelectorAll('input').forEach(input => {
            input.addEventListener('input', function() {
                this.classList.remove('is-invalid');
                const errorDiv = loginForm.querySelector('.alert-danger');
                if (errorDiv) {
                    errorDiv.remove();
                }
            });
        });
    }
});

// Show/Hide password functionality
document.addEventListener('DOMContentLoaded', function() {
    const togglePasswordButtons = document.querySelectorAll('.toggle-password');
    togglePasswordButtons.forEach(button => {
        button.addEventListener('click', function() {
            const input = document.querySelector(this.getAttribute('data-target'));
            if (input) {
                if (input.type === 'password') {
                    input.type = 'text';
                    this.innerHTML = '<i class="fas fa-eye-slash"></i>';
                } else {
                    input.type = 'password';
                    this.innerHTML = '<i class="fas fa-eye"></i>';
                }
            }
        });
    });
});