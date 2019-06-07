//Various utility functions

function CheckRequiredFields() {
    var blnCheckRequiredFields = true;
    //$("input.required").each(function () {
    //    $(this).removeClass('is-invalid');
    //    if ($(this).val().trim() == "" && $(this).is(':visible')) {
    //        $(this).addClass('is-invalid');
    //        $(this).data('placement', 'bottom');
    //        $(this).data('content', 'This is a required field.');
    //        $(this).popover('show');
    //        blnCheckRequiredFields = false;
    //    }
    //});
    return blnCheckRequiredFields;
}

var blnBBoxLoading = false;
var inptFocused = null;
function BBoxLoading() {
    blnBBoxLoading = true;
    //bootbox.dialog({ message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>', closeButton: false }).on('shown.bs.modal',
    //    function () {
    //        //If we've already set the close the loading box before it's finished opening, make sure it closes
    //        if (!blnBBoxLoading) BBoxLoadingClose();
    //    });
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

function IsEmail(strEmail) {
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    return filter.test(strEmail);
}

function msgConfirm(bvModal, title, msg, func) {
    func = func || "";

    bvModal
        .msgBoxConfirm(msg,
            {
                title: title,
                size: 'sm',
                buttonSize: 'sm',
                okVariant: 'danger',
                headerClass: 'p-2 border-bottom-0',
                footerClass: 'p-2 border-top-0',
                centered: true
            })
        .then(value => {
            if (value && func !== "") {
                func();
            }
        })
        .catch(err => {
            // An error occurred
        });
}
