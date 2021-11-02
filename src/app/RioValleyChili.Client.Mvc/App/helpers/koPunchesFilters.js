var rvc = require('rvc');

var koPunches = (function () {
  var self = this;

  // Behaviors
  ko.filters.toDate = function (value) {
    if ( value == null ) {
      return null;
    }

    var dateStr = null,
      input = new Date(ko.unwrap(value));

      var month = (input.getUTCMonth() + 1).toString();
      month = month.length === 2 ? month : "0" + month;

      var day = input.getUTCDate().toString();
      day = day.length === 2 ? day : "0" + day;

    dateStr = month + '/' + day + '/' + input.getUTCFullYear();

    return dateStr;
  };

  ko.filters.toDateTime = function ( value, format ) {
    var dateObj;

    if (typeof value === "string") {
      dateObj = new Date( value );
    }

    if ( !(dateObj instanceof Date) ) {
      throw new Error( 'Invalid input. Expected date but encountered ' + (typeof input) + '.' );
    }

    return dateObj.format( format || 'm/d/yyyy hh:MM TT' );
  };

  ko.filters.lotKey = function (input) {
    var value = ko.unwrap(input);

    if (value == undefined) {
      return '';
    }

    var key = value.toString().replace(/ /g, '');
    var keyLength = key.length;

    if (keyLength === 0) {
      return '';
    } else if (keyLength <= 2) {
      return key;
    } else if (keyLength <= 4) {
      return [key.substr(0, 2), key.substr(2)].join(' ');
    } else if (keyLength <= 7) {
      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3)].join(' ');
    } else {
      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3), key.substr(7)].join(' ');
    }
  };

  ko.filters.toteKey = function (input) {
    var value = ko.unwrap(input);

    if (value == undefined) {
      return '';
    }

    var key = value.toString().replace(/ /g, '');
    if (key.length === 0) {
      return '';
    } else if (key.length <= 2) {
      return key;
    } else if (key.length <= 4) {
      return [key.substr(0, 2), key.substr(2)].join(' ');
    } else {
      return [key.substr(0, 2), key.substr(2, 2), key.substr(4)].join(' ');
    }
  };

    ko.filters.secToHrMin = function(value) {
      var valueNum = +ko.unwrap(value);
      var isNegative = valueNum < 0;

      // Parse as positive number, Math.floor rounds negative numbers down
      // Ex: -0.2 = -1
      if (!isNaN(valueNum)) {
        var secondsTotal = isNegative ? -valueNum : valueNum;
        var hours = Math.floor(secondsTotal / 3600);
        var minutes = Math.floor((secondsTotal - (3600 * hours)) / 60);

        return isNegative ?
          "".concat('-', hours, 'h ', minutes, 'm') :
          "".concat(hours, 'h ', minutes, 'm');
      } else {
        return '0m';
      }
    };

  ko.filters.USD = function(value) {
    var amt = parseFloat(ko.unwrap(value));

    return typeof amt === 'number' ?
      '$' + amt.toFixed(2) :
      '';
  };

  ko.filters.toFixed = function(value, numOfDigits) {
    var amt = parseFloat(ko.unwrap(value));

    return typeof amt === 'number' ?
      amt.toFixed(numOfDigits || 2) :
      '';
  };

  ko.filters.toNumber = function( value ) {
    var numValue = value != null ? +value : null;

    if ( numValue != null ) {
      return numValue.toLocaleString();
    } else {
      return null;
    }
  };

  ko.filters.contractStatus = function ( value ) {
    var input = ko.unwrap( value );
    var statuses = rvc.lists.contractStatuses;
    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
      return statuses[ status ].key === input;
    });

    return statuses[ statusKey ].value;
  };

  ko.filters.orderStatus = function ( value ) {
    var input = ko.unwrap( value );
    var statuses = rvc.lists.orderStatus;
    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
      return statuses[ status ].key === input;
    });

    return statuses[ statusKey ].value;
  };

  ko.filters.sampleStatus = function( value ) {
    var input = ko.unwrap( value );
    var statuses = rvc.lists.sampleStatusTypes;
    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
      return statuses[ status ].key === input;
    });

    return statuses[ statusKey ].value;
  };

  ko.filters.statusType = function( value, statusName ) {
    var input = ko.unwrap( value );

    if ( input == null ) {
      return;
    }

    var statuses = rvc.lists[ statusName ];
    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
      return statuses[ status ].key === input;
    });

    return statusKey != null ? statuses[ statusKey ].value : null;
  };

  ko.filters.name = function( value ) {
    var input = ko.unwrap( value );

    return input && input.Name;
  };

  ko.filters.length = function( value ) {
    if ( typeof value === 'string' || Array.isArray( value ) ) {
      return value.length;
    }

    return 0;
  };

  // Exports
  return this;
})();

module.exports = koPunches;
