function notify(title, type, message) {
    toastr.remove();
    toastr.options = {
        closeButton: false,
        progressBar: true,
        showMethod: "slideDown"
    };

    switch (type) {
        case "success":
            toastr.success(message, title);
            break;
        case "warning":
            toastr.warning(message, title);
            break;
        case "info":
            toastr.info(message, title);
            break;
        case "error":
            toastr.error(message, title);
            break;
    }

}

