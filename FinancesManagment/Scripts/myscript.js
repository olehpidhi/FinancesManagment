function createConfirm(onConfirm) {
    $.confirm({
        content: "",
        title: "Are you sure?",
        confirm: onConfirm,
        cancel: function () {
        },
        confirmButton: 'Yes',
        cancelButton: 'No'
    });
}

function createAlert(text, onConfirm) {
    $.alert({
        content: "",
        title: text,
        confirm: onConfirm || function () { },
        confirmButton: 'Ok'
    });
}