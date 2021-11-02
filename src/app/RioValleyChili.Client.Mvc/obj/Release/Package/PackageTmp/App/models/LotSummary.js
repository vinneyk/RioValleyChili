var lotDefectFactory = require('App/models/LotDefect');
var lotDefectResolutionFactory = require('App/models/LotDefectResolution');
var LotAttributeFactory = require('App/models/LotAttribute');
var rvc = require('app');

require('koProjections');

function LotSummary( values, checkOutOfRange ) {
    if (!(this instanceof LotSummary)) return new LotSummary(values);
    var self = this;

    this.LotKey = values.LotKey;
    this.LotDate = values.LotDate;
    this.AstaCalc = values.AstaCalc;
    this.LoBac = values.LoBac;
    this.HoldType = ko.observable(values.HoldType).extend({ lotHoldType: true });
    this.HoldDescription = ko.observable(values.HoldDescription);
    this.Product = values.Product ? JSON.parse(ko.toJSON(values.Product)) : {};
    this.ProductionStatus = ko.observable(values.ProductionStatus).extend({ productionStatusType: true });
    this.QualityStatus = ko.observable(values.QualityStatus).extend({ lotQualityStatusType: true });
    this.ProductSpecStatus = ko.observable(values.ProductSpecStatus);
    this.Defects = ko.observableArray(ko.utils.arrayMap(values.Defects, function (item) { return lotDefectFactory(item); }));
    this.Attributes = this.buildAttributes( values.Attributes, checkOutOfRange );
    this.Treatment = JSON.parse(ko.toJSON(values.InventoryTreatment || values.Treatment || {}));
    this.CustomerName = values.CustomerName;
    this.CustomerKey = values.CustomerKey;
    this.Notes = values.Notes;
    this.InHouseDefects = self.Defects.filter(function (d) {
      return ko.unwrap(d.DefectType) === rvc.lists.defectTypes.InHouseContamination.key;
    });
    this.OpenInHouseDefects = self.InHouseDefects.filter(function(d) {
      return !d.isResolved();
    });

    this.Product.ProductType = ko.observable(ko.unwrap(values.Product.ProductType)).extend({ inventoryTypes: true });

    this.QualityControlNotebookKey = values.QualityControlNotebookKey;
    this.ValidLotQualityStatuses = values.ValidLotQualityStatuses;
    this.OldContextLotStat = values.OldContextLotStat;
    this.CustomerAllowances = values.CustomerAllowances;
    this.CustomerOrderAllowances = values.CustomerOrderAllowances;
    this.ContractAllowances = values.ContractAllowances;

    this.tooltipText = ko.computed(function () {
        return this.LotKey + ' - ' + this.Product.ProductName;
    }, this);

    return self;
}

LotSummary.prototype.buildAttributes = function( attributeValues, checkOutOfRange ) {
    if (!attributeValues || !attributeValues.length) return [];

    var defects = this.Defects();
    defects = defects && defects.length ? defects.reverse() : [];
    var attrDefectsCache = buildAttributeDefectsCache(defects);

    return ko.utils.arrayMap(attributeValues, function (attr) {
        attr.Defect = attrDefectsCache[attr.Key];
        return new LotAttributeFactory( attr, checkOutOfRange );
    });
};

function buildAttributeDefectsCache(defects) {
    var dCache = [];
    ko.utils.arrayMap(defects, function (d) {
        if (d.AttributeDefect && d.AttributeDefect.AttributeShortName) {
            dCache[d.AttributeDefect.AttributeShortName] = d;
        }
    });
    return dCache;
}

module.exports = LotSummary;
