function LotDefectResolution(values) {
    if (!(this instanceof LotDefectResolution)) return new LotDefectResolution(values);
    this.LotDefectKey = values.LotDefectKey;
    this.ResolutionType = ko.observable(values.ResolutionType).extend({ defectResolutionType: true, required: true });
    this.Description = ko.observable(values.Description).extend({ required: true });
    this.isEditing = ko.observable(false);
}

module.exports = LotDefectResolution;