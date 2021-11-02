module.exports = {
    init: function (target, customDisposalFn) {
        ko.utils.extend(target.prototype, {
            registerDisposable: function (disposable) {
                this.__disposables = this.__disposables || [];
                this.__disposables.push(disposable);
            },
            unregisterDisposable: function (disposable) {
                this.__disposables && this.__disposables.length &&
                    ko.utils.arrayRemoveItem(this.__disposables, disposable);
            },
            disposeOne: function (propOrValue, value) {
                var disposable = value || propOrValue;

                if (disposable && typeof disposable.dispose === "function") {
                    disposable.dispose();
                }
            },
            dispose: function () {
                ko.utils.arrayForEach(this.__disposables, this.disposeOne);
                ko.utils.objectForEach(this, this.disposeOne);
                if (typeof customDisposalFn === "function") customDisposalFn();
            }
        });
    }
}