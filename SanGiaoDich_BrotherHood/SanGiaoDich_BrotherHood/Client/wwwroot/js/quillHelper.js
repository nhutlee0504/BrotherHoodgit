window.showFileDialog = async () => {
    return new Promise((resolve) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.onchange = async (event) => {
            const file = event.target.files[0];
            if (file) {
                const fileData = {
                    name: file.name,
                    size: file.size,
                    type: file.type,
                };
                resolve(fileData);
            } else {
                resolve(null);
            }
        };
        input.click();
    });
};

window.initializeQuill = (editorId) => {
    const quill = new Quill(`#${editorId}`, {
        theme: 'snow',
        placeholder: 'Nhập mô tả chi tiết về sản phẩm...',
        modules: {
            toolbar: [
                [{ 'header': [1, 2, false] }],
                ['bold', 'italic', 'underline'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                ['link', 'image'],
                [{ 'align': [] }],
                ['clean']
            ]
        }
    });

    quill.on('text-change', function () {
        const content = quill.root.innerHTML;
        DotNet.invokeMethodAsync("YourNamespace", "UpdateDescription", content);
    });
};
