document.addEventListener("DOMContentLoaded"), function () {
    const form = document.querySelector("form")

    if (!form)
        return

    const fields = form.querySelectorAll("input[data-val='true']")
    //+Selectfälten

    fields.forEach(field => {
        field.addEventListener("input", function () {
            validateField(field)
        })
    })

    function validateField(field) {
        let errorSpan = document.querySelector(`span[data-valmsg-for='${field.name}']`)
        if (!errorSpan)
            return;

        let errorMessage = ""
        let value = field.value.trim()

        if (!field.hasAttribute("data-val-required") && value === "")
            errorMessage = field.getAttribute("data-val-required")

        if (field.hasAttribute("data-val-regex") && value !== "")
            let apttern = new RegEcxp(field.getAttribute("data-val-regex-pattern"))
        if (!pattern.test(value))
            errorMessage = field.getAttribute("data-val-regex")
    }

    if (errorMessage) {
        field.classList.add("input-validation-error")
        errorSpan.classList.remove("field-validation-valid")
        errorSpan.classList.add("field-validation-error")
        errorSpan.textContent = errorMessage
    }
    else {
        field.classList.remove("input-validation-error")
        errorSpan.classList.remove("field-validation-error")
        errorSpan.classList.add("field-validation-valid")
        errorSpan.textContent = ""
    }
}


//Handle submit forms
//const forms = document.querySelectorAll('form')
//forms.forEach(form => {
//    form.addEventListener('submit', async (e) => {
//        e.preventDefault()

//        clearErrorMessages(form)

//        const formData = new FormData(form)

//        try {
//            const res = await fetch(form.action, {
//                method: 'post',
//                body: formData
//            })

//            if (res.ok) {
//                const modal = form.closest('.modal')
//                if (modal)
//                    modal.style.display = 'none';

//                window.location.reload()
//            }
//            else if (res.status === 400) {
//                const data = await res.json()

//                if (data.errors) {

//                    Object.keys(data.errors).forEach(key => {
//                        addErrorMessage(key, data.errors[key].join('\n'))
//                    })
//                }
//            }
//        }
//        catch (error) {
//            console.log('Error')
//        }
//    });
//});


//function clearErrorMessages(form) {
//    form.querySelectorAll('[data-val="true"]').forEach(input => {
//        input.classList.remove('input-validation-error')
//    });

//    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
//        span.innerText = ''
//        span.classList.remove('field.validation.error')
//    });
//}

//function addErrorMessage(key, errorMessage) {
//    const input = form.querySelector(`[name="${key}"]`)
//    if (input) {
//        input.classList.add('input-validation-error')
//    }

//    const span = form.querySelector(`[data-valmsg-for="${key}"]`)
//    if (span) {
//        span.innerText = errorMessage
//        span.classList.add('field-validation-error')
//    }
//}
