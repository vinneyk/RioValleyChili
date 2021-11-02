function ScheduleItem(packScheduleNumber, preInstruction, postInstruction) {
    this.PackScheduleNumber = packScheduleNumber;
    this.PreInstruction = preInstruction;
    this.PostInstruction = postInstruction;
}

function ScheduleItem2(packScheduleNumber, chileProduct, packagingName, units, dateDue, index) {
    this.packScheduleKey = packScheduleNumber;
    this.index = index;
    this.chileProductName = chileProduct;
    this.packagingName = packagingName;
    this.units = units;
    this.dateDue = dateDue;
    this.itemTitle = this.chileProductName + " #" + this.packScheduleKey;
    this.unitsToProduce = this.units + " - " + this.packagingName;
}

var ProductionSchedulerViewModel = function () {
    var self = this;

    self.scheduleItems = ko.observableArray([]);

    self.getOrderedItems = function () {
        var orderedBatchItems = [];
        $("#sortable>li").each(function () {
            orderedBatchItems.push(
                new ScheduleItem(this.id, self.getPreInstruction(this.id), self.getPostInstruction(this.id)));
        });

        return orderedBatchItems;
    };

    self.getPreInstruction = function (packScheduleNumber) {
        return self.getInstructionText(packScheduleNumber, "preInst");
    };

    self.getPostInstruction = function (packScheduleNumber) {
        return self.getInstructionText(packScheduleNumber, "postInst");
    };

    self.getInstructionText = function (packScheduleNumber, target) {
        return $("li#" + packScheduleNumber + " input." + target).first().val();
    };

    self.pushScheduleItems = function (vm) {
        var mapped = ko.utils.arrayMap(vm, function (item) {
            return new ScheduleItem2(
                item.PackScheduleKey,
                item.ChileProductName,
                item.PackagingName,
                item.UnitsToProduce,
                item.DateDueDisplay,
                item.OrderIndex);
        });

        ko.utils.arrayPushAll(self.scheduleItems, mapped);
    };
};

$(function() {
    var $sortable = $("#sortable");
    var $showInst = $("#showInst");
    var $hideInst = $("#hideInst");
    var $allInst = $(".batch-inst");

    $allInst.hide();
    $hideInst.hide();

    $sortable.sortable({
        placeholder: "ui-state-highlight"
    });
    $sortable.disableSelection();
    $showInst.click(function() {
        $allInst.slideDown();
        $showInst.hide();
        $hideInst.show();
    });
    $hideInst.click(function() {
        $allInst.slideUp();
        $showInst.show();
        $hideInst.hide();
    });
});