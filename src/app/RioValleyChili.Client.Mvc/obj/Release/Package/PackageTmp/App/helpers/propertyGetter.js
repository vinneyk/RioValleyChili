define(function() {
    var arrayIndexRegex = /^(\w+)?\[(\d+)\]\.?(.*)/;

    //Object.prototype.deepSearch = function(propertyName) {
    //    return getValueFromPropertyName(this, propertyName);
    //}

    return {
        getValue: getValueFromPropertyName
    }

    function getValueFromPropertyName(obj, propName) {
        if (obj == undefined) return undefined;
        var matches = arrayIndexRegex.exec(propName);
        if (matches && matches.length) {
            var index = parseInt(matches[2]);
            obj = matches[1]
                ? obj[matches[1]][index]
                : obj[index];

            return matches[3]
                ? getValueFromPropertyName(obj, matches[3])
                : obj;
        }

        var paths = propName.split('.');
        if (paths.length > 1) {
            var p = paths.shift();
            return getValueFromPropertyName(obj[p], paths.join('.'));
        }

        return obj[propName];
    }
})
