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

//Had som help here from Claude AI with debugging since some things weren't working as expected. 
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
            quill.root.innerHTML = content;
;
        } catch (error) {
            quill.setText(content);
        }
    }

    quill.on('text-change', () => {
        textArea.value = quill.root.innerHTML;
        console.log("Text changed, updated textarea");
    });
}