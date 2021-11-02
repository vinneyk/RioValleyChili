define(['ko'], function(ko) {
    Array.prototype.filterNonNull = function() {
        return ko.utils.arrayFilter(this, function(item) { return item !== null; });
    };
    Array.prototype.clone = function() {
        return this.slice(0);
    };
    Array.prototype.copy = function() {
        return JSON.parse(JSON.stringify(this));
    };
    Array.prototype.sortNum = function() {
        return this.sort(function(a, b) {
            return a - b;
        });
    };
    Array.prototype.sortDesc = function() {
        return this.sort(function(a, b) {
            return b - a;
        });
    };
    Array.prototype.pushAllWithoutDuplicates = function(items, keySelectorPredicate) {
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
    Array.prototype.toObj = function(keyPredicate, valuePredicate) {
        var obj = {};
        for (var i = 0; i < this.length; i++) {
            var currentItem = this[i];
            valuePredicate = valuePredicate || function() { return currentItem; };
            obj[keyPredicate.call(currentItem, currentItem)] = valuePredicate.call(currentItem, currentItem);
        }
        return obj;
    };

    // work around for lack of ability to load CSS in require.js (11/20/2014 vk)
    document.insertStyleIntoHead = function (style) {
        var styleElem = document.createElement('style');
        styleElem.innerHTML = style;
        document.getElementsByTagName("head")[0].appendChild(styleElem);
    }
});