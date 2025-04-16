document.addEventListener("DOMContentLoaded", function () {
    
    const forms = document.querySelectorAll("form");
    
    const form = document.querySelector('form');
    const submitButtons = form.querySelectorAll('button[type="submit"], input[type="submit"]');
    submitButtons.forEach(button => {
        button.addEventListener('click', function (e) {

        });
    });


    forms.forEach(form => {
        const fields = form.querySelectorAll("[data-val='true']")


        fields.forEach(field => {
            field.addEventListener('input', function () {
                validateField(field)
            });
        });

        form.addEventListener('submit', async (e) => {
            e.preventDefault()
            console.log("Form submitting with team members:", document.getElementById('selected-team-member-ids')?.value);
            e.stopPropagation();

            let isValid = true;
            fields.forEach(field => {
                const fieldValid = validateField(field);
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
                    method: 'post',
                    body: formData
                });

                if (res.ok) {
                    const modal = form.closest('.modal')
                    if (modal) {
                        modal.style.display = 'none';
                    }
                    window.location.reload()
                }
                else if (res.status === 400) {
                    const data = await res.json()

                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
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
                errorMessage = "End date must be after start date";
            }
        }
    }


    if (field.hasAttribute("required") || field.getAttribute("data-val-required")) {
 
        if (value === "") {
            errorMessage = field.getAttribute("data-val-required") || "Required";
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