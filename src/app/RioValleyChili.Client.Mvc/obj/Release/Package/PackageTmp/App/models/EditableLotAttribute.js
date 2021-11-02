function EditableLotAttribute(values) {
    if (!(this instanceof EditableLotAttribute)) return new EditableLotAttribute(values);
    var rvc = require('app');

    var model = this;

    this.Key = values.Key;
    this.Value = ko.numericObservable(values.Value);
    this.AttributeDate = ko.observableDate(values.AttributeDate).extend({
        required: {
            onlyIf: function () {
                return model.Value && model.Value() != undefined;
            }
        }
    });

    var lotDefectFactory = require('App/models/LotDefect');
    this.Defect = values.Defect && lotDefectFactory(values.Defect);

    this.isResolved = ko.computed(function () {
        return this.Defect && this.Defect.isResolved();
    }, this);
    this.isUnresolved = ko.computed(function () {
        return this.Defect && !this.isResolved();
    }, this);
    this.hasContaminatedDefect = ko.computed(function () {
        return this.isUnresolved() && this.Defect.DefectType() === rvc.lists.defectTypes.Bacterial.key;
    }, this);
    this.hasActionableDefect = ko.computed(function () {
        return this.isUnresolved() && this.Defect.DefectType() === rvc.lists.defectTypes.ActionableDefect.key;
    }, this);
    this.isValueComputed = ko.pureComputed(function() {
        return (values.Computed || values.isValueComputed) && values.Value === model.Value();
    });

    this.formattedValue = ko.computed(function() {
        var value = this.Value();
        return value && value.toLocaleString();
    }, this);

    return this;
}

module.exports = EditableLotAttribute;