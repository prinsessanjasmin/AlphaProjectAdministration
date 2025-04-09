document.addEventListener('DOMContentLoaded', function () {
    initAddProjectQuill();


    const modalButtons = document.querySelectorAll('[data-modal="true"]');
    modalButtons.forEach(button => {
        if (button.getAttribute('data-target') === '#editProjectModal') {
            button.addEventListener('click', function () {
                setTimeout(initEditProjectQuill, 200);
            });
        }
    });
});

function initAddProjectQuill() {
    

    const textArea = document.querySelector('#addProjectForm .hidden-textarea'); 
    if (!textArea) {
        return;
    }

    const quill = new Quill('#addProjectForm #wysiwyg-editor', {
        modules: {
            syntax: true,
            toolbar: '#addProjectForm #wysiwyg-toolbar'
        },
        placeholder: 'Type something',
        theme: 'snow'
    });

    quill.on('text-change', () => {
        textArea.value = quill.root.innerHTML;
    });
}

function initEditProjectQuill() {

    console.log("Initializing Edit Project Quill...");
    const textArea = document.querySelector('#editProjectForm .hidden-textarea');
    if (!textArea) {
        console.error("Text area not found in edit modal");
        return;
    }

    const content = textArea.value;
    console.log("Textarea content:", content);

    const quill = new Quill('#editProjectForm #wysiwyg-editor', {
        modules: {
            syntax: true,
            toolbar: '#editProjectForm #wysiwyg-toolbar'
        },
        placeholder: 'Type something',
        theme: 'snow'
    });

    if (content && content.trim() !== '') {
        try {
            // Try setting as HTML first
            quill.root.innerHTML = content;
            console.log("Set content as HTML");
        } catch (error) {
            console.error("Error setting HTML content:", error);
            // Fallback to setting as text
            quill.setText(content);
            console.log("Set content as text");
        }
    }

    quill.on('text-change', () => {
        textArea.value = quill.root.innerHTML;
        console.log("Text changed, updated textarea");
    });
}