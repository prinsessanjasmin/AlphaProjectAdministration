﻿//A separate validation function to not have to deal with JSON and AJAX responses. Suggestion by teacher Emil Vettainen to break it up like this. 

document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form");

    forms.forEach(form => {
        const fields = form.querySelectorAll("[data-val='true']")

        fields.forEach(field => {
            field.addEventListener('input', function () {
                validateField(field)
            });
        });

        form.addEventListener('submit', async (e) => {
           
            clearErrorMessages(form)
            
            let isValid = true;
            fields.forEach(field => {

                if (!validateField(field)) {
                    isValid = false;
                    console.log(`${field.id} failed validation`);
                }
            });

            if (!isValid) {
                e.preventDefault();
                console.log("Form is not valid.");
                return;
            }
            console.log("Field valid.")
            console.log("SUBMITTING FORM")
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

function validateField(field) {
    const formElement = field.closest('form');
    const fieldName = field.name || field.id;

    let errorSpan = document.querySelector(`span[data-valmsg-for='${fieldName}']`);
    if (!errorSpan)
        return; 

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

