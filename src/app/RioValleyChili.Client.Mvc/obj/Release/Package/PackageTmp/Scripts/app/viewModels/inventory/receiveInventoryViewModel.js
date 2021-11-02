var ReceiveInventoryViewModel = (function (api) {
    var defaultPackagingDelegate = function() {
        return this.ProductName === "1000 lb Tote";
    };
    var defaultWarehouseLocationDelegate = function () {
        return this.Description === "DHY00";
    };
    var newItemDefaults = {
        Quantity: 1,
        Packaging: null,
        Location: null
    };

    var self = {
        resetForm: resetForm,
        recentEntries: ko.observableArray([]),
        SupplierOptions: ko.observableArray([]),

        User: "",
        ProductionDate: ko.observable().extend({ isoDate: "m/d/yyyy" }),
        Supplier: ko.observable(),
        Load: ko.numericObservable(1),
        Product: ko.observable(),
        PurchaseOrder: ko.observable(),
        ShipperNumber: ko.observable(),
        selectedProducts: ko.observableArray([]),
        currentProduct: ko.observable(),
        NewMaterials: ko.observableArray(),
        showItem: rvc.utils.animateNewItem({
            scrollToItem: true
        }),
    };

    // subscribers
    self.ShipperNumber.subscribe(function () {
        self.focusTote();
    });

    // computed
    self.isEditing = ko.computed(function(){
        return this.selectedProducts().length === 1;
    }, self);
    self.totalWeight = ko.computed(function () {
        var w = 0;
        ko.utils.arrayForEach(this.NewMaterials(), function (item) {
            w += item.Weight();
        });
        return w;
    }, self);

    // commands
    self.addMaterialCommand = ko.asyncCommand({
        execute: function (complete) {
            if (self.isEditing()) {
                self.selectItemCommand.execute(self.selectedProducts()[0]);
            } else {
                var current = self.currentProduct(),
                  currentVariety = current.Variety();

                if (currentVariety === "Add New Variety") {
                  current.Variety = ko.observable(current.NewVariety());
                  self.ChileVarietyOptions.push(current.NewVariety());
                }

                self.NewMaterials.push(current);

                var newItem = new self.DehydratedMaterial();

                newItem.Tote.formattedTote(current.Tote.getNextTote());
                newItem.Packaging(current.Packaging());
                newItem.Location(current.Location());
                newItem.Grower(current.Grower());
                newItem.Variety(currentVariety);
                newItem.NewVariety(current.NewVariety());

                self.currentProduct(newItem);
            }
            self.focusTote();
            complete();
        },
        canExecute: function (isExecuting) {
            var currentProduct = self.currentProduct();
            return !isExecuting && currentProduct && self.selectedProducts().length <= 1 && currentProduct.isValid();
        }
    });
    self.loadDehydratorsCommand = ko.composableCommand({
        execute: function(complete) {
            api.companies.getDehydrators({
                successCallback: function(data) {
                    self.SupplierOptions(data);
                },
                errorCallback: function(xhr, statusText, message) {
                    showUserMessage("Unable to load Dehydrator Companies list.");
                },
                completeCallback: complete,
        });
        },
        canExecute: function(isExecuting) { return !isExecuting; }
    });
    self.selectItemCommand = ko.command({
        execute: selectItem.bind(self),
    });
    self.removeItemCommand = ko.command({
        execute: removeSelectedItems.bind(self),
        canExecute: function() {
            return self.selectedProducts().length > 0;
        }
    });
    self.saveCommand = ko.asyncCommand({
        execute: function(complete) {
            var dto = buildDto.call(self);
            postAsync(dto, complete, self);
        },
        canExecute: function(isExecuting) {
            return !isExecuting && enableSave();

            function enableSave() {
                return self.ProductionDate() && self.Supplier() && self.Load() && self.Product() && self.NewMaterials().length > 0;
            }
        }
    });

    // child object
    self.DehydratedMaterial = function () {
        var me = this;
        var values = newItemDefaults;

        // always new tote
        me.Tote = ko.observable().extend({
            toteKey: true,
            required: true,
            validation: {
                message: 'This tote has already been used!',
                validator: function () {
                    if (self.selectedProducts().length) return true; //todo: handle duplicated created during edit
                    return !isDuplicate();

                    function isDuplicate() {
                        var val = me.Tote.formattedTote();
                        return ko.utils.arrayFirst(self.NewMaterials(), function(material) {
                            return material.Tote.isMatch(val);
                        }) !== null;
                    }
                }
            },
        });
        me.Quantity = ko.observable(values.Quantity).extend({ required: true });
        me.Packaging = ko.observable(values.Packaging).extend({ required: true });
        me.Variety = ko.observable(values.Variety).extend({ required: true });
        me.NewVariety = ko.observable(values.NewVariety).extend({
          validation: {
            validator: function(val) {
              return (me.Variety() !== 'Add New Variety') || val && typeof val === 'string';
            },
            message: 'This field is required.'
          }
        });
        me.Location = ko.observable(values.Location).extend({ required: true });
        me.Grower = ko.observable(values.Grower).extend({ required: true });

        // still calculated the same
        me.Weight = ko.computed(function () {
            if (me.Quantity() && me.Packaging()) return me.Quantity() * me.Packaging().Weight;
            else return 0;
        });
        me.selected = ko.observable(false);

        var validated = ko.validatedObservable(me);
        me.isValid = ko.computed(function() {
            return validated.isValid();
        });
    };

    // private functions
    function removeSelectedItems() {
        this.NewMaterials($.grep(this.NewMaterials(), function (item) {
            return !item.selected();
        }));
        this.selectedProducts([]);
        this.currentProduct(new this.DehydratedMaterial());
    }
    function selectItem(o) {
        if (!o.selected()) {
            self.selectedProducts.push(o);
            self.currentProduct(o);
        }
        else {
            self.selectedProducts.splice(self.selectedProducts().indexOf(o), 1);
            self.currentProduct(new self.DehydratedMaterial());
        }
        o.selected(!o.selected());
    }

    self.focusTote = function() {
        $("#toteInput")[0].focus();
    };

    // INIT
    self.init = function (opts) {
        ko.utils.arrayForEach(["ChileProductOptions", "ChileVarietyOptions", "PackagingOptions", "WarehouseOptions"], function(prop) {
            self[prop] = opts[prop];
        });
        self.ChileVarietyOptions.unshift("Add New Variety");
        newItemDefaults.Packaging = ko.utils.arrayFirst(self.PackagingOptions, function (item) { return defaultPackagingDelegate.apply(item); });
        newItemDefaults.Location = ko.utils.arrayFirst(self.WarehouseOptions, function (item) { return defaultWarehouseLocationDelegate.apply(item); });
        self.currentProduct(new self.DehydratedMaterial());
        self.ProductionDate(Date.today().toISOString());

        return self;
    };

    self.loadDehydratorsCommand.execute();
    return self;

    function resetForm() {
        self.Supplier(null);
        var loadNum = parseInt(self.Load());
        self.Load(isNaN(loadNum) ? '' : ++loadNum);
        self.PurchaseOrder('');
        self.ShipperNumber('');
        self.NewMaterials([]);

        var currentProduct = self.currentProduct();
        currentProduct.Tote('');
        currentProduct.Packaging(newItemDefaults.Packaging || null);
        currentProduct.Variety(null);
        currentProduct.NewVariety(null);
        currentProduct.Location(newItemDefaults.Location || null);
        currentProduct.Grower('');
        currentProduct.Quantity(1);
    }

    function buildDto() {
        var model = {
            User: "",
            ProductionDate: this.ProductionDate(),
            DehydratorKey: this.Supplier().CompanyKey,
            Load: this.Load(),
            ChileProductKey: this.Product().ProductKey,
            PurchaseOrder: this.PurchaseOrder(),
            ShipperNumber: this.ShipperNumber(),
            ItemsReceived: []
        };
        model.ItemsReceived = ko.utils.arrayMap(this.NewMaterials(), function (item) {
            return {
                Quantity: item.Quantity(),
                PackagingProductKey: item.Packaging().ProductKey,
                Variety: item.Variety(),
                GrowerCode: item.Grower(),
                WarehouseLocationKey: item.Location().LocationKey,
                ToteKey: item.Tote(),
            };
        });

        return model;
    }

    function postAsync(dto, complete, owner) {
        owner = owner || this;
        var json = ko.toJSON(dto);

        $.ajax("/api/dehydratedMaterialsReceived", {
            data: json,
            type: "POST",
            contentType: "application/json",
            success: function (data, textStatus, xhr) {
                var message = "The Dehydrated Materials have been recorded and received into inventory with the lot \"" + data + "\".";
                var location = xhr.getResponseHeader("Location");
                if (location) {
                    message += " <a href='" + location + "'>Click here to view.</a>";
                }

                showUserMessage("Dehydrated materials received successfully", {
                    description: message
                });

                owner.recentEntries.push({ lot: data, link: location });
                owner.resetForm();
            },
            error: function (data, status, message) {
                showUserMessage("Dehydrated Materials Failed to Save", { description: message });
            },
            complete: complete,
        });
    }
})(api);
