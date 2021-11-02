define(['jquery', 'ko'], function ($, ko) {

    function Cursor(options) {
        if (!(this instanceof Cursor)) { return new Cursor(options); }

        var self = this;

        $.extend(self, Cursor.prototype.DEFAULTS, options);

        return self;
    }

    Cursor.prototype.DEFAULTS = {
        skipCount: 0,
        pageSize: 50,
    };

    Cursor.prototype.incrementSkipCount = function (count) {
        var increment = 1;
        if (count != undefined) {
            increment = parseInt(count);
            if (isNaN(increment)) {
                throw new Error("The argument could not be parsed. Expected value undefined or number.");
            }
        }
        this.skipCount += increment;
    };

    return {
        init: init
    }
    function init(options) {
        var settings = typeof options === "string"
            ? { urlBase: options }
            : options || {},
            parameters = settings.parameters || {},
            newPageSetCallbacks = [];
        
        var cursor = new Cursor(settings);
        var self = {
            allDataLoaded: ko.observable(false),
            urlBase: buildUrl(settings.urlBase, parameters),
        };
        self.nextPage = function (args) {
            var opts = args == undefined ? settings : $.extend({}, settings, args);
            return getNextPage.call(self, settings.urlBase, cursor, parameters, opts);
        };
        self.GetNextPage = self.nextPage;
        self.resetCursor = function () {
            self.urlBase = undefined;
        }

        self.addParameter = addParameterPublic;
        self.addParameters = addParametersPublic;
        self.removeParameter = removeParameterPublic;
        self.containsParameter = containsParameter.bind(parameters);
        self.notifyNewPageSetSubscribers = function() {
            publishNotificationsPublic.call(newPageSetCallbacks);
        }
        
        self.removeNewPageSetCallback = removeNewPageSetCallbackPublic;
        self.addNewPageSetCallback = addNewPageSetCallbackPublic;
        self.incrementSkipCount = cursor.incrementSkipCount;

        self.callbackOptions = settings.callbackOptions || settings;
        
        if (settings.onNewPageSet && typeof settings.onNewPageSet === "function") {
            self.addNewPageSetCallback(settings.onNewPageSet);
        }

        return self;


        function containsParameter(name) {
            return parameters.hasOwnProperty(name);
        }
        function addParameterPublic(name, value) {
            parameters[name] = value;
        }
        function addParametersPublic(params, replaceDuplicates) {
            if (!params) return;
            if (!isObject(params)) throw new Error("Parameter type \"" + typeof params + "\" is not supported.");
            for (var p in params) {
                if (!params.hasOwnProperty(p)) continue;
                if (replaceDuplicates === true || !containsParameter(p)) {
                    addParameterPublic(p, params[p]);
                }
            }

            function isObject(o) {
                return typeof (o) === "object" && o != undefined;
            }
        }
        function removeParameterPublic(name) {
            delete parameters[name];
        }
        function addNewPageSetCallbackPublic(callback) {
            if (!(typeof callback === "function")) throw new Error("Invalid argument: callback. Expected function.");
            findCallback(callback, function (item) {
                if (!item) newPageSetCallbacks.push(callback);
            });
        }
        function removeNewPageSetCallbackPublic(callback) {
            if (!(typeof callback === "function")) throw new Error("Invalid argument: callback. Expected function.");
            findCallback(callback, function (item, index) {
                if (item) {
                    newPageSetCallbacks.splice(index, 1);
                    return;
                }
            });
        }
        function findCallback(callbackToFind, callback) {
            var index = -1, existing;
            ko.utils.arrayFirst(newPageSetCallbacks, function (item, i) {
                if (item === callbackToFind) {
                    index = i;
                    existing = item;
                    return true;
                }
            });

            callback(existing, index);
        }
    };

    function publishNotificationsPublic() {
        for (var i = 0; i < this.length; i++) {
            this[i]();
        }
    }
    
    function getNextPage(urlBase, cursor, parameters, options) {
        var url = buildUrl(urlBase, parameters),
            numberOfPages = 1,
            skipCountOverride,
            self = this;

        options = $.extend({}, self.callbackOptions, options);
        if (!isNaN(options.numberOfPages)) {
            numberOfPages = Math.max(options.numberOfPages, 1);
        }

        // skipCountOverride option enables the client to specify that they 
        // already have the first N records loaded.
        if (!isNaN(options.skipCountOverride)) {
            skipCountOverride = Math.max(options.skipCountOverride, 0);
        }

        // If the url base has changed, reset the current page to zero or the skipCountOverride value, if provided.
        // A change to the urlBase variable indicates that the query criteria has changed.
        if (url && this.urlBase !== url) {
            this.urlBase = url;
            self.allDataLoaded(false);
            cursor.skipCount = skipCountOverride || 0;
            self.notifyNewPageSetSubscribers();
        }

        return $.ajax({
            url: buildNextPageUrl(numberOfPages),
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            success: success,
            complete: options.completeCallback,
            error: options.errorCallback,
        });

        function success(data) {
            data = data || {};

            self.AllDataLoaded = (data.length < (numberOfPages * cursor.pageSize)) ? true : false;
            self.allDataLoaded(self.AllDataLoaded);

            cursor.skipCount += typeof options.resultCounter === "function"
                ? options.resultCounter(data)
                : data.length;

            if (data.length < cursor.pageSize && typeof options.onEndOfResults === "function") {
                options.onEndOfResults();
            }
            if (typeof options.successCallback === "function") { options.successCallback(data); }
        }

        function buildNextPageUrl() {
            var skipCount = cursor.skipCount;
            var pageSize = cursor.pageSize * numberOfPages;
            var appendToken = self.urlBase.indexOf("?") > 0 ? "&" : "?";
            return self.urlBase + appendToken + "skipCount=" + skipCount + "&pageSize=" + pageSize;
        }

    }

    function buildQuerystring(parameters) {
        var queryString = '';
        var params = parameters || {};

        for (var p in params) {
            var val = ko.utils.unwrapObservable(params[p]);
            if (typeof val === "function") val = val();
            if (val == undefined) continue;

            if (queryString.length > 0) queryString += '&';
            queryString += p + '=' + val;
        }

        return queryString.length ? "?" + queryString : '';
    }
    function buildUrl(urlBase, parameters) {
        return urlBase + buildQuerystring(parameters);
    }
})