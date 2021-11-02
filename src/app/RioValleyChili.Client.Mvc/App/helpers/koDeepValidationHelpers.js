(function (ko) {
    ko.validation.init({
        insertMessages: false,
        decorateInputElement: true,
        grouping: {
          deep: true,
          live: true,
          observable: true
        },
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block'
    });
}(require('ko')));
