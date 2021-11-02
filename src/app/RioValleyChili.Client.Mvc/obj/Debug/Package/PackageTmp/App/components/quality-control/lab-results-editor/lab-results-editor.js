var rvc = require('app');
var lotService = require('App/services/lotService');

require('App/helpers/koPunchesFilters.js');
require('bootstrap');

ko.components.register( 'allowances-editor', require('App/components/quality-control/allowances-editor/allowances-editor'));

ko.punches.enableAll();

ko.dirtyFlag = function( root, isInitiallyDirty ) {
  var result = function() {};
  var _initialState = ko.observable( ko.toJSON( root ) );
  var _isInitiallyDirty = ko.observable( isInitiallyDirty );

  result.isDirty = ko.computed(function() {
    return _isInitiallyDirty() || _initialState() !== ko.toJSON( root );
  });

  result.reset = function() {
    _initialState( ko.toJSON( root ));
    _isInitiallyDirty( false );
  };

  return result;
};

/** Attribute constructor */
function Attribute( attr ) {
  var self = this;

  var defect = attr.defects || {};
  var _hasDefect = !!defect;
  var isTheoretical = attr.data.Computed || attr.data.isValueComputed || false;

  this.AttributeKey = attr.Key;
  this.isEditable = this.AttributeKey !== 'AstaC';
  this.isResolving = ko.observable( false );

  this.NewValue = ko.observable( null ).extend({ notify: 'always' });

  var initialValue = attr.data.Value != null ?
    '' + attr.data.Value :
    null;
  this.NewValue( initialValue );

  this.AttributeDate = ko.observableDate( attr.data.AttributeDate ).extend({
    required: {
      message: "Date required for all value entries",
      onlyIf: function() {
        var val = self.NewValue();

        return val != null && val !== '' && self.AttributeKey !== 'AstaC';
      }
    }
  });

  this.MinValue = attr.data.MinValue;
  this.MaxValue = attr.data.MaxValue;
  this.Defect = ko.observable( defect.DefectType ).extend({ defectType: true });
  this.Resolution = ko.observable(defect.Resolution ? new DefectResolution(defect.Resolution) : null);

  /** Resolution data if value update corrects defect */
  this.isResolved = ko.pureComputed(function () {
    var r = this.Resolution();
    return r && r.ResolutionType() != null;
  }, self);

  this.isInSpec = ko.pureComputed(function () {
    return isInSpec(self.NewValue());
  });

  function isInSpec(val) {
    var hasSpec = self.MinValue != null && self.MaxValue != null;
    if (hasSpec && val != null) {
      return val >= self.MinValue && val <= self.MaxValue;
    } else {
      return true;
    }
  }

  this.isDefective = ko.pureComputed(function() {
    return !self.isResolved() && !self.isInSpec();
  });

  this.isActive = ko.pureComputed(function() {
    return self.NewValue() || self.AttributeDate();
  });

  this.isTheoreticalValue = ko.pureComputed(function() {
    return !self.isEditable || (isTheoretical && !isValueChanged());
  });


  this.dirtyFlag = new ko.dirtyFlag({
    value: self.NewValue,
    date: self.AttributeDate
  });

  this.validation = ko.validatedObservable({
    AttributeDate: self.AttributeDate,
    ResolutionType: self.Resolution
  }, { deep: true });

  function isValueChanged() {
    var val = +self.NewValue();
    return val !== Number(initialValue);
  }

  var resolutionHold = self.Resolution.peek();
  var newValWatcher = ko.computed(function () {
    if (!isValueChanged()) {
      self.Resolution( resolutionHold );
      self.isResolving(false);
      return;
    } else if ( initialValue === '' || initialValue == null ) {
      self.isResolving( false );
    } else if (self.isInSpec() && !isInSpec( Number(initialValue) )) {
      var defaults = isTheoretical ? {
        ResolutionType: rvc.lists.defectResolutionTypes.InvalidValue.key,
        Description: 'Invalid theoretical values'
      } : {};
      self.Resolution(new DefectResolution(defaults));
      self.isResolving(true);
    } else {
      self.Resolution(null);
      self.isResolving(false);
    }
  });
}

Attribute.prototype.toDto = function() {
  return {
    AttributeKey: this.AttributeKey,
    AttributeInfo: this.NewValue() != null && this.AttributeDate() != null ?
      {
        Value: this.NewValue,
        Date: this.AttributeDate
      } :
      null,
    Resolution: this.Resolution()
  };
};


function DefectResolution(values) {
  values = values || {};
  var me = {
    ResolutionType: ko.observable(values.ResolutionType).extend({
      defectResolutionType: true,
      required: {
        message: "Resolution type is required"
      }
    }),
    Description: ko.observable(values.Description)
      //.extend({
      //required: {
      //  message: "Resolution type is required",
      //  //onlyIf: function () {
      //  //  return self.isNewlyResolved();
      //  //}
      //}
      //})
  }

  me.setResolutionType = function (resolution) {
    me.ResolutionType(resolution.key);
  };

  return me;
}

/** Editor constructor */
function EditorDataModel( labData, attributeList ) {
  var self = this;

  this.labData = labData;

  var _defects = this.mapDefects( this.labData.Defects );

  this.lotKey = this.labData.LotKey;
  this.customers = ko.observableArray( [] );

  this.lastDateEntered = ko.observable( null );
  var _attrValues = self.labData.Attributes;

  this.attrs = ko.observableArray( this.mapAttributes( attributeList, _attrValues, _defects ) );

  this.cacheData = {
    holdType: ko.observable( labData.HoldType )
  };

  this.OverrideOldContextLotAsCompleted = ko.observable( false );

  /** Header data */
  this.header = {
    productionStatus: ko.observable( self.labData.ProductionStatus ).extend({ productionStatusType: true }),
    qualityStatus: ko.observable( self.labData.QualityStatus ).extend({ lotQualityStatusType: true }),
    oldSystemLotStat: ko.observable( self.labData.OldContextLotStat || 'Unknown' ),
    customer: self.labData.CustomerName || '-',
    customerSpec: ko.observable( self.labData.ProductSpecStatus ),
    notes: ko.observable( self.labData.Notes ),
  };

  var _notesDirty = new ko.dirtyFlag({
    notes: this.header.notes
  });

  /** Allowances editor */
  this.allowancesEditor = {
    input: {
      allowances: {
        contracts: self.labData.ContractAllowances,
        customers: self.labData.CustomerAllowances,
        customerOrders: self.labData.CustomerOrderAllowances
      },
      customers: self.customers,
      lotKey: self.lotKey,
      createAllowance: self.createAllowance,
      deleteAllowance: self.deleteAllowance,
    },
    exports: ko.observable(  )
  };
  this.allowancesEditor.totalAllowances = ko.pureComputed(function() {
    var editor = self.allowancesEditor.exports();

    return editor && editor.totalAllowances();
  });

  this.lotStatuses = ko.pureComputed(function() {
    var statusList = [];

    ko.utils.arrayForEach( self.labData.ValidLotQualityStatuses, function( key ) {
      var statusTypes = rvc.lists.lotQualityStatusTypes;

      var matchedStatus = ko.utils.arrayFirst( Object.keys( statusTypes ), function( stat ) {
        return statusTypes[ stat ].key === key;
      });

      if ( matchedStatus ) {
        statusList.push( statusTypes[ matchedStatus ]);
      }
    });

    return statusList;
  });

  this.qualityHold = {
    holdType: ko.observable( self.labData.HoldType ).extend({ lotHoldType: true }),
    description: ko.observable( self.labData.HoldDescription )
  };

  this.isDirty = ko.pureComputed(function() {
    var dirtyAttrs = ko.utils.arrayFilter( self.attrs(), function( attr ) {
      return attr.dirtyFlag.isDirty();
    });

    return dirtyAttrs.length > 0 || _notesDirty.isDirty();
  });
}

EditorDataModel.prototype.mapDefects = function( defects ) {
  var defectsObj = {};

  ko.utils.arrayForEach( defects, function( defect ) {
    if (defect.AttributeDefect) {
      defectsObj[ defect.AttributeDefect.AttributeShortName ] = defect;
    }
  });

  return defectsObj;
};

EditorDataModel.prototype.mapAttributes = function( attributes, attributeValues, defects ) {
  var lastDateEntered = ko.observable( null );
  var mappedAttrs = ko.utils.arrayMap( attributes, mapAttribute );

  //attribute view model
  function mapAttribute( attr ) {
    var key = attr.Key;
    var _lastDate = lastDateEntered;
    var attrData = ko.utils.arrayFirst( attributeValues, function( labAttr ) {
      return labAttr.Key === key;
    }) || {};

    attrData.MinValue = attr.MinValue;
    attrData.MaxValue = attr.MaxValue;

    var newAttr = new Attribute({
      Key: attr.Key,
      data: attrData,
      defects: defects[attr.Key]
    });
    newAttr.showResolution = ko.computed(function() {
      return newAttr.isResolved() && !newAttr.isResolving();
    });
    newAttr.showDefectResolutionUI = ko.computed(function() {
      return newAttr.isEditable && newAttr.isResolving();
    });
    newAttr.showResolveDefectButton = ko.computed(function() {
      return newAttr.isEditable && newAttr.isResolving();
    });
    newAttr.resolveDefect = ko.command({
      execute: function () {
        newAttr.Resolution(new DefectResolution());
        newAttr.isResolving(true);
      },
      canExecute: function () {
        return newAttr.isEditable && newAttr.isDefective() && !newAttr.isResolving();
      }
    });
    newAttr.cancelResolution = ko.command({
      execute: function () {
        newAttr.Resolution(null);
        newAttr.isResolving(false);
      },
      canExecute: function () {
        return true;
      }
    });

    newAttr.defectClass = ko.pureComputed(function () {
      return newAttr.isResolved() ?
        "defect unresolved resolved" :
          newAttr.isInSpec() ? '' : getDefectClassNameByDefectType(newAttr.Defect());
    });

    var cachedDate = newAttr.AttributeDate();

    newAttr.NewValue.subscribe(function( val ) {
      if ( val && !newAttr.AttributeDate() ) {
        newAttr.AttributeDate( _lastDate() );
      }
    });

    newAttr.AttributeDate.subscribe(function( date ) {
      if ( date && date !== cachedDate ) {
        _lastDate( date );
        cachedDate = date;
      }
    });

    return newAttr;
  }

  var defectClasses = {
    0: '',
    1: 'bacteria',
    3: 'actionable',
  };
  function getDefectClassNameByDefectType(key) {
    return "defect unresolved " + defectClasses[key];
  };

  return mappedAttrs;
};

EditorDataModel.prototype.validate = function () {
  var val = ko.validatedObservable({
    attrs: this.attrs
  }, { deep: true });

  if (!val.isValid()) {
    val.errors.showAllMessages();
    return false;
  }

  return true;
}

EditorDataModel.prototype.toDto = function () {
  var attributes = ko.utils.arrayFilter( this.attrs(), function (a) {
    return a.isEditable === true;
  });

  if ( this.validate() ) {
    var dto = {
      LotKey: this.lotKey,
      Notes: this.header.notes,
      OverrideOldContextLotAsCompleted: this.OverrideOldContextLotAsCompleted
    };

    dto.Attributes = ko.utils.arrayMap( attributes, function( attr ) {
      return attr.toDto();
    });

    return ko.toJS( dto );
  } else {
    return null;
  }
};

/**
  * @param {Object} input - Input methods and properties
  * @param {Object} data - Results data
  * @param {Object[]} attrs - List of attributes to display
  * @param {Object} exports - Observable, container for exported methods/properties
  */
function LabResultsEditorVM( params ) {
  if ( !(this instanceof LabResultsEditorVM) ) { return new LabResultsEditorVM( params ); }

  var self = this;

  // Data
  var attributeList = params.attrs;

  EditorDataModel.prototype.createAllowance = createAllowance;
  EditorDataModel.prototype.deleteAllowance = deleteAllowance;
  this.labResult = ko.observable( buildLabResult() );

  params.data.subscribe(function( newData ) {
    if ( newData ) {
      self.labResult( buildLabResult() );
    }
  });

  this.lotKey = ko.pureComputed(function() {
    return self.labResult().lotKey;
  });

  this.validationOpts = {
    insertMessages: false,
    decorateInputElement: true,
    decorateElement: true,
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block',
    decorateElementOnModified: false,
  };

  // Behaviors
  function setAttributeValues( newAttrs ) {
    var attrs = self.labResult().attrs();
    var newAttrsObj = {};

    ko.utils.arrayForEach( newAttrs, function( newAttr ) {
      newAttrsObj[ newAttr.Key ] = newAttr;
    });

    ko.utils.arrayForEach( attrs, function( attr ) {
      var newAttr = newAttrsObj[ attr.AttributeKey ];

      attr.NewValue( newAttr && newAttr.Value != null ? newAttr.Value : '' );
      attr.AttributeDate( newAttr && newAttr.AttributeDate );
    });
  }
  function buildLabResult() {
    var data = ko.toJS( params.data );
    var attrs = ko.unwrap( attributeList );
    var labResult = new EditorDataModel( data, attrs );

    labResult.customers( ko.unwrap( params.input.customers ) );

    return labResult;
  }

  function setQualityStatus( status ) {
    return params.input.setStatus( self.lotKey(), status );
  }

  this.overrideLotStatus = function( lotStatus, element ) {
    var newVal = lotStatus.key;
    var labResult = self.labResult();
    var currentStatus = labResult.header.qualityStatus();

    if ( newVal != currentStatus ) {
      var setStatus = setQualityStatus( newVal ).then(
      function( data, textStatus, jqXHR ) {
        labResult.header.qualityStatus( newVal );
        labResult.header.oldSystemLotStat( data.LotStat || 'Unknown' );
      });

      return setStatus;
    }
  };

  function createAllowance( lotKey, type, key ) {
    return params.input.createAllowance( lotKey, type, key );
  }

  function deleteAllowance( lotKey, type, key ) {
    return params.input.deleteAllowance( lotKey, type, key );
  }

  function toDto() {
    return self.labResult().toDto();
  }

  this.saveQualityHold = ko.asyncCommand({
    execute: function( complete ) {
      var labResult = self.labResult();
      var lotKey = self.lotKey();
      var holdData = ko.toJS( labResult.qualityHold );

      var save = params.input.setQualityHold( lotKey, holdData ).then(
      function( data, textStatus, jqXHR ) {
        labResult.cacheData.holdType( holdData.holdType );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.isDirty = ko.pureComputed(function() {
    var labResult = self.labResult();
    return labResult && labResult.isDirty();
  });

  // Exports
  if ( params && params.exports ) {
    params.exports({
      toDto: toDto,
      isDirty: this.isDirty,
      setAttributeValues: setAttributeValues
    });
  }

  return this;
}

module.exports = {
  viewModel: LabResultsEditorVM,
  template: require('./lab-results-editor.html')
};
