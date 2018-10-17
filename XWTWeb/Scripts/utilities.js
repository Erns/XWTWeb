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

var blnBBoxLoading = false;
var inptFocused = null;
function BBoxLoading() {
    blnBBoxLoading = true;
    bootbox.dialog({ message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>', closeButton: false }).on('shown.bs.modal',
        function () {
            //If we've already set the close the loading box before it's finished opening, make sure it closes
            if (!blnBBoxLoading) BBoxLoadingClose();
        });
}

function BBoxLoadingClose() {
    blnBBoxLoading = false;
    bootbox.hideAll();
}

function SortSelect(select) {
    select.html(select.find('option').sort(function (x, y) {
        // to change to descending order switch "<" for ">"
        return $(x).text() > $(y).text() ? 1 : -1;
    }));
}