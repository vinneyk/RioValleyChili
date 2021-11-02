define(['ko', 'app', './koBindings', 'scripts/knockout.validation', 'scripts/ko.extenders.date', 'App/scripts/ko.extenders.lotKey'], function(ko, app) {
    ko.extenders.timeEntry = function (target) {
        var pattern = /^(\d)?(\d)?:?(\d)?(\d)?$/;
        target.formattedTime = ko.computed({
            read: function () {
                if (!target()) return '00:00';
                var val = target();
                var formatted = val;
                switch (val.length) {
                    case 1:
                        formatted = val.replace(pattern, "0$1");
                    case 2:
                        formatted += ":00";
                        break;
                    case 3:
                        formatted = val.replace(pattern, "0$1:$2$3");
                        break;
                    case 4:
                        formatted = val.replace(pattern, "$1$2:$3$4");
                        break;
                }
                return formatted;
            },
            write: function (value) {
                if (typeof value === "string") {
                    var parsed = Date.parse(value);
                    if (parsed) {
                        var d = new Date(parsed);
                        var hours = d.getHours();
                        hours = hours < 10 ? ('0' + hours) : hours;
                        var minutes = d.getMinutes();
                        minutes = minutes < 10 ? ('0' + minutes) : minutes;
                        value = hours + ":" + minutes;
                    }
                }
                target(value);
            }
        });

        target.extend({ pattern: { message: "Invalid Date", params: /^([01]\d|2[0-3]):?([0-5]\d)$/ } });
        target.Hours = ko.computed(function () {
            if (!target.formattedTime()) return 0;
            return target.formattedTime().split(":")[0];
        });
        target.Mins = ko.computed(function () {
            if (!target.formattedTime()) return 0;
            return target.formattedTime().split(":")[1];
        });
        target.formattedTime(target());
        return target;
    };

    ko.extenders.toteKey = function (target, callback) {
        var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{4})?$/;
        var isComplete = ko.observable(false);
        target.formattedTote = ko.computed({
            read: function () {
                var value = target();
                return formatTote(value);
            },
            write: function (input) {
                var value = cleanInput(input);
                if (target() === value) return;

                target(value);
                if (value && value.match(pattern)) {
                    var formatted = formatTote(value);
                    if (formatted.length === 10) {
                        isComplete(true);
                        if (typeof callback === "function") callback(formatted);
                    }
                }
            },
        });
        target.isComplete = ko.computed(function () {
            return isComplete();
        });
        target.getNextTote = function () {
            var formatted = target.formattedTote();
            var sequence = parseSequence();
            if (isNaN(sequence)) return null;
            sequence++;

            var sequenceString = formatSequence();
            return formatted.replace(pattern, '0$2 $4 ' + sequenceString);

            function parseSequence() {
                var sections = formatted.split(" ");
                if (sections.length !== 3) return null;
                return parseInt(sections[2]);
            }
            function formatSequence() {
                var val = sequence.toString();
                while (val.length < 4) {
                    val = "0" + val;
                }
                return val;
            }
        };
        target.isMatch = function (val) {
            var formattedVal = formatTote(ko.utils.unwrapObservable(val));
            if (!formattedVal) return false;
            var p = new RegExp("^" + target.formattedTote() + "$");
            return formattedVal.match(p);
        };

        target.extend({ throttle: 800 });
        target.formattedTote(target());
        return target;

        function formatTote(input) {
            if (input == undefined) return '';
            if (!input.match(pattern)) return input;
            input = input.trim();
            return input.replace(pattern, '0$2 $4 $6').trim().replace("  ", " ");
        }
        function cleanInput(input) {
            if (typeof input == "number") input = input.toString();
            if (typeof input !== "string") return undefined;
            return input.replace(/\s/g, '');
        }
    };
    
    ko.extenders.contractType = function (target) {
        return new TypeExtension(target, app.lists.contractTypes.toDictionary());
    };

    ko.extenders.contractStatus = function (target) {
        return new TypeExtension(target, app.lists.contractStatuses.toDictionary());
    };

    ko.extenders.defectResolutionType = function (target) {
        return new TypeExtension(target, app.lists.defectResolutionTypes.toDictionary());
    };

    ko.extenders.defectType = function (target) {
        return new TypeExtension(target, app.lists.defectTypes.toDictionary());
    };
    
    ko.extenders.facilityType = function (target) {
        return new TypeExtension(target, app.lists.facilityTypes.toDictionary());
    };

    ko.extenders.inventoryType = function (target) {
        return new TypeExtension(target, app.lists.inventoryTypes.toObjectDictionary());
    };

    ko.extenders.productType = function (target) {
      var extension = new TypeExtension(target, app.lists.inventoryTypes.toDictionary());
      extension.trueValue = ko.pureComputed(function() {
        var raw = target();
        if (raw == null) return null;
        return parseInt(raw);
      });
      return extension;
    };

    ko.extenders.locationStatusType = function (target) {
        return new TypeExtension(target, app.lists.locationStatusTypes.toDictionary());
    }

    ko.extenders.lotHoldType = function (target) {
        return new TypeExtension(target, app.lists.lotHoldTypes.toDictionary());
    };

    ko.extenders.lotQualityStatusType = function (target) {
        return new TypeExtension(target, app.lists.lotQualityStatusTypes.toDictionary());
    };

    ko.extenders.lotType = function (target) {
        return new TypeExtension(target, app.lists.lotTypes.toDictionary());
    };

    ko.extenders.lotType2 = function (target) {
        return new TypeExtension(target, app.lists.lotTypes.toObjectDictionary());
    };

    ko.extenders.productionStatusType = function (target) {
        return new TypeExtension(target, app.lists.productionStatusTypes.toDictionary());
    };

    ko.extenders.chileType = function (target) {
        var options = {
            0: 'Other Raw',
            1: 'Dehydrated',
            2: 'WIP',
            3: 'Finished Goods'
        };
        return new TypeExtension(target, options);
    };

    ko.extenders.treatmentType = function (target) {
        return new TypeExtension(target, app.lists.treatmentTypes.toDictionary());
    };

    ko.extenders.shipmentStatusType = function (target) {
        return new TypeExtension(target, app.lists.shipmentStatus.toDictionary());
    };
    ko.extenders.orderStatusType = function (target) {
        return new TypeExtension(target, app.lists.orderStatus.toDictionary());
    };
    ko.extenders.customerOrderStatusType = function (target) {
        return new TypeExtension(target, app.lists.customerOrderStatus.toDictionary());
    };

    ko.extenders.movementTypes = function (target) {
        var options = {
            0: 'Same Warehouse',
            1: 'Between Warehouses',
        };
        return new TypeExtension(target, options);
    };

    ko.extenders.inventoryOrderTypes = function (target, defaultOption) {
        return new TypeExtension(target, app.lists.inventoryOrderTypes, defaultOption);
    };

    // Data input binding extension. Converts input to numeric values.
    ko.extenders.numeric = function (target, precision) {
        console.warn('Replace numeric binding extender with numericObservable object');
        var mode = 'readonly', isWriteable = false;
        if (!ko.isWriteableObservable(target)) {
            mode = 'writeable';
            isWriteable = true;
            //throw new Error('Object must be a writableObservable in order to be used with the numeric binding. For read-only binding, use formatNumber instead.');
        }

        target.numericMode = mode;
        if (isWriteable) return writable();
        else return readonly();

        function writable() {
            applyFormatting(target());
            target.subscribe(applyFormatting, target);
            return target;

            function applyFormatting(value) {
                value = formatValue(value);
                if (value === target()) return;
                setValue(value);
            }
            function setValue(value) {
                target(value);
            }
        }

        function readonly() {
            target.formattedNumber = ko.computed({
                read: function () {
                    return formatValue(target()) || undefined;
                },
                write: function (val) {
                    target(formatValue(val) || undefined);
                }
            }, target);
            return target;
        }

        function formatValue(input) {
            var numVal = parseFloat(input);
            if (isNaN(numVal)) return undefined;
            else return parseFloat(numVal.toFixed(precision));
        }
    };

    //Read-only binding for displaying numeric values with a specific decimal precision.
    //For numeric input bindings, use the numeric binding instead.
    ko.extenders.formatNumber = function (target, precision) {
        function formatValue(input) {
            precision = parseInt(precision) || 0;
            return precision > 0 ? parseFloat(input).toFixed(precision) : parseInt(input);
        }

        target.formattedNumber = ko.computed(function () {
            return formatValue(target()) || 0;
        }, target);
        return target;
    };

    //******************************
    // MAPPING HELPERS

    ko.mappings = ko.mappings || {};
    ko.mappings.formattedDate = function (options, format) {
        var dateString = options.data;
        var date = null;
        if (typeof dateString == "string" && dateString.length > 0) {
            if (dateString.match(/^\/Date\(\d*\)\/$/)) {
                dateString = dateString.replace(/[^0-9 +]/g, '');
                dateString = parseInt(dateString);
            }
            date = new Date(dateString).toISOString();
        }
        var result = ko.observable(date).extend({ isoDate: format || 'm/d/yyyy' });
        return result;
    };

    //****************************************
    // validation rules
    ko.validation.rules['isUnique'] = {
        validator: function (newVal, options) {
            if (options.predicate && typeof options.predicate !== "function")
                throw new Error("Invalid option for isUnique validator. The 'predicate' option must be a function.");

            var array = options.array || options;
            var count = 0;
            ko.utils.arrayMap(ko.utils.unwrapObservable(array), function (existingVal) {
                if (equalityDelegate()(existingVal, newVal)) count++;
            });
            return count < 2;

            function equalityDelegate() {
                return options.predicate ? options.predicate : function (v1, v2) { return v1 === v2; };
            }
        },
        message: 'This value is a duplicate',
    };

    /*
     * Determines if a field is required or not based on a function or value
     * Parameter: boolean function, or boolean value
     * Example
     *
     * viewModel = {
     *   var vm = this;     
     *   vm.isRequired = ko.observable(false);
     *   vm.requiredField = ko.observable().extend({ conditional_required: vm.isRequired});
     * }   
    */
    ko.validation.rules['conditional_required'] = {
        validator: function (val, condition) {
            var required;
            if (typeof condition == 'function') {
                required = condition();
            } else {
                required = condition;
            }

            if (required) {
                return !(val == undefined || val.length == 0);
            } else {
                return true;
            }
        },
        message: "Field is required"
    };

    ko.validation.rules['doesNotEqual'] = {
        validator: function (v1, v2) {
            ko.validation.rules['doesNotEqual'].message = "\"" + v1 + "\" is not valid.";
            return v1 !== v2;
        },
    };

    ko.validation.rules['isValidTreatment'] = {
        validator: function (val) {
            return val !== app.lists.treatmentTypes.NotTreated.key
                && val !== app.lists.treatmentTypes.LowBac.key;
        },
        message: "Invalid Treatment"
    };

    ko.validation.rules['isTrue'] = {
        validator: function (value, fnInvalid) {
            return fnInvalid.apply(value) === true;
        },
        message: "The new location is the same as the previous location. There is no need to create a movement if the items don't change location.",
    };

    ko.validation.registerExtenders();


    //******************************************
    // private functions

    function TypeExtension(target, options, defaultOption) {
        if (defaultOption === undefined && options.length) defaultOption = options[0];
        target.displayValue = ko.computed({
            read: function () {
                if (target() == undefined) return '';
                return getTypeOption(target()) || defaultOption;
            }
        });
        target.options = buildSelectListOptions(options);
        return target;

        function buildSelectListOptions(source) {
            var selectListOptions = [];
            for (var opt in source) {
                selectListOptions.push({
                    key: opt,
                    value: source[opt]
                });
            }
            return selectListOptions;
        }
        function getTypeOption(val) {
            switch (typeof val) {
                case "string": return fromString(val);
                case "number": return fromNumber(val);
                case "object": return fromObject(val);
                default: return undefined;
            }

            function fromString(s) {
                return fromNumber(parseInt(s))
                    || findOptionByName();

                function findOptionByName() {
                    for (var prop in options) {
                        if (options[prop] === s) {
                            return fromString(prop);
                        }
                    }
                    return undefined;
                }
            }
            function fromNumber(num) {
                if (isNaN(num)) return undefined;
                return options[num + ''];
            }
            function fromObject(o) {
                if (!o || o.value == undefined) return undefined;
                return o.value;
            }
        }
    }

});
