function range(max){
    var x = [];
    for(var i = 0; i < max; i++) x.push(i);
    return x;
}

var MillAndWetdownViewModel = (function () {
    var self = {
        toDto: getDto,
    };
    self.Options = {};


    // private functions
    var createDate = function(dateStr, time) {
        return new Date(dateStr).addHours(time.Hours()).addMinutes(time.Mins());
    };
    var getLocationByName = function(name) {
        if (!self.Options.WarehouseOptions) return;
        return ko.utils.arrayFirst(self.Options.WarehouseOptions, function(i) {
            return i.WarehouseLocationName == name;
        });
    };
    self.previousLocation = null;
    
    // Custom objects
    self.Material = function() {
        this.Number = ko.observable().extend({ lotKey: true });
        this.InventoryItems = ko.observableArray();
    };
    self.OutputItem = function() {
        this.Location = ko.observable(self.previousLocation);
        this.Location.subscribe(function(val) {
            self.previousLocation = val;
        });
        this.Quantity = ko.numericObservable();
        this.Packaging = ko.observable();
        this.Weight = ko.computed(function() {
            if (!this.Packaging() || !this.Quantity()) return 0;
            return this.Quantity() * this.Packaging().Weight;
        }, this);
        // validation
        this.model = ko.validatedObservable({
            location: this.Location.extend({ required: true }),
            quantity: this.Quantity.extend({ required: true }),
            packaging: this.Packaging.extend({ required: true }),
            weight: this.Weight.extend({ required: true })
        });
        this.model.errors.subscribe(function(newErrors) {
            if (!newErrors.length) self.addOutput();
        });
    };
    
    // global vars
    self.popupVisible = ko.observable(false);
    self.OtherInventory = ko.observableArray([]);
    self.DehyInventory = ko.observableArray([]);
    self.Outputs = ko.observableArray([]);
    self.activeOutputs = ko.computed(function() {
        return ko.utils.arrayFilter(self.Outputs(), function (item) {
            return item.Quantity() && item.Quantity() > 0;
        });
    });

    self.Outputs.subscribe(function (newList) {
        if (self.Outputs().length < 1) self.addOutput();
    });

    // instances
    self.newLot = ko.observable(new self.Material());
    self.newTote = ko.observable(new self.Material());
    self.currentInventory = ko.observable(self.newLot());

    self.newInventory = ko.computed(function () {
        return $.grep(self.currentInventory().InventoryItems(), function (item, index) {
            return item.RealQty() > 0;
        });
    });
    self.allTotes = ko.observableArray();
    self.usedTotes = ko.computed(function () {
        return ko.utils.arrayFilter(self.allTotes(), function (tote) {
            return ko.utils.arrayFilter(tote.Inventory, function (item) {
                return item.RealQty() > 0;
            }).length > 0;
        });
    });
    
    // Removes
    
    self.removeOtherInv = function(item) {
        self.OtherInventory.splice(self.OtherInventory().indexOf(item), 1);
    };
    self.removeOutput = function(item) {
        self.Outputs.splice(self.Outputs().indexOf(item), 1);
    };
    self.removeOutputEsc = function(item, evt) {
        if (evt.keyCode == 27) {
            self.removeOutput(item);
            $(".outputRow").last().children()[1].children[0].focus();
            //$(evt.target).parent().siblings()[1].children[0].focus();
        }
        return true;
    };
    
   // popup responses
    self.cancelPopup = function() {
        self.popupVisible(false);
        self.newLot(self.currentInventory());
    };
    self.acceptPopup = function() {
        self.popupVisible(false);
        var inventory = self.newInventory();
        ko.utils.arrayPushAll(self.OtherInventory, inventory);
        self.newLot(new self.Material());
    };

    // total weights
    self.totalDehyWeight = ko.computed(function () {
        var w = 0;
        ko.utils.arrayForEach(self.DehyInventory(), function (item) { w += item.Weight(); });
        return w;
    });
    self.totalOtherWeight = ko.computed(function () {
        var w = 0;
        ko.utils.arrayForEach(self.OtherInventory(), function (item) { w += item.Weight(); });
        return w;
    });
    self.totalOutputWeight = ko.computed(function () {
        var w = 0;
        ko.utils.arrayForEach(self.Outputs(), function (item) { w += item.Weight(); });
        return w;
    });

    self.validOutputs = ko.computed(function () {
        return ko.utils.arrayFilter(self.Outputs(), function (item) {
            return item.model.isValid();
        });
    });
   
    // Adds
    self.addOutput = function() {
        if (self.validOutputs().length >= self.Outputs().length)
            self.Outputs.push(new self.OutputItem());
    };
    self.addLot = ko.asyncCommand({
        execute: function (complete) {
            var lot = self.newLot();
            $.ajax("/api/inventory/" + lot.Number.formattedLot(), {
                dataType: 'json',
                statusCode: {
                    200: function (data) {
                        data = data.InventoryItems;
                        if (data.length < 1) {
                            showUserMessage("There is no available inventory for the Lot \"" + lot.Number() + "\".");
                            return;
                        }

                        $.each(data, function (index, item) {
                            item.RealQty = ko.observable(data.length > 1 ? 0 : 1);
                            item.Weight = ko.computed(function () {
                                return item.RealQty() * item.PackagingCapacity;
                            });
                        });
                        lot.InventoryItems(data);
                        self.currentInventory(lot);
                        if(data.length > 1) self.popupVisible(true);
                        else self.acceptPopup();
                    },
                    404: function () {
                        showUserMessage("Invalid Lot Number", { description: "The lot number \"<strong>" + lot.Number.formattedLot() + "</strong>\" is not valid or was not found. Please check your entry and try again." });
                    },
                    500: function (data) {
                        console.log(data);
                        showUserMessage("Server side error during inventory lookup. Contact your administrator.");
                    }
                },
                complete: function () { complete(); }
            });
        },
        canExecute: function (isExecuting) {
            return self.newLot().Number() && !isExecuting;
        }
    });
    self.addTote = ko.asyncCommand({
        execute: function (complete) {
            var tote = self.newTote();
            $.ajax("/api/toteinventory/" + tote.Number(), {
                dataType: 'json',
                statusCode: {
                    200: function (data) {
                        $.each(data.Inventory, function (index, item) {
                            // not in list already
                            if (!~self.DehyInventory().indexOf(item)) {
                                item.ToteKey = tote.Number();
                                item.RealQty = ko.observable(data.length > 1 ? 0 : 1);
                                item.Weight = ko.computed(function () {
                                    return item.RealQty() * item.PackagingProduct.Weight;
                                });
                                self.DehyInventory.push(item);
                            }
                        });
                        self.allTotes.push(data);
                        self.newTote(new self.Material());
                    },
                    404: function () {
                        showUserMessage("Tote not found", { description: "There is no available inventory with Tote \"<strong>" + tote.Number() + "</strong>\"." });
                    },
                    500: function (data) {
                        console.log(data);
                        showUserMessage("Server side error during inventory lookup. Contact your administrator.");
                    }
                },
                complete: function () { complete(); }
            });
        },
        canExecute: function (isExecuting) {
            return self.newTote().Number() && !isExecuting;
        }
    });


    // Root info
    self.HeaderInfo = {};
    self.HeaderInfo.Line = ko.observable();
    self.HeaderInfo.Shift = ko.observable();
    self.HeaderInfo.ChileProduct = ko.observable();
    self.HeaderInfo.Begin = ko.observable().extend({ isoDate: "mm/dd/yyyy" });
    self.HeaderInfo.BeginTime = ko.observable().extend({ timeEntry: true });
    self.HeaderInfo.End = ko.observable().extend({ isoDate: "mm/dd/yyyy" });
    self.HeaderInfo.EndTime = ko.observable().extend({ timeEntry: true });
    self.HeaderInfo.TotalTime = ko.computed(function () {
        var tspan = "∞";
        var d1 = createDate(self.HeaderInfo.Begin(), self.HeaderInfo.BeginTime);
        var d2 = createDate(self.HeaderInfo.End(), self.HeaderInfo.EndTime);
        if (d1.getTime() && d2.getTime()) {
            tspan = Number((d2 - d1) / 1000 / 60 / 60.0).toFixed(2);
        }
        return tspan + ' hrs';
    });
    self.HeaderInfo.LotKey = ko.observable().extend({lotKey: true});
    self.HeaderInfo.ProductionDate = ko.computed(function () {
        var date = this.LotKey.Date();
        if (date && date != 'Invalid Date') return date.format("mm/dd/yyyy");
    }, self.HeaderInfo);
    self.HeaderInfo.Begin.formattedDate.subscribe(function (val) {
        self.HeaderInfo.End.formattedDate(val);
    });
    self.resetForm = resetForm;

    // SAVING
    self.save = ko.asyncCommand({
        execute: function (completed) {
            var model = self.toDto();
            var modelStr = ko.utils.stringifyJson(model);

            $.ajax("/api/millwetdown", {
                type: 'POST',
                contentType: 'application/json',
                data: modelStr,
                success: function (data) {
                    showUserMessage("Saved successfully.");
                    self.resetForm();
                },
                error: function (data) {
                    var msg = "Error Saving: ";
                    if (data.statusText) msg += data.statusText;
                    else if (data.responseText) msg += data.responseText;
                    else msg += "Unknown";
                    showUserMessage(msg);
                    console.log(data);
                },
                complete: function (data) { completed(); }
            });
        },
        canExecute: function (isExecuting) {
            var head = self.HeaderInfo;
            // require lot, shift, start/end, and at least 1 output and material
            return !isExecuting && (self.DehyInventory().length || self.OtherInventory().length) &&
                head.LotKey() && head.Shift() && head.Begin() && head.End() && self.activeOutputs().length > 0;
        }
    });

    // INIT
    self.init = function(model) {
        var vmOptions = ko.utils.parseJson(model);
        self.Options = vmOptions;
        self.previousLocation = getLocationByName('P01');
        self.addOutput();
        initDate.call(self);
        $("input[type=button]").attr("tabindex", -1); // disable tab focus

        $(document).keydown(function(evt) { // bind Ctrl+S to save
            if (evt.ctrlKey && evt.keyCode == 83) {
                self.save.execute();
                evt.preventDefault();
            }
        });
        document.querySelector("#lotKey").focus(); // initial focus
        return self;
    };
    return self;
    
    // private functions
    
    function resetForm() {
        resetHeader.call(this);
        
        this.allTotes([]);
        this.DehyInventory([]);
        this.OtherInventory([]);
        this.Outputs([]);
        
        document.querySelector("#shift").focus();
        
        function resetHeader() {
            incrementLot.call(this);
            clearHeaderInfo.call(this);
            initDate.call(this);
            
            function clearHeaderInfo() {
                var head = this.HeaderInfo;
                for (var prop in head) {
                    if (prop != 'LotKey' && ko.isWriteableObservable(head[prop])) head[prop](null);
                }
            }
        }
        function incrementLot() {
            var seq = 1 + parseInt(this.HeaderInfo.LotKey.Sequence());
            this.HeaderInfo.LotKey.Sequence(seq);
        };
    }
    
    function initDate() {
        this.HeaderInfo.Begin(Date.today().format("mm/dd/yyyy"));
    };
    
    function getDto() {
        var model = this;
        var head = model.HeaderInfo;
        return {
            User: '',
            ProductionDate: head.ProductionDate(),
            LotSequence: head.LotKey.Sequence(),
            ShiftKey: head.Shift(),
            ChileProductKey: head.ChileProduct().ProductKey,
            ProductionLineKey: head.Line().KeyValue,
            ProductionBegin: createDate(head.Begin(), head.BeginTime),
            ProductionEnd: createDate(head.End(), head.EndTime),
            ResultItems: createResultItemsDto.call(model),
            PickedItems: createPickedItemsDto.call(model)
        };
        
        function createResultItemsDto() {
            var results = [];
            $.each(this.activeOutputs(), function (index, item) {
                results.push({
                    WarehouseLocationKey: item.Location().WarehouseLocationKey,
                    Quantity: item.Quantity(),
                    PackagingProductKey: item.Packaging().ProductKey
                });
            });
            return results;
        }
        function createPickedItemsDto() {
            var picked = [];
            ko.utils.arrayPushAll(picked, getDehyItems.call(this));
            ko.utils.arrayPushAll(picked, getOtherInventoryItems.call(this));
            return picked;
            
            function getDehyItems() {
                return ko.utils.arrayMap(this.DehyInventory(), function(item) {
                    return {
                        Tote: item.ToteKey,
                        InventoryKey: item.InventoryKey,
                        Quantity: item.RealQty()
                    };
                });
            }
            function getOtherInventoryItems() {
                return ko.utils.arrayMap(this.OtherInventory(), function(item) {
                    return {
                        Tote: '',
                        InventoryKey: item.InventoryKey,
                        Quantity: item.RealQty()
                    };
                });
            }
        }
    }
})();