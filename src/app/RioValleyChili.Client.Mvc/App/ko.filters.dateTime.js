var ko = require('ko');
ko.filters.dateTime = function (input, format) {
    var value = input;
    if (typeof value === "string") {
        value = new Date(Date.parse(value));
    }

    if (!(value instanceof Date)) {
        throw new Error('Invalid input. Expected date but encountered ' + (typeof input) + '.');
    }
    return value.format(format || 'm/d/yyyy h:M TT')
}