define(['app', 'jquery', 'helpers/pagedDataHelper', 'ko'], function (app, $, pagedDataHelper, ko) {
    return {
        setupFn: setupFn,
        buildUrl: buildUrl,
        buildQueryString: buildQueryString,
        ajax: ajax,
        ajaxPut: ajaxPut,
        ajaxPost: ajaxPost,
        ajaxDelete: ajaxDelete,
        pagedDataHelper: pagedDataHelper,
    };

    //#region exports
    function setupFn(fn, url) {
        var urlFn = url;
        if (typeof url === "string") urlFn = function () { return url; }
        if (typeof urlFn !== "function") throw new Error("Missing or invalid URL function.");

        fn.url = function() {
            return urlFn.apply(null, arguments);
        }
        return fn;
    }
    function buildUrl(fn, parameters) {
        if (arguments.length === 0) throw new Error("No URL function was supplied.");

        fn = arguments[0];
        parameters = Array.prototype.slice.call(arguments, 1);
        return fn.apply(null, parameters);
    }
    function buildQueryString(params) {
        var queryString = '';
        params = ko.utils.unwrapObservable(params);

        if (!params) return '';
        if (params.toObj) params = params.toObj();
        for (var param in params) {
            if (queryString.length > 0) queryString += '&';
            queryString += param + '=' + params[param];
        }
        return queryString.length > 0 ? ("?" + queryString) : '';
    }
    function ajax(url, options) {
        if (!app.antiForgeryTokenId) {
            app.antiForgeryTokenId = window.antiForgeryTokenId;
        }
        
        options = options || {};
        //todo: remove references to failCallback (replace with errorCallback)
        if (options.failCallback) {
            options.errorCallback = options.failCallback;
            try {
                console.warn("API options parameter contains reference to \"failCallback\". Replace with \"errorCallback\".");
            } catch (ex) { }
        }

        switch (typeof (url)) {
            case "function":
                url = url();
                break;
            case "string":
                break;
            default:
                throw new Error("Invalid parameter 'url'. Expected function or string but received " + typeof (url));
        }

        var headers = {};
        headers[app.antiForgeryTokenId] = getAntiForgeryToken();
        
        options.type = options.type || 'GET';

        var dfd = $.Deferred();

        send();

        var promise = dfd.promise();
        promise.error = promise.fail; // error was available to the promise returned by $.ajax; some code still depends on it.
        return promise;

        function send() {
            $.ajax({
                url: url,
                type: options.type,
                dataType: options.dataType || 'json',
                contentType: options.contentType || 'application/json',
                data: options.data,
                headers: headers,
                complete: options.completeCallback,
                success: function () {
                    options.successCallback && options.successCallback.apply(null, arguments);
                    dfd.resolve.apply(null, arguments);
                },
                error: function (xhr) {
                    if (retry(xhr)) send();
                    else {
                        options.errorCallback && options.errorCallback.apply(null, arguments);
                        dfd.reject.apply(null, arguments);
                    }
                }
            });
        }
        function retry(xhr) {
            var timeOutStatusCode = '408';

            var statusCode = typeof xhr.statusCode === "function"
                ? xhr.statusCode().status
                : xhr.statusCode;

            // If operation is not a GET method, do not retry unless
            // the error is a timeout error.
            if (options.type.toUpperCase() !== 'GET') {
                if (statusCode !== timeOutStatusCode) return false;
            }

            options.attemptCount = (options.attemptCount || 0) + 1;

            var tryCount = options.attemptCount,
                maxAttempts = options.maxAttempts || app.ajaxMaxRetryAttempts;

            return tryCount < maxAttempts;
        }
    }
    function ajaxPut(url, data, callbackOptions) {
        var options = callbackOptions || {};
        options.type = 'PUT';
        options.contentType = 'application/json';
        options.data = json(data);
        return ajax(url, options);
    }
    function ajaxPost(url, data, callbackOptions) {
        if (arguments.length === 2 && data.data) {
            callbackOptions = data;
            data = data.data;
        }
        var options = callbackOptions || {};
        options.type = 'POST';
        options.contentType = 'application/json';
        options.data = json(data);
        return ajax(url, options);
    }
    function ajaxDelete(url, callbackOptions) {
        var options = callbackOptions || {};
        options.type = 'DELETE';
        return ajax(url, options);
    }
    //#endregion exports

    //#region private
    function getAntiForgeryToken() {
        return $('input[name="' + app.antiForgeryTokenId + '"]').val();
    }
    function json(data) {
        if (typeof data === "object") return ko.toJSON(data);
        return data;
    }
    //#endregion private
});