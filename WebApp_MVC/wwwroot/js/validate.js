document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form:not(.no-validation)");

    forms.forEach(form => {
        const fields = form.querySelectorAll("[data-val='true']")


        fields.forEach(field => {
            field.addEventListener('input', function () {
                validateField(field)
            });
        });

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            e.stopPropagation();
            await submitFormAsync(form);

        });
    });
});

async function submitFormAsync(form) {
    const fields = form.querySelectorAll("[data-val='true']")
    let isValid = true;

    fields.forEach(field => {
        if (!validateField(field)) {
            isValid = false;
        }
    });

    const teamMembersField = form.querySelector('#SelectedTeamMemberIds');
    if (teamMembersField && !validateMemberSelection(teamMembersField)) {
        isValid = false;
    }

    if (!isValid) {
        return;
    }

    clearErrorMessages(form)
    const formData = new FormData(form)

    console.log("Form data before submission:");
    for (let pair of formData.entries()) {
        console.log(pair[0] + ': ' + pair[1]);
    }

    try {
        const res = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        console.log("Fetch response: ", res);
        const contentType = res.headers.get("Content-Type") || "";

        if (res.ok && contentType.includes("application/json")) {
           

            if (contentType && contentType.includes("application/json")) {
                const data = await res.json();
                console.log("AJAX response data:", data);

                if (data.success) {
                    const modal = form.closest('.modal');
                    if (modal) {
                        modal.style.display = 'none';
                    }

                    // Redirect to the URL from the server (e.g., returnUrl)
                    if (data.redirectUrl) {
                        window.location.href = data.redirectUrl;
                    } else {
                        // fallback, in case redirectUrl is missing
                        window.location.reload();
                    }
                }
            } else if (data.message) {
                addErrorMessage(form, "", data.message);
            }
        }
        else if (res.status === 400 && contentType.includes("application/json")) {
            
            const data = await res.json();
            if (data.errors) {
                Object.entries(data.errors).forEach(([key, messages]) => {
                    addErrorMessage(form, key, messages.join('\n'));
                });
            } else if (data.message) {
                addErrorMessage(form, "", data.message);
            }
        }
        else {
            const textResponse = await res.text();
            console.error('Non-JSON error response:', textResponse);
            addErrorMessage(form, "", "An unexpected error occurred");
        }
    }
    catch (error) {
        console.error('Error submitting form:', error);
        addErrorMessage(form, "", "An error occurred while submitting the form.");
    }
}

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
        input = form.querySelector(`#${normalizedKey}`);
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
}

function validateField(field) {
    const formElement = field.closest('form');
    const fieldName = field.name || field.id; 

    if (fieldName === "SelectedTeamMemberIds") {
        return validateMemberSelection(field);
    }

    let errorSpan = document.querySelector(`span[data-valmsg-for='${fieldName}']`);
    if (!errorSpan)
        errorSpan = formElement.querySelector(`span[for="${fieldName}"]`);

    if (!errorSpan) {
        return true;
    }

    let errorMessage = "";
    let value = field.value.trim();

    if (field.hasAttribute('required') || field.getAttribute('data-val-required')) {
        if (value === "") {
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
                errorMessage = "Too early";
            }
        }
    }

    if (errorMessage) {
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
}  

function validateMemberSelection(field) {
    const fieldName = field.name || field.id;
    const formElement = field.closest('form');

    let errorSpan = document.querySelector(`span[data-valmsg-for='${fieldName}']`);
    if (!errorSpan) {
        errorSpan = formElement.querySelector(`span[for='${fieldName}']`);
    }

    if (!errorSpan) {
        return true;
    }

    let value = field.value.trim();
    console.log("Validating team members:", value);
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

