"strict";

var PagedDataHelper = (function (root, $, undefined) {
    
    var cursor = {};

    var self = {
        allDataLoaded: ko.observable(false),
        urlBase: '',
    };
    self.GetNextPage = getNextPage.bind(self);
    self.nextPage = getNextPage.bind(self);

    var cursorDefaults = {
        skipCount: 0,
        pageSize: 20,
    };
    
    self.init = function (options) {
        var lastUrl;
        var cursorSettings = typeof options === "string"
            ? { urlBase: options }
            : options;
        
        cursor = $.extend({}, cursorDefaults, cursorSettings);
        self.urlBase = cursorSettings.urlBase;
        self.incrementSkipCount = function(count) {
            count = parseInt(count) || 1;
            cursor.skipCount += count;
        };
        self.callbackOptions = options.callbackOptions || {};
        cursor.parameterizedUrl = ko.computed({
            read: function () {
                var queryString = '';
                var params = options.parameters || {};

                for (var param in params) {
                    var val = ko.utils.unwrapObservable(params[param]);
                    if (val == undefined) continue;

                    if (queryString.length > 0) queryString += '&';
                    queryString += param + '=' + val;
                }

                var url = cursorSettings.urlBase + "?" + queryString;
                if (url != lastUrl) {
                    self.allDataLoaded(false);
                    lastUrl = url;
                }
                return url;
            },
            deferEvaluation: true,
        });
        return self;
    };

    return self;

    function getNextPage(options) {
        var urlBase,
            numberOfPages = 1,
            skipCount = null;

        var skipCountOverride;
        
        if (typeof options == "string") {
            urlBase = options; 
        } else {
            options = $.extend({}, self.callbackOptions, options);
            urlBase = options.urlBase || cursor.parameterizedUrl();
            if (!isNaN(options.numberOfPages)) {
                numberOfPages = Math.max(options.numberOfPages, 1);
            }

            // skipCountOverride option enables the client to specify that they 
            // already have the first N records loaded.
            if (!isNaN(options.skipCountOverride)) {
                skipCountOverride = Math.max(options.skipCountOverride, 0);
            }
        }
        
        // If the url base has changed, reset the current page to zero or the skipCountOverride value, if provided.
        // A change to the urlBase variable indicates that the search criteria has changed.
        if (urlBase && this.urlBase !== urlBase) {
            this.urlBase = urlBase;
            cursor.skipCount = skipCountOverride || 0;
            cursor.onNewPageSet && cursor.onNewPageSet();
        }
        skipCount = (cursor.currentPage * cursor.pageSize) || 0;
        
        var url = buildNextPageUrl(this.urlBase, numberOfPages, skipCount);
        $.ajax({
            url: url,
            type: 'GET',
            contentType: 'application/json',
            success: success,
            complete: options.completeCallback,
            error: options.errorCallback,
        });
        
        function success(data) {
            data = convertObjectToArray(data) || [];
            
            self.AllDataLoaded = (data.length < (numberOfPages * cursor.pageSize)) ? true : false;
            self.allDataLoaded(self.AllDataLoaded);

            cursor.skipCount += cursor.resultCounter 
                ? cursor.resultCounter(data)
                : data.length;

            if (options.successCallback) { options.successCallback(data); }
            
            function convertObjectToArray(input) {
                if (!input) return {};
                return input.length == undefined
                    ? [input]
                    : input;
            }
        }
    }

    function buildNextPageUrl(urlBase, numberOfPages) {
        var skipCount = cursor.skipCount;
        var pageSize = cursor.pageSize * numberOfPages;
        var appendToken = urlBase.indexOf("?") > 0 ? "&" : "?";
        return urlBase + appendToken + "skipCount=" + skipCount + "&pageSize=" + pageSize;
    }
}(window, jQuery))