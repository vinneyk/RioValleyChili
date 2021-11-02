//******************************
// NOTE: Replaced by sh.core.js
//******************************

// NOTE: use [Number].toLocaleString({style: 'currency'}) instead - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/toLocaleString#Browser_Compatibility
Number.prototype.formatMoney = function (decimalPercision, decimalCharacter, periodCharacter) {
    return s + "$" + this.addFormatting(decimalPercision, decimalCharacter, periodCharacter);
};


// NOTE: use [Number].toLocaleString() instead - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/toLocaleString#Browser_Compatibility
Number.prototype.addFormatting = function (decimalPercision, decimalCharacter, periodCharacter) {
    //http://stackoverflow.com/questions/149055/how-can-i-format-numbers-as-money-in-javascript

    decimalPercision = isNaN(decimalPercision = Math.abs(decimalPercision)) ? 0 : decimalPercision;
    decimalCharacter = decimalCharacter == undefined ? "." : decimalCharacter;
    periodCharacter = periodCharacter == undefined ? "," : periodCharacter;
    
    var num = this,
        sign = num < 0 ? "-" : "",
        numWithPercision = parseInt(num = Math.abs(+num || 0).toFixed(decimalPercision)) + "",
        countOfPeriods = (countOfPeriods = numWithPercision.length) > 3 ? countOfPeriods % 3 : 0;

    return sign + (countOfPeriods ? numWithPercision.substr(0, countOfPeriods) + periodCharacter : "") + numWithPercision.substr(countOfPeriods).replace(/(\d{3})(?=\d)/g, "$1" + periodCharacter) + (decimalPercision ? decimalCharacter + Math.abs(num - numWithPercision).toFixed(decimalPercision).slice(2) : "");
};

String.prototype.pluralize = function (count) {
    var noun = this;

    if (!typeof count == "number") { count = 2; }
    else { count = Math.abs(count); }

    if (count == 1 || noun.match(/s$/i)) { return noun; }

    var regex = /y$/i;
    if (noun.match(regex)) return noun.replace(regex, "ies");

    regex = /[sh|ss|s|z|x|ch]$/i;
    if (noun.match(regex)) return noun + "es";

    regex = /[a|e|i|o|u|y]o$/i;
    if (noun.match(regex)) return noun + "s";

    regex = /o$/i;
    if (noun.match(regex)) return noun + "es";

    regex = /[^f][fe|f]$/i;
    if (noun.match(regex)) return noun.replace(/[fe|f]$/i, "ves");

    return noun + "s";
};

//#region Array prototype functions
Array.prototype.isLastElement = function (element) {
    var lastIndex = this.length - 1;
    return this[lastIndex] === element;
};
Array.prototype.filterNonNull = function () {
    return ko.utils.arrayFilter(this, function (item) { return item !== null; });
};
Array.prototype.clone = function () {
    return this.slice(0);
};
Array.prototype.sortNum = function () {
    return this.sort(function (a, b) {
        return a - b;
    });
};
Array.prototype.sortDesc = function () {
    return this.sort(function (a, b) {
        return b - a;
    });
};
Array.prototype.pushAllWithoutDuplicates = function (items, keySelectorPredicate) {
    var hold = this.toObj(keySelectorPredicate);
    var itemsAdded = [];
    for (var i = 0; i < items.length; i++) {
        var currentItem = items[i];
        var keyVal = keySelectorPredicate.call(currentItem, currentItem);
        if (!hold[keyVal]) {
            this.push(currentItem);
            hold[keyVal] = true;
            itemsAdded.push(currentItem);
        }
    }
    return itemsAdded;
};
Array.prototype.toObj = function (keyPredicate, valuePredicate) {
    var obj = {};
    for (var i = 0; i < this.length; i++) {
        var currentItem = this[i];
        valuePredicate = valuePredicate || function () { return currentItem; };
        obj[keyPredicate.call(currentItem, currentItem)] = valuePredicate.call(currentItem, currentItem);
    }
    return obj;
};
//#endregion