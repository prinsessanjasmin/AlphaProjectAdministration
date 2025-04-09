document.addEventListener("DOMContentLoaded", function () {
    console.log("Validation script loaded");
    const forms = document.querySelectorAll("form");
    console.log("Forms found:", forms.length);
    const form = document.querySelector('form');
    const submitButtons = form.querySelectorAll('button[type="submit"], input[type="submit"]');
    submitButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            console.log("Submit button clicked");
        });
    });

    console.log("Submit buttons in form:", form.querySelectorAll('button[type="submit"], input[type="submit"]').length);

    forms.forEach(form => {
        const fields = form.querySelectorAll("[data-val='true']")
        console.log("Validation fields in form:", fields.length);

        fields.forEach(field => {
            field.addEventListener('input', function () {
                validateField(field)
            });
        });

        form.addEventListener('submit', async (e) => {
            console.log("Form submit event triggered");
            e.preventDefault()
            e.stopPropagation();
            alert('Form submit prevented');

            let isValid = true;
            fields.forEach(field => {
                console.log("Validating field:", field.name);
                const fieldValid = validateField(field);
                if (!validateField(field)) {
                    console.log(`Field ${field.name} valid:`, fieldValid);
                    isValid = false;
                }
            });

            const teamMembersField = form.querySelector('#SelectedTeamMemberIds');
            if (teamMembersField && !validateMemberSelection(teamMembersField)) {
                isValid = false;
            }

            console.log("Form valid, proceeding with submission");
            console.log("Overall form valid:", isValid);
            if (!isValid) {
                console.log("Stopping submission due to validation failures");
                return;
            }

            clearErrorMessages(form)
            const formData = new FormData(form)

            try {
                const res = await fetch(form.action, {
                    method: 'post',
                    body: formData
                });

                console.log("Response status:", res.status);

                if (res.ok) {
                    console.log("Request successful");
                    const modal = form.closest('.modal')
                    if (modal) {
                        modal.style.display = 'none';
                    }
                    window.location.reload()
                }
                else if (res.status === 400) {
                    console.log("Bad request - validation errors");
                    const data = await res.json()
                    console.log('Validation errors:', data);

                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
                            console.log(`Adding error for ${key}:`, data.errors[key]);
                            addErrorMessage(form, key, data.errors[key].join('\n'))
                        });
                    }
                }
            }
            catch (error) {
                console.error('Error submitting form:', error);
            }
        });
    });  
});

function clearErrorMessages(form) {
    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove('input-validation-error');
    });

    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.textContent = '';
        span.classList.remove('field-validation.error');
        span.classList.add('field-validation-valid');
    });
}

function addErrorMessage(form, key, errorMessage) {
    const normalizedKey = key.replace(/\./g, '_');
    let input = form.querySelector(`[name="${key}"]`);
    if (!input) {
        input = form.querySelector(`#{$normalizedKey}`);
    }
    if (input) {
        input.classList.add('input-validation-error');
    }

    let span = form.querySelector(`[data-valmsg-for="${key}"]`);
    if (!span) {
        span = form.querySelector(`span[for="${normalizedKey}"]`);
    }

    if (span) {
        span.textContent = errorMessage;
        span.classList.remove('field-validation-valid');
        span.classList.add('field-validation-error');
    }
    else {
        console.warn(`Validation span not found for ${key}`);
    }
}

function validateField(field) {
    const formElement = field.closest('form');
    const fieldName = field.name || field.id; 

    if (fieldName === "SelectedTeamMemberIds") {
        return validateMemberSelection(field);
    }

    let errorSpan = document.querySelector(`span[data-valmsg-for='${fieldName}']`);
    if (!errorSpan)
        errorSpan = formElement.querySelector(`span[for="$fieldName"]`);

    if (!errorSpan) {
        console.warn(`No validation span found for ${fieldName}`);
        return true;
    }

    console.log(`Found error span for ${fieldName}`);

    let errorMessage = "";
    let value = field.value.trim();

    if (field.hasAttribute('required') || field.getAttribute('data-val-required')) {
        if (field === "") {
            errorMessage = field.getAttribute("data-val-required") || "Required";
        } 
    }

    if (errorMessage === "" && field.hasAttribute("data-val-regex") && value !== "") {
        const pattern = new RegExp(field.getAttribute("data-val-regex-pattern")); 
        if (!pattern.test(value)) {
            errorMessage = field.getAttribute("data-val-regex"); 
        }
    }

    if (field.type === "date" && fieldName === "EndDate") {
        const startDateField = formElement.querySelector('[name="StartDate"]');
        if (startDateField && value && startDateField.value) {
            const startDate = new Date(startDateField.value);
            const endDate = new Date(value);

            if (endDate < startDate) {
                errorMessage = "End date must be after start date";
            }
        }
    }


    if (field.hasAttribute("required") || field.getAttribute("data-val-required")) {
        console.log(`Field ${fieldName} is required, current value: "${value}"`);
        console.log(`Required attribute: ${field.hasAttribute("required")}`);
        console.log(`data-val-required attribute: ${field.getAttribute("data-val-required")}`);

        if (value === "") {
            errorMessage = field.getAttribute("data-val-required") || "Required";
            console.log(`Setting error message to: "${errorMessage}"`);
        }
    }

    console.log(`Validating field ${fieldName} with value "${field.value}"`);

    if (errorMessage) {
        console.log(`Field ${fieldName} invalid. Setting error: "${errorMessage}"`);
        field.classList.add("input-validation-error");
        errorSpan.classList.remove("field-validation-valid");
        errorSpan.classList.add("field-validation-error");
        errorSpan.textContent = errorMessage;
        return false; 
    }
    else {
        field.classList.remove("input-validation-error");
        errorSpan.classList.remove("field-validation-error");
        errorSpan.classList.add("field-validation-valid");
        errorSpan.textContent = "";
        return true;
    }

    function validateMemberSelection(field) {
        const fieldName = field.name || field.id; 
        const formElement = field.closest('form');

        let errorSpan = document.querySelector(`span[data-valmsg-for='${fieldName}']`);
        if (!errorSpan) {
            errorSpan = formElement.querySelector(`span[for='${fieldName}']`);
        }

        if (!errorSpan) {
            console.warn(`No validation found for ${fieldName}`);
            return true;
        }

        let value = field.value.trim(); 
        let errorMessage = "";

        try {
            const selectedMembers = JSON.parse(value || "[]");
            if (!Array.isArray(selectedMembers) || selectedMembers.length === 0) {
                errorMessage = "Please select at least one member";
            }
        }
        catch (e) {
            errorMessage = "Invalid team member selection"; 
        }
        
        if (errorMessage) {
            field.classList.add("input-validation-error");
            errorSpan.classList.remove("field-validation-valid");
            errorSpan.classList.add("field-validation-error");
            errorSpan.textContent = errorMessage;
            return false;
        } else {
            field.classList.remove("input-validation-error");
            errorSpan.classList.remove("field-validation-error");
            errorSpan.classList.add("field-validation-valid");
            errorSpan.textContent = "";
            return true;
        }
    }
}  