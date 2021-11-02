(function (ko) {
    ko.validation.init({
        insertMessages: false,
        decorateInputElement: true,
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block'
    });
}(require('ko')));