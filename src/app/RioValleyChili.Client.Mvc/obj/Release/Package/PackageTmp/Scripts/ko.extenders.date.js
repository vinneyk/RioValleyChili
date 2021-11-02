﻿/* http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/ 
 * Knockout extender for dates that are round-tripped in ISO 8601 format
 *  Depends on knockout.js and date.format.js 
 *  Includes extensions for the date object that: 
 *      add Date.toISOString() for browsers that do not nativly implement it
 *      replaces Date.parse() with version to supports ISO 8601 (IE and Safari do not)
 *  Includes example of how to use the extended binding
 */

(function () {
    if (require) {
        define(['ko'], extendKo);
    } else {
        extendKo(ko);
    }
    
    function extendKo(ko) {
        ko.extenders.isoDate = function (target, formatString) {
            target.formattedDate = ko.computed({
                read: function () {
                    if (!target()) {
                        return;
                    }
                    var dt = new Date(Date.parse(target()));
                    // Modified to not change to UTC. Should not effect 
                    // anything NOT using time formatting.
                    return dt.format(formatString);
                    //return dt.format(formatString, true);
                },
                write: function (value) {
                    // Modified from original to enable setting value to null
                    // previously, when the value was "falsey", assignment was 
                    // skipped all together. -- VK 5/27/13
                    if (!value) {
                        target(null);
                    } else {
                        target(new Date(Date.parse(value)).toISOString());
                    }
                }
            });

            target.asDate = ko.computed(function () {
                return new Date(target.formattedDate());
            });

            //initialize with current value
            target.formattedDate(target());

            //return the computed observable
            return target;
        };
    }
}());


/** from the mozilla documentation (before they implemented the function in the browser)
 * https://developer.mozilla.org/index.php?title=en/JavaScript/Reference/Global_Objects/Date&revision=65
 */
(function(Date) {
    if (!Date.prototype.toISOString) {
        Date.prototype.toISOString = function() {
            function pad(n) {
                return n < 10 ? '0' + n : n;
            }
            return this.getUTCFullYear() + '-' + pad(this.getUTCMonth() + 1) + '-' + pad(this.getUTCDate()) + 'T' + pad(this.getUTCHours()) + ':' + pad(this.getUTCMinutes()) + ':' + pad(this.getUTCSeconds()) + 'Z';
        };
    }
}(Date));

/**
 * Date.parse with progressive enhancement for ISO 8601 <https://github.com/csnover/js-iso8601>
 * © 2011 Colin Snover <http://zetafleet.com>
 * Released under MIT license.
 */
(function(Date) {
    var origParse = Date.parse,
        numericKeys = [1, 4, 5, 6, 7, 10, 11];
    Date.parse = function(date) {
        var timestamp, struct, minutesOffset = 0;

        // ES5 §15.9.4.2 states that the string should attempt to be parsed as a Date Time String Format string
        // before falling back to any implementation-specific date parsing, so that’s what we do, even if native
        // implementations could be faster
        //              1 YYYY                2 MM       3 DD           4 HH    5 mm       6 ss        7 msec        8 Z 9 ±    10 tzHH    11 tzmm
        if ((struct = /^(\d{4}|[+\-]\d{6})(?:-(\d{2})(?:-(\d{2}))?)?(?:T(\d{2}):(\d{2})(?::(\d{2})(?:\.(\d{3}))?)?(?:(Z)|([+\-])(\d{2})(?::(\d{2}))?)?)?$/.exec(date))) {
            // avoid NaN timestamps caused by “undefined” values being passed to Date.UTC
            for (var i = 0, k;
            (k = numericKeys[i]); ++i) {
                struct[k] = +struct[k] || 0;
            }

            // allow undefined days and months
            struct[2] = (+struct[2] || 1) - 1;
            struct[3] = +struct[3] || 1;

            if (struct[8] !== 'Z' && struct[9] !== 'undefined') {
                minutesOffset = struct[10] * 60 + struct[11];

                if (struct[9] === '+') {
                    minutesOffset = 0 - minutesOffset;
                }
            }

            timestamp = Date.UTC(struct[1], struct[2], struct[3], struct[4], struct[5] + minutesOffset, struct[6], struct[7]);
        }
        else {
            timestamp = origParse ? origParse(date) : NaN;
        }

        return timestamp;
    };
}(Date));