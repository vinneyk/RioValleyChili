define(['ko'], function (ko) {
    var self = {
        enums: initEnums(),
        lists: buildLists(),
        navigation: buildNavigationHelper(),
        constants: {
            rinconWarehouse: new TypeDescriptor({
                WarehouseKey: '2',
                WarehouseName: 'Rincon',
            }),
            ThousandPoundTotePackaging: {
                ProductKey: "792",
            },
            DehyLocation: {
                LocationKey: "155",
                Description: "DHY00",
            }
        }
    };
    self.helpers = buildHelpers();


    return self;

    function buildLists() {
        var inventoryTypes = new TypeDescriptor({
            'Chile': { key: 1, value: 'Chile' },
            'Packaging': { key: 5, value: 'Packaging' },
            'Additive': { key: 4, value: 'Additive' },
            'NonInventory': { key: 7, value: 'Non-Inventory' },
        });
        var lotTypes = new TypeDescriptor({
            'OtherRaw': { key: 0, value: 'Other Raw', inventoryType: inventoryTypes.Chile },
            'Dehydrated': { key: 1, value: 'Dehydrated', inventoryType: inventoryTypes.Chile },
            'WIP': { key: 2, value: 'WIP', inventoryType: inventoryTypes.Chile },
            'FinishedGoods': { key: 3, value: 'Finished Goods', inventoryType: inventoryTypes.Chile },
            'Additive': { key: 4, value: 'Additive', inventoryType: inventoryTypes.Additive },
            'Packaging': { key: 5, value: 'Packaging', inventoryType: inventoryTypes.Packaging },
            'Other': { key: 11, value: 'Other', inventoryType: inventoryTypes.Chile },
            'GRP': { key: 12, value: 'GRP', inventoryType: inventoryTypes.Chile },
        });

        var lotTypesByInventoryType = {};
        for (var k in lotTypes) {
            if (k === 'length' || !lotTypes.hasOwnProperty(k)) continue;
            var current = lotTypes[k];
            if (!lotTypesByInventoryType[current.inventoryType.value]) lotTypesByInventoryType[current.inventoryType.value] = [];
            lotTypesByInventoryType[current.inventoryType.value].push(current);
        }

        var lists = {
            rinconWarehouse: new TypeDescriptor({
                WarehouseKey: '2',
                WarehouseName: 'Rincon',
            }),
            inventoryTypes: inventoryTypes,
            productionStatusTypes: new TypeDescriptor({
                'Batched': { key: 0, value: 'Batched' },
                'Produced': { key: 1, value: 'Produced' },
            }),
            lotQualityStatusTypes: new TypeDescriptor({
              'Pending': { key: 0, value: 'Pending' },
              'Released': { key: 1, value: 'Released' },
              'Contaminated': { key: 2, value: 'Contaminated' },
              'Rejected': { key: 3, value: 'Rejected' },
            }),
            lotTypes: lotTypes,
            lotTypesByInventoryType: lotTypesByInventoryType,
            locationStatusTypes: new TypeDescriptor({
                Available: { key: 1, value: 'Available' },
                Locked: { key: 2, value: 'Locked' },
                InActive: { key: 3, value: 'InActive' },
            }),
            chileTypes: new TypeDescriptor({
                'OtherRaw': { key: 0, value: 'Other Raw' },
                'Dehydrated': { key: 1, value: 'Dehydrated' },
                'WIP': { key: 2, value: 'WIP' },
                'FinishedGoods': { key: 3, value: 'Finished Goods' },
            }),
            chileClassifications: new TypeDescriptor({
                'ChiliPepper': { key: 1, value: 'Chili Pepper' },
                'ChiliPowder': { key: 2, value: 'Chili Powder' },
                'Paprika': { key: 3, value: 'Paprika' },
                'RedPepper': { key: 4, value: 'Red Pepper' },
                'Other': { key: 7, value: 'Other' },
                'GRP': { key: 12, value: 'GRP' },
            }),
            defectTypes: new TypeDescriptor({
                'ProductSpec': { key: 0, value: 'Product Spec' },
                'Bacterial': { key: 1, value: 'Bacterial Contamination' },
                'InHouseContamination': { key: 2, value: 'In-House Defect' },
                'ActionableDefect': { key: 3, value: 'Quality Hold' },
                'CustomerProductSpec': { key: 4, value: 'Customer Spec' },
            }),
            lotHoldTypes: new TypeDescriptor({
                'HoldForCustomer': { key: 0, value: 'Customer' },
                'HoldForTesting': { key: 1, value: 'Testing' },
                'HoldForTreatment': { key: 2, value: 'Treatment' }
            }),
            defectResolutionTypes: new TypeDescriptor({
                'DataEntryCorrection': { key: 0, value: 'Data Entry Correction' },
                'Treated': { key: 2, value: 'Treated' },
                'Retest': { key: 3, value: 'Retested' },
                'AcceptedByUser': { key: 4, value: 'Accepted By QC' },
                'InvalidValue': {key: 6, value: 'Invalid Value' }
            }),
            treatmentTypes: new TypeDescriptor({
                'NotTreated': { key: 0, value: 'NA' },
                'ET': { key: 1, value: 'ET' },
                'LowBac': { key: 2, value: 'LB' },
                'GT': { key: 3, value: 'GT' },
                'EF': { key: 4, value: 'EF' },
            }),
            orderStatus: new TypeDescriptor({
                'Scheduled': { key: 0, value: 'Scheduled' },
                'Fulfilled': { key: 1, value: 'Fulfilled' },
                'Void': { key: 2, value: 'Void' },
            }),
            customerOrderStatus: new TypeDescriptor({
              'Ordered': { key: 0, value: 'Ordered' },
              'Invoiced': { key: 1, value: 'Invoiced' },
            }),
            shipmentStatus: new TypeDescriptor({
                'Unscheduled': { key: 0, value: 'Unscheduled' },
                'Scheduled': { key: 1, value: 'Scheduled' },
                'Shipped': { key: 10, value: 'Shipped' },
                'Delivered': { key: 100, value: 'Delivered' },
            }),
            inventoryOrderTypes: new TypeDescriptor({
                'Unknown': { key: 0, value: 'Unknown' },
                "IntraWarehouseMovement": { key: 1, value: 'WarehouseMovements' },
                'TransWarehouseMovement': { key: 2, value: 'TransWarehouseMovements' },
                'Treatment': { key: 3, value: 'Treatments' },
                'ProductonBatch': { key: 4, value: 'ProductionBatch' },
                'CustomerOrder': { key: 5, value: 'CustomerOrder' },
            }),
            companyTypes: new TypeDescriptor({
                'Customer': { key: 0, value: 'Customer', group: 'Customer' },
                'Supplier': { key: 1, value: 'Supplier', group: 'Vendor' },
                'Dehydrator': { key: 2, value: 'Dehydrator', group: 'Vendor' },
                'Broker': { key: 3, value: 'Broker', group: 'Broker' },
                'Freight': { key: 4, value: 'Freight', group: 'Vendor' },
                'TreatmentFacility': { key: 5, value: 'Treatment Facility', group: 'Vendor' }
            }),
            inventoryPickingContexts: new TypeDescriptor({
                'WarehouseMovements': { key: 1, value: 'WarehouseMovements' },
                'TransWarehouseMovements': { key: 2, value: 'TransWarehouseMovements' },
                'Treatments': { key: 3, value: 'Treatments' },
                'ProductionBatch': { key: 4, value: 'ProductionBatch' },
                'CustomerOrder': { key: 5, value: 'CustomerOrder' },
            }),
            contractTypes: new TypeDescriptor({
                'Contract': { key: 0, value: 'Contract' },
                'Quote': { key: 1, value: 'Quote' },
                'Spot': { key: 2, value: "Spot" },
                'Interim': { key: 3, value: 'Interim' }
            }),
            contractStatuses: new TypeDescriptor({
                'Pending': { key: 0, value: 'Pending' },
                'Rejected': { key: 1, value: 'Rejected' },
                'Confirmed': { key: 2, value: 'Confirmed' },
                'Completed': { key: 3, value: 'Completed' },
            }),
            facilityTypes: new TypeDescriptor({
                'Internal': { key: 0, value: 'Internal Warehouse' },
                'External': { key: 1, value: 'External Warehouse' },
                'Consignment': { key: 2, value: 'Consignment Warehouse' },
                'Treatment': { key: 3, value: 'Treatment Facility' },
            }),
            sampleStatusTypes: new TypeDescriptor({
              'Pending': { key: 0, value: 'Pending' },
              'Sent': { key: 1, value: 'Sent' },
              'Approved': { key: 2, value: 'Approved' },
              'Rejected': { key: 3, value: 'Rejected' },
              'See Journal Entry': { key: 4, value: 'See Journal Entry' },
            })
        };
        return lists;
    }

    function buildHelpers() {
        var inventoryTypes = buildInventoryTypeArray();
        var lotTypes = buildLotTypeArray();

        return {
            forEachInventoryType: forEachInventoryType,
            forEachLotType: forEachLotType,
            extend: extend
        };

        function forEachInventoryType(fn) {
            ko.utils.arrayForEach(inventoryTypes, fn);
        }

        // Preserves prototype chain and correctly extends upon it
        function extend( base, sub ) {
          // Cache sub prototype before overwriting with base
          var origPrototype = sub.prototype;
          sub.prototype = Object.create( base.prototype );

          // Re-add sub sub prototype's properties and methods
          for ( var key in origPrototype ) {
            sub.prototype[key] = origPrototype[key];
          }

          // Assign correct constructor and set as non-enumerable
          sub.prototype.constructor = sub;
          Object.defineProperty( sub.prototype, 'constructor', {
            enumerable: false,
            value: sub
          });
        }

        function forEachLotType(fn) {
            ko.utils.arrayForEach(lotTypes, fn);
        }

        function buildInventoryTypeArray() {
            var types = [];
            for (var prop in self.lists.inventoryTypes) {
                if (self.lists.inventoryTypes.hasOwnProperty(prop))
                    types.push(self.lists.inventoryTypes[prop]);
            }
            return types;
        }

        function buildLotTypeArray() {
            var types = [];
            for (var prop in self.lists.lotTypes) {
                if (self.lists.lotTypes.hasOwnProperty(prop))
                    types.push(self.lists.lotTypes[prop]);
            }
            return types;
        }
    }
});

function initEnums() {
    var enums = {};

    enums.LotQualityStatus = new Enum({
        Pending: 0,
        Released: 2,
        Contaminated: 3,
        Rejected: 4,
    });

    enums.NotificationLevel = new Enum({
        Information: 0,
        Warning: 10,
        Critical: 100,
    });

    return enums;
}

function Enum(vals) {
    if (!(this instanceof Enum)) return new Enum(vals);

    var me = this;

    for (var p in vals) {
        var val = vals[p];
        me[p] = val;
        me[val] = p;
    }

    Object.freeze(me);

    return me;
}

function TypeDescriptor(properties) {
    if (!(this instanceof TypeDescriptor)) return new TypeDescriptor(properties);
    var type = this;

    for (var p in properties) {
        type[p] = properties[p];
    }

    TypeDescriptor.prototype.findByKey = function (keyValue) {
        var stringValue = keyValue.toString();
        for (var p in this) {
            if (this[p].key != undefined && this[p].key.toString() === stringValue) {
                return this[p];
            }
        }
        return null;
    };
    TypeDescriptor.prototype.toDictionary = function () {
        var options = {};
        for (var prop in this) {
            if (this.hasOwnProperty(prop)) {
                var opt = this[prop];
                options[opt.key] = opt.value;
            }
        }
        return options;
    };
    TypeDescriptor.prototype.toObjectDictionary = function () {
        var options = {};
        for (var prop in this) {
            if (this.hasOwnProperty(prop)) {
                var opt = this[prop];
                options[opt.value] = opt;
            }
        }
        return options;
    };
    TypeDescriptor.prototype.asEnum  = function() {
        return new Enum(this.optionValues);
    };
    TypeDescriptor.prototype.buildSelectListOptions = function () {
        var selectListOptions = [];
        for (var opt in this) {
            if (this.hasOwnProperty(opt)) {
                selectListOptions.push({
                    key: opt,
                    value: this[opt]
                });
            }
        }
        return selectListOptions;
    }; // is lotTypes
    if(properties.Dehydrated && properties.WIP && properties.Packaging && properties.Additive)
        TypeDescriptor.prototype.fromLotKey = function (lotKey) {
            var re = /^(\d{2})( \d{2,3}){3}$/;
            var key = parseInt(lotKey.match(re));
            return this.findByKey(key);
        };
}

function TypeExtension(target, options, defaultOption) {
    target.displayValue = ko.computed({
        read: function () {
            if (target() == undefined) return '';
            return getTypeOption(target()) || defaultOption;
        }
    });
    target.options = buildSelectListOptions(options);
    return target;

    function buildSelectListOptions(source) {
        var selectListOptions = [];
        for (var opt in source) {
            selectListOptions.push({
                key: opt,
                value: source[opt]
            });
        }
        return selectListOptions;
    }
    function getTypeOption(val) {
        switch (typeof val) {
            case "string": return fromString(val);
            case "number": return fromNumber(val);
            case "object": return fromObject(val);
            default: return undefined;
        }

        function fromString(s) {
            return fromNumber(parseInt(s))
                || findOptionByName();

            function findOptionByName() {
                for (var prop in options) {
                    if (options[prop] === s) {
                        return fromString(prop);
                    }
                }
                return undefined;
            }
        }
        function fromNumber(num) {
            if (isNaN(num)) return undefined;
            return options[num + ''];
        }
        function fromObject(o) {
            if (!o || o.value == undefined) return undefined;
            return o.value;
        }
    }
}

function buildNavigationHelper() {
    return {
        getHashValue: getHashValue ,
        urlDecode: urlDecode,
        updateHistory: updateHistory,
        replaceState: replaceState,
        pushState: pushState,
    };

    function urlDecode(value) {
        return decodeURIComponent((value + '').replace(/\+/g, '%20'));
    }


    // obsolete:
    function updateHistory(hash, title, state, replace) {
        if (!replace && getHashValue() === hash) return;

        var url = hash ? "#" + hash : window.location.pathname;
        var args = [state, title, url];

        replace === true
            ? replaceState.apply(null, args)
            : pushState.apply(null, args);
    }
    function replaceState(state, pageTitle, location) {
        if (getHashValue() !== location) {
            history.replaceState(state, pageTitle + " - Rio Valley Chili, Inc.", location);
        }
    }
    function pushState(state, pageTitle, location) {
        if (getHashValue() !== location) {
            history.pushState(state, pageTitle + " - Rio Valley Chili, Inc.", location);
        }
    }
    function getHashValue() {
        if (location.hash) {
            return urlDecode(location.hash.replace("#", ""));
        }
        return null;
    }
}
