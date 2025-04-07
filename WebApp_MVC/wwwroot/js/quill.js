document.addEventListener('DOMContentLoaded', function () {
    initWysiwyg('.hidden-textarea', '@Html.Raw(ViewBag.Description ?? "")')
})


function initWysiwyg(textareaclass, content) {
    const textArea = document.querySelector(textareaclass)
    
    const quill = new Quill('#wysiwyg-editor', {
        modules: {
            syntax: true,
            toolbar: '#wysiwyg-toolbar'
        },
        placeholder: 'Type something',
        theme: 'snow'
    })

    if (content)
        quill.root.innerHtml = content; 

    quill.on('text-change', () => {
        textArea.value = quill.root.innerHtml;
    })
}