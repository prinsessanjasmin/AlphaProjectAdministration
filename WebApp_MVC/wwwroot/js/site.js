document.addEventListener('DOMContentLoaded', () => {

    initializeDropdowns();
    updateRelativeTimes();
    setInterval(updateRelativeTimes, 6000);
    

    const controller = document.body.dataset.controller;
    const sidebarProjects = document.querySelector('#sidebar-projects');
    const sidebarMembers = document.querySelector('#sidebar-members');
    const sidebarClients = document.querySelector('#sidebar-clients');

    [sidebarProjects, sidebarMembers, sidebarClients].forEach(navButton => navButton?.classList.remove('active-controller'));

    if (controller === "Project" && sidebarProjects) {
        sidebarProjects.classList.add('active-controller');
        initializeProjectFilters();
    } else if (controller === "Employee" && sidebarMembers) {
        sidebarMembers.classList.add('active-controller');
    } else if (controller === "Client" && sidebarClients) {
        sidebarClients.classList.add('active-controller');
    }

    sidebarProjects?.addEventListener('click', () => {
        window.location.href = '/project';
    });

    sidebarClients?.addEventListener('click', () => {
        window.location.href = '/client';
    });

    sidebarMembers?.addEventListener('click', () => {
        window.location.href = '/employee';
    });

    const daySelect = document.getElementById("day");
    const monthSelect = document.getElementById("month");
    const yearSelect = document.getElementById("year");

    if (daySelect && monthSelect && yearSelect) {
        populateDateDropdowns(daySelect, monthSelect, yearSelect);
    }

    //modals
    const modalButtons = document.querySelectorAll('[data-modal="true"]')
    modalButtons.forEach(button => {
        button.addEventListener('click', async () => {
            const modalTarget = button.getAttribute('data-target')
            const modal = document.querySelector(modalTarget)

            if (document.querySelector('.modal')) {
                document.querySelector('.modal').scrollTop = 0;
            }
            if (document.querySelector('.modal-content')) {
                document.querySelector('.modal-content').scrollTop = 0;
            }
            if (!modal) return;

            // Get the ID if present (for edit and delete modals)
            const itemId = button.getAttribute('data-id');

            if ((modalTarget === '#delete-project-modal' ||
                modalTarget === '#delete-employee-modal' ||
                modalTarget === '#delete-client-modal') && itemId) {

                try {
                    const url = `/${controller}/ConfirmDelete?id=${itemId}`;

                    console.log(url); 

                    const response = await fetch(url);
                    if (!response.ok) throw new Error("Failed to load delete confirmation");

                    const data = await response.text();
                    modal.querySelector('.modal-content').innerHTML = data;
                }
                catch (error) {
                    console.error("Error loading delete confirmation:", error);
                    return;
                }
            }

            // Check if we need to load content dynamically (edit modal)
            else if (itemId) {
                const action = button.getAttribute('data-action');
                const controller = button.getAttribute('data-controller');

                if (!action || !controller) {
                    console.error("Missing asp-action or asp-controller attributes.");
                    return;
                }

                const url = `/${controller}/${action}?id=${itemId}`;

                try {
                    const response = await fetch(url);
                    if (!response.ok) throw new Error("Failed to load modal content");

                    const data = await response.text();
                    modal.querySelector('.modal-content').innerHTML = data;
                    initializeModalForms(modal);

                    //Claude AI generated this for me to make sure the script is loading 
                    const scripts = modal.querySelectorAll('script');
                    scripts.forEach(oldScript => {
                        const newScript = document.createElement('script');
                        Array.from(oldScript.attributes).forEach(attr =>
                            newScript.setAttribute(attr.name, attr.value)
                        );
                        newScript.textContent = oldScript.textContent;
                        oldScript.parentNode.replaceChild(newScript, oldScript);
                    });

                    const daySelect = modal.querySelector('#day');
                    const monthSelect = modal.querySelector('#month');
                    const yearSelect = modal.querySelector('#year');

                    if (daySelect && monthSelect && yearSelect) {
                        // For edit modals, the selected values will be set by the script in the modal
                        // content if it includes the Model values
                        populateDateDropdowns(daySelect, monthSelect, yearSelect);
                    }
                }
                catch (error) {
                    console.error("Error loading modal content:", error);
                    return;
                }
            }

            modal.classList.add('visible');

        });
    });
});

document.addEventListener('click', (event) => {
    if (event.target.matches('[data-close="true"]')) {
        const modal = event.target.closest('.modal');
        if (modal) {
            modal.classList.remove('visible');

            modal.querySelectorAll('form').forEach(form => {
                form.reset();

                const imagePreview = form.querySelector('.image-preview');
                if (imagePreview) imagePreview.src = '';

                const imagePreviewer = form.querySelector('.image-previewer');
                if (imagePreviewer) imagePreviewer.classList.remove('selected');

                const existingMemberContainer = form.querySelector('#selected-member-display');

                if (existingMemberContainer) {
                    while (existingMemberContainer.hasChildNodes()) {
                        existingMemberContainer.removeChild(existingMemberContainer.firstChild)
                    }
                }

                const selectedIdsInput = form.querySelector('#selected-team-member-ids');
                if (selectedIdsInput) {
                    selectedIdsInput.value = '[]';
                }

            });
        }
    }
});

//open project options when clicking details button (had som help from ChatGPT 4o here)
document.querySelectorAll(".btn-2-dots").forEach(button => {
    button.addEventListener("click", function (event) {
        event.stopPropagation();

        const card = this.closest(".card");
        const optionsModal = card.querySelector(".mini-modal");

        const isCurrentlyVisible = optionsModal.classList.contains('visible');

        document.querySelectorAll(".mini-modal").forEach(modal =>
            modal.classList.remove('visible')
        );

        if (!isCurrentlyVisible) {
            optionsModal.classList.add('visible');
        }
    });
});

document.querySelectorAll(".mini-modal button").forEach(button => {
    button.addEventListener("click", function (event) {
        event.stopPropagation();
        this.closest(".mini-modal").classList.remove('visible');
    });
});


//Close project options when clicking outside of project card (had som help from ChatGPT 4o here)
document.addEventListener("click", function(event) {
    document.querySelectorAll(".mini-modal").forEach(modal => {
        if (!event.target.closest(".project-card")) {
            modal.classList.remove('visible');
        }
    });
});


const previewSize = 150

//handle img previewer 
document.querySelectorAll('.image-previewer').forEach(previewer => {
    const fileInput = previewer.querySelector('input[type="file"]')
    const imagePreview = previewer.querySelector('.image-preview')

    previewer.addEventListener('click', () => fileInput.click())
    fileInput.addEventListener('change', ({ target: { files } }) => {
        const file = files[0]
        if (file)
            processImage(file, imagePreview, previewer, previewSize)
    });
});



async function loadImage(file) {
    return new Promise((resolve, reject) => { 
        const reader = new FileReader()

        reader.onerror = () => reject(new Error("Failed to load file"))
        reader.onload = (e) => {
            const img = new Image()
            img.onerror = () => reject(new Error("Failed to load image"))
            img.onload = () => resolve(img)
            img.src = e.target.result
        }
        reader.readAsDataURL(file)
    })
}

async function processImage(file, imagePreview, previewer, previewSize = 150) {
        try {
            const img = await loadImage(file)
            const canvas = document.createElement('canvas')
            canvas.width = previewSize
            canvas.height = previewSize

            const ctx = canvas.getContext('2d')
            ctx.drawImage(img, 0, 0, previewSize, previewSize)
            imagePreview.src = canvas.toDataURL('image/jpeg')
            previewer.classList.add('selected')
        }
        catch (error) {
            console.error('Failed on image processing:', error)
        }
}

function populateDateDropdowns(daySelect, monthSelect, yearSelect, selectedDay, selectedMonth, selectedYear) {
    if (!daySelect || !monthSelect || !yearSelect) return;

    while (daySelect.options.length > 1) {
        daySelect.remove(1);
    }
    while (monthSelect.options.length > 1) {
        monthSelect.remove(1);
    }
    while (yearSelect.options.length > 1) {
        yearSelect.remove(1);
    }

    // Populate Day (1-31)
    for (let i = 1; i <= 31; i++) {
        let option = new Option(i, i);
        daySelect.add(option);
    }

    // Populate Month (January - December)
    let monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    monthNames.forEach((month, index) => {
        let option = new Option(month, index + 1);
        monthSelect.add(option);
    });

    // Populate Year (From 1900 to Current Year)
    let currentYear = new Date().getFullYear();
    for (let i = currentYear; i >= 1900; i--) {
        let option = new Option(i, i);
        yearSelect.add(option);
    }
}

//Dropdowns
function closeAllDropdowns(exceptDropdown, dropdownElements) {
    dropdownElements.forEach(dropdown => {
        if (dropdown !== exceptDropdown) {
            dropdown.classList.remove('visible');
        }
    });
}
function initializeDropdowns() {
    const dropdownTriggers = document.querySelectorAll('[data-type="dropdown"]');
    const dropdownElements = new Set()
    dropdownTriggers.forEach(trigger => {
        const targetSelector = trigger.getAttribute('data-target');
        if (targetSelector) {
            const dropdown = document.querySelector(targetSelector);
            if (dropdown) {
                dropdownElements.add(dropdown)
            }
        }
    });

    dropdownTriggers.forEach(trigger => {
        trigger.addEventListener('click', (e) => {
            e.stopPropagation();
            const targetSelector = trigger.getAttribute('data-target');
            if (!targetSelector) return;
            const dropdown = document.querySelector(targetSelector);
            if (!dropdown) return;

            closeAllDropdowns(dropdown, dropdownElements)
            dropdown.classList.toggle('visible');
        });
    });

    dropdownElements.forEach(dropdown => {
        dropdown.addEventListener('click', (e) => {
            e.stopPropagation();
        })
    })

    document.addEventListener('click', () => {
        closeAllDropdowns(null, dropdownElements);
    });
}


//Had help with this from Claude AI
function initializeProjectFilters() {
    const startedProjects = document.getElementById('started-projects');
    const completedProjects = document.getElementById('completed-projects');
    const allProjects = document.getElementById('all-projects');

    allProjects.classList.add('active');

    startedProjects.addEventListener('click', function () {
        filterProjects('active');
        setActiveLink(startedProjects);
    });

    completedProjects.addEventListener('click', function () {
        filterProjects('completed');
        setActiveLink(completedProjects);
    });

    allProjects.addEventListener('click', function() {
        filterProjects('all');
        setActiveLink(allProjects);
    });

    const projectCards = document.querySelectorAll('.project-card');


    function setActiveLink(activeLink) {
        allProjects.classList.remove('active');
        startedProjects.classList.remove('active');
        completedProjects.classList.remove('active');

        activeLink.classList.add('active');
    }

    function filterProjects(status) {
        projectCards.forEach(card => {
            const statusId = card.getAttribute('data-status-id');
            if (status === 'all') {
                card.style.display = 'block';
            } else if (status === 'active' && statusId === '2') {
                card.style.display = 'block';
            } else if (status === 'completed' && statusId === '3') {
                card.style.display = 'block';
            }
            else {
                card.style.display = 'none';
            }
        });
    }
}

//Darkmode stuff 
const darkmodeSwitch = document.getElementById('dm-switch');
const hasSetDarkmode = localStorage.getItem('darkmode');

if (hasSetDarkmode == null) {
    if (window.matchMedia('prefers-color-scheme: dark)').matches) {
        enableDarkmode();
    } else {
        disableDarkmode();
    }
} else if (hasSetDarkmode === 'on') {
    enableDarkmode();
} else if (hasSetDarkmode === 'off') {
    disableDarkmode();
}

darkmodeSwitch.addEventListener('change', () => {
    if (darkmodeSwitch.checked) {
        document.documentElement.classList.add('darkmode');
        localStorage.setItem('darkmode', 'on');
    }
    else {
        document.documentElement.classList.remove('darkmode');
        localStorage.setItem('darkmode', 'off');
    }
})

function enableDarkmode() {
    darkmodeSwitch.checked = true;
    document.documentElement.classList.add('darkmode');
}

function disableDarkmode() {
    darkmodeSwitch.checked = false;
    document.documentElement.classList.remove('darkmode');
}
