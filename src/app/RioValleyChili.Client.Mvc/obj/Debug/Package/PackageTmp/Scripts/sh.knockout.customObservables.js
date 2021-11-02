(function() {

    var isoFormat = /^(\d{4})-0?(\d+)-0?(\d+)[T ]0?(\d+):0?(\d+):0?(\d+)$/;

    if (require) {
        define(['ko'], defineExtensions);
    } else {
        defineExtensions(ko);
    }


    function defineExtensions(ko) {
        ko.formattedObservable = function (value, format) {
            var formatArg, initialValueArg;

            if (arguments.length > 1) {
                initialValueArg = value;
                formatArg = format;
            } else {
                initialValueArg = null;
                formatArg = arguments[0];
            }

            if (typeof formatArg !== "function") throw new Error("Must provide a format function.");

            var _private = ko.observable(applyFormatting(initialValueArg));
            var self = ko.computed({
                read: function () {
                    return _private();
                },
                write: function (val) {
                    _private(applyFormatting(val));
                }
            });
            return self;

            function applyFormatting(val) {
                return formatArg(val);
            }
        };

        ko.observableDateTime = function (value, format) {
            var _private = ko.observable().extend({ isoDate: format });
            var self = ko.computed({
                read: function () {
                    return _private();
                },
                write: function (val) {
                    _private(parseValue(val));
                },
                owner: _private,
            });
            self.formattedDate = ko.computed({
                read: function () {
                    return _private.formattedDate();
                },
                write: function (val) {
                    _private.formattedDate(parseValue(val));
                }
            });
            self.formattedDate(value);
            return self;

            function parseValue(input) {
                if (!input) return null;
                if (input instanceof Date) return input.getTime ? input.toISOString() : null;
                if (isoFormat.test(input)) return input;
                return new Date(Date.parse(input)).toISOString();
            }
        };
        ko.observableTime = function (value, format) {
            //todo: handle millisecond precision on isoPattern
            var isoPattern = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/;
            var timePattern = /([0-2]?[1-9]):(\d{2})(:\d{2})?/g;
            format = format || 'HH:MM';
            var _private = ko.observable().extend({ isoDate: format });
            var self = ko.computed({
                read: function () { return _private.formattedDate(); },
                write: function (val) {
                    if (val && val.trim) val.trim();
                    if (!val) {
                        _private(null);
                        return;
                    }

                    var timeValue = "00:00";
                    if (val instanceof Date) {
                        timeValue = val.getTime ? val.format(format) : timeValue;
                    } else if (val.match(isoPattern)) {
                        var d = new Date(Date.parse(val));
                        timeValue = d.getTime ? d.format(format) : timeValue;
                    }
                    else if (val.match(timePattern)) {
                        var matches = val.match(timePattern);
                        timeValue = val.match(timePattern)[matches.length - 1];
                    } else if (val.match(/^\d{3,4}$/)) {
                        timeValue = val.replace(/(\d{1,2})(\d{2})/, "$1:$2");
                        if (timeValue.length == 4) timeValue = "0" + timeValue;
                    }
                    else timeValue = val;

                    var date = new Date();
                    _private.formattedDate(date.format('mm/dd/yyyy') + " " + timeValue);
                }
            });
            self(value);
            return self;
        };

        ko.observableDate = function (value, format) {
            ko.observableDate.defaultFormat = 'm/d/yyyy';
            var _private = ko.observable().extend({ isoDate: format || ko.observableDate.defaultFormat });
            var self = ko.computed({
                read: function () {
                    return _private.formattedDate();
                },
                write: function (val) {
                    _private.formattedDate(parseValueWithoutTime(val));
                },
                owner: _private,
            });
            self(value);
            return self;

            function parseValueWithoutTime(input) {
                if (!input) return null;
                if (input instanceof Date) return input.getTime ? input.toISOString() : null;
                if (typeof input === "string") {
                    // Dates are automatically converted to UTC in JavaScript. 
                    // In order to preserve the input date, we must include the TimeZone offset. 
                    // For example, 2014-04-06 will be Sat Apr 05 2104 18:00:00 GMT-0600 (Mountain Daylight Time) when parsed.
                    // By adding the timezone offset value, we are recapturing the desired date of April 6, 2014.
                    var dt = new Date(Date.parse(input));
                    dt.addMinutes(dt.getTimezoneOffset());
                    return dt.toISOString();
                }
                return input;
            }
        };

        ko.numericObservable = function (value, precision) {
            var _private = ko.observable(formatValue(value, precision)),
                _lastVal = value,
                numExp = /^(-|\+)?[0-9]*(\.)?([0-9]+)?$/;
            var result = ko.computed({
                read: function () {
                    return _private();
                },
                write: function (val) {
                    if (val == undefined || (typeof val !== "string" && typeof val !== "number")) {
                        _private(null);
                        return;
                    }

                    var matches = val.toString().match(numExp);
                    if (matches) {
                        // if update trigger is key-based event (such as onkeydown), don't update 
                        // the value if the decimal is the last entry.
                        if (matches[1] + matches[2] === '.' && matches[3] === undefined) {
                            _lastVal = val;
                            return;
                        }

                        var formattedValue = formatValue(val, precision);
                        _lastVal = formattedValue;
                        _private(formattedValue);
                    }
                    
                    _private.notifySubscribers(_lastVal);
                }
            });
            result._private = _private;
            result._decimalPrecision = parseInt(precision);

            return result;

            function formatValue(input, decimalPrecision) {
                var numVal = parseFloat(input);
                if (isNaN(numVal)) return undefined;
                else return parseFloat(isNaN(decimalPrecision) ? numVal : numVal.toFixed(decimalPrecision));
            }
        };        
    }
}());