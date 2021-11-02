var rvc = require('rvc');
define(['ko', 'scripts/ko.extenders.date'], function () {
    ko.extenders.lotKey = function (target, options) {
        var matchCallback, changedCallback;

        if (typeof options === "function") {
            matchCallback = options;
            changedCallback = null;
        } else if(options != undefined && typeof options === "object") {
            matchCallback = options.matchCallback;
            changedCallback = typeof options.changedCallback === "function" ? options.changedCallback : undefined;
        }

	    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)(\d{2}\d*)?$/,
	        newPattern = /^([01])?([0-9]{1})\s{0,}(\d{1,2})?\s{0,}(\d{1,3})?\s{0,}(\d{1,})?(.*)$/,
            completePattern = /^(\d{2})(\s)(\d{2})(\s)(\d{3})(\s)(\d{2}\d*)$/,
            isComplete = ko.observable(false);

	    target.formattedLot = ko.pureComputed({
			read: function () {
				return target();
			},
			write: function (value) {
				value = cleanInput(value);
				if (target.peek() === value) return;

				var formatted = formatAsLot(value);
				target(formatted);
			    if (formatted && formatted.match(completePattern)) {
			        isComplete(true);
			        if (typeof matchCallback === "function") matchCallback(formatted);
			        if (typeof changedCallback === "function") changedCallback(value, formatted);
			    } else changedCallback && changedCallback(value, undefined); // the second argument of the changedCallback is only returned when the lot key is complete

			}
		});
        
		function cleanInput(input) {
			if (typeof input == "number") input = input.toString();
			if (typeof input !== "string") return undefined;
			return input.trim();
		}
		function formatAsLot(input) {
            var re = /\d+/g,
            newInput = input.match(re);

			if (newInput === undefined || newInput === null) { return; }
			newInput = newInput.join('');

			return newInput.replace(newPattern, function(match, p1, p2, p3, p4, p5, p6) {
			    if (p1) {
			        return [String(p1) + p2, p3, p4, p5].join(' ');
                } else if (!p1 && (p2 === "0" || p2 === "1")) {
                    return [p2, p3, p4, p5].join(' ');
                } else {
                    return [String(0) + p2, p3, p4, p5].join(' ');
                }
            }).trim();
		}

		target.match = function (valueToCompare) {
			var partialPattern = new RegExp('^' + target.formattedLot() + '$');
			return valueToCompare.match(partialPattern);
		};
		target.isComplete = ko.pureComputed(function () {
			return isComplete();
		}, target);
		target.Date = ko.pureComputed(function () {
		    var formattedLot = this.formattedLot();
			if (formattedLot) {
				var sections = formattedLot.split(" ");
				var days = parseInt(sections[2]);
				var defDate = "1/1/" + (parseInt(sections[1]) >= 90 ? "19" : "20");
				var date = new Date(defDate + sections[1]).addDays(days - 1);
				date.addMinutes(date.getTimezoneOffset());

				return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
			}
		}, target);
		target.formattedDate = ko.pureComputed(function () {
			var date = this();
			if (date && date != 'Invalid Date') return date.format("UTC:m/d/yyyy");
			return '';
		}, target.Date);
		target.LotType = ko.pureComputed(function () {
		  var lot = target.formattedLot();
		  if( lot ) {
		    var sections = lot.split(" ");
		    return Number(sections[0]);
		  }
		});
		target.InventoryTypeKey = ko.pureComputed(function () {
		  var lotType = target.LotType();
      switch (lotType) {
        case 1:
        case 2:
        case 3:
        case 11:
        case 12:
          return rvc.lists.inventoryTypes.Chile.key;
        case 4:
          return rvc.lists.inventoryTypes.Additive.key;
        case 5:
          return rvc.lists.inventoryTypes.Packaging.key;
        default:
          return null;
		  }
		});
		target.Sequence = ko.pureComputed({
			read: function () {
				if (this.formattedLot()) {
					var sections = this.formattedLot().split(" ");
					if (sections.length === 4)
						return sections[3];
				}
			},
			write: function (newSeq) {
				var val = '';
				if (isComplete()) {
					var reg = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)/;
					val = this.formattedLot().match(reg)[0];
					val += newSeq < 10 ? '0' : '';
					val += newSeq;
					this.formattedLot(val);
				}
			}
		}, target);
		target.getNextLot = function () {
			var sequence = parseInt(target.Sequence());
			sequence++;
			if (sequence < 10) sequence = '0' + sequence;
			return target.formattedLot().replace(pattern, '0$2 $4 $6 ' + sequence);
		};

		target.extend({ throttle: 800 });

		target.formattedLot(target.peek());
		return target;
	};
});
