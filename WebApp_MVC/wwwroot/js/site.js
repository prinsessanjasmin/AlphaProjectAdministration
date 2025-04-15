document.addEventListener('DOMContentLoaded', () => {

    const controller = document.body.dataset.controller;
    const sidebarProjects = document.querySelector('#sidebar-projects');
    const sidebarMembers = document.querySelector('#sidebar-members');
    const sidebarClients = document.querySelector('#sidebar-clients');

    [sidebarProjects, sidebarMembers, sidebarClients].forEach(navButton => navButton?.classList.remove('active-controller'));

    if (controller === "Project" && sidebarProjects) {
        sidebarProjects.classList.add('active-controller');
    } else if (controller === "Employee" && sidebarMembers) {
        sidebarMembers.classList.add('active-controller');
    } else if (controller === "Client" && sidebarClients) {
        sidebarClients.classList.add('active-controller');
    }

    sidebarProjects?.addEventListener('click', () => {
        window.location.href = '/projects';
    });

    sidebarClients?.addEventListener('click', () => {
        window.location.href = '/clients';
    });

    sidebarMembers?.addEventListener('click', () => {
        window.location.href = '/members';
    });


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
            const projectId = button.getAttribute('data-id');

            if (modalTarget === '#delete-modal' && projectId) {
                try {
                    const url = `/Project/ConfirmDelete?id=${projectId}`;

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
            else if (projectId) {
                const action = button.getAttribute('data-action');
                const controller = button.getAttribute('data-controller');

                if (!action || !controller) {
                    console.error("Missing asp-action or asp-controller attributes.");
                    return;
                }

                const url = `/${controller}/${action}?id=${projectId}`;

                try {
                    const response = await fetch(url);
                    if (!response.ok) throw new Error("Failed to load modal content");

                    const data = await response.text();
                    modal.querySelector('.modal-content').innerHTML = data;

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

                }
                catch (error) {
                    console.error("Error loading modal content:", error);
                    return;
                }
            }

            modal.style.display = 'flex';

        });
    });
});

document.addEventListener('click', (event) => {
    if (event.target.matches('[data-close="true"]')) {
        const modal = event.target.closest('.modal');
        if (modal) {
            modal.style.display = 'none';

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

        document.querySelectorAll(".mini-modal").forEach(modal => modal.style.display = "none");

        const projectCard = this.closest(".project-card");
        const optionsModal = projectCard.querySelector(".mini-modal");

        optionsModal.style.display = optionsModal.style.display === "flex" ? "none" : "flex";
    });
});

document.querySelectorAll(".mini-modal button").forEach(button => {
    button.addEventListener("click", function () {
        this.closest(".mini-modal").style.display = "none";
    });
});

//Close project options when clicking outside of project card (had som help from ChatGPT 4o here)
document.addEventListener("click", function(event) {
    document.querySelectorAll(".mini-modal").forEach(modal => {
        if (!event.target.closest(".project-card")) {
            modal.style.display = "none";
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