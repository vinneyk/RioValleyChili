// replaced by rvc.js for AMD modules; discontinue use and replace.
window.app = window.app || {};
(function (app) {
    app.navigation = {
        updateHistory: function (hash, title, state, replace) {
            if (!replace && rvc.utils.getHashValue() === hash) return;

            var url = hash ? "#" + hash : window.location.pathname;
            var args = [state, title, url];

            replace === true
                ? rvc.helpers.history.replaceState.apply(null, args)
                : rvc.helpers.history.pushState.apply(null, args);
        }
    };

    window.onpopstate = function (e) {
        app.navigation.loadFromState && app.navigation.loadFromState(e.state);
    };
}(window.app));