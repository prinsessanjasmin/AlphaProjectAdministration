document.addEventListener('DOMContentLoaded', () => {
    if (!getCookie("cookieConsent")) {
        openCookieConsentModal();
    }
});

function openCookieConsentModal() {
    const modal = document.getElementById('cookieModal');
    if (modal) modal.classList.add("visible");

    const consentValue = getCookie('cookieConsent');
    if (!consentValue) return;

    try {
        const consent = JSON.parse(consentValue);

        const functional = document.getElementById('cookieFunctional'); 
        const analytical = document.getElementById('cookieAnalytical'); 
        const marketing = document.getElementById('cookieMarketing'); 

        if (functional) functional.checked = consent.functional;
        if (analytical) analytical.checked = consent.analytical;
        if (marketing) marketing.checked = consent.marketing;
    }
    catch (error) {
        console.error('Unable to handle cookie consent values', error);
    }
}

function closeCookieConsentModal() {
    const modal = document.getElementById('cookieModal');
    if (modal) modal.classList.remove("visible");
}

function getCookie(name) {
    const nameEQ = name + "="; 
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.indexOf(nameEQ) === 0) {
            return decodeURIComponent(cookie.substring(nameEQ.length));
        }
    }
    return null; 
}

function setCookie(name, value, days) {
    let expires = ""; 
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }

    const encodedValue = encodeURIComponent(value || "");
    document.cookie = `${name}=${encodedValue}${expires}; path=/; SameSite=Lax`
}

async function acceptAll() {
    const consent = {
        essential: true,
        functional: true,
        analytical: true,
        marketing: true
    }

    setCookie("cookieConsent", JSON.stringify(consent), 90)
    await handleConsent(consent);
    closeCookieConsentModal(); 
}

async function acceptSelected() {
    const form = document.getElementById("cookieConsentForm");
    const formData = new FormData(form);

     const consent = {
        essential: true,
        functional: formData.get("functional") === "on",
         analytical: formData.get("analytical") === "on",
         marketing: formData.get("marketing") === "on",
    }

    setCookie("cookieConsent", JSON.stringify(consent), 90)
    await handleConsent(consent);
    closeCookieConsentModal(); 

}

async function handleConsent(consent) {
    try {

        //Had help here from ClaudeAI with tracking the user, since my original script didn't take into account if the user changed 
        const userId = document.querySelector('meta[name="user-id"]')?.content;
        const requestData = {
            ...consent,
            userId: userId || null  // Include user ID if available
        };

        const res = await fetch('/cookies/setcookies', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(consent)
        });

        if (!res.ok) {
            console.error('Unable to set cookie consent', await res.text());
        }
    }
    catch (error) {
        console.error("Error: ", error);
    }
} 


