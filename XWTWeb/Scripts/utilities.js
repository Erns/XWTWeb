//Various utility functions

function CheckRequiredFields() {
    var blnCheckRequiredFields = true;
    $("input.required").each(function () {
        $(this).removeClass('is-invalid');
        if ($(this).val().trim() == "" && $(this).is(':visible')) {
            $(this).addClass('is-invalid');
            $(this).data('placement', 'bottom');
            $(this).data('content', 'This is a required field.');
            $(this).popover('show');
            blnCheckRequiredFields = false;
        }
    });
    return blnCheckRequiredFields;
}

function BBoxLoading() {
    bootbox.dialog({ message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>', closeButton: false });
}