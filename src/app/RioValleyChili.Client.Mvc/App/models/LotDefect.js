function EditableLotDefect(input) {
    if (input instanceof EditableLotDefect) return input;
    if (!(this instanceof EditableLotDefect)) return new EditableLotDefect(input);

    var lotDefectResolutionFactory = require('App/models/LotDefectResolution');
    var values = ko.toJS(input) || {};
    var model = this;

    this.LotDefectKey = values.LotDefectKey;
    this.DefectType = ko.observable(ko.utils.unwrapObservable(values.DefectType)).extend({ defectType: true, required: true });
    this.Description = ko.observable(values.Description).extend({ required: true });
    this.Resolution = ko.observable(values.Resolution ? lotDefectResolutionFactory(values.Resolution) : undefined);
    this.AttributeDefect = values.AttributeDefect;
    this.SummaryText = ko.computed({
        read: function () {
            return values.AttributeDefect
                ? this.DefectType && this.DefectType.displayValue() + " (" + values.AttributeDefect.OriginalMinLimit + " - " + values.AttributeDefect.OriginalMaxLimit + ")"
                : this.Description();
        },
        owner: model,
    });
    this.isResolved = ko.computed(function() {
        return model.Resolution() != undefined;
    });

    return this;
}

module.exports = EditableLotDefect;