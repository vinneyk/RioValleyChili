function PackSchedule( packSchedule ) {
  var self = this;

  this.PackScheduleKey = packSchedule.PackScheduleKey;

  this.ProductionDeadline = packSchedule.ProductionDeadline;
  this.Instructions = packSchedule.Instructions;

  this.AverageGranularity = packSchedule.AverageGranularity;
  this.AverageAoverB = packSchedule.AverageAoverB;
  this.AverageScoville = packSchedule.AverageScoville;
  this.AverageScan = packSchedule.AverageScan;

  this.ChileProduct = packSchedule.ChileProduct || {};
}

function ScheduleItem( scheduleData, args ) {
  var self = this;

  this.Index = scheduleData.Index;
  this.FlushBefore = ko.observable( !!scheduleData.FlushBefore );
  this.FlushBeforeInstructions = ko.observable( scheduleData.FlushBeforeInstructions || '' );
  this.FlushAfter = ko.observable( !!scheduleData.FlushAfter );
  this.FlushAfterInstructions = ko.observable( scheduleData.FlushAfterInstructions || '' );
  this.PackSchedule = new PackSchedule( scheduleData.PackSchedule || {} );
}

ScheduleItem.prototype.toDto = function() {
  return {
    Index: this.Index,
    FlushBefore: this.FlushBefore,
    FlushBeforeInstructions: this.FlushBefore() ? this.FlushBeforeInstructions : null,
    FlushAfter: this.FlushAfter,
    FlushAfterInstructions: this.FlushAfter() ? this.FlushAfterInstructions : null,
    PackScheduleKey: this.PackSchedule && this.PackSchedule.PackScheduleKey
  };
};

function SchedulesEditor( editorData ) {
  var self = this;

  this.ProductionScheduleKey = editorData.ProductionScheduleKey;
  this.Links = editorData.Links;

  // Editable Data
  this.ScheduledItems = ko.observableArray();
  var scheduledItemsMaxIndex = ko.pureComputed(function() {
    return self.ScheduledItems().length;
  });

  function buildItem( scheduleItem, args ) {
    var _item = new ScheduleItem( scheduleItem, args );

    _item.currentIndex = ko.pureComputed(function() {
      return self.ScheduledItems().indexOf( _item );
    });

    _item.canMoveUp = ko.computed(function() {
      return _item.currentIndex() > 0;
    });

    _item.canMoveDown = ko.computed(function() {
      return _item.currentIndex() < scheduledItemsMaxIndex() - 1;
    });

    return _item;
  }

  var _scheduleItems = ko.utils.arrayForEach( editorData.ScheduledItems || [], function( scheduleItem ) {
    var args = {
      maxIndex: scheduledItemsMaxIndex,
    };

    var _item = buildItem( scheduleItem, args );

    self.ScheduledItems.push( _item );
  });

  this.moveItemToTop = function( data, element ) {
    var _items = self.ScheduledItems();

    var i = _items.indexOf( data );
    if ( i > 0 ) {
      self.ScheduledItems.splice( i, 1 );
      self.ScheduledItems.unshift( data );
    }
  };

  this.moveItemUp = function( data, element ) {
    var _items = self.ScheduledItems();

    var i = _items.indexOf( data );
    if ( i > 0 ) {
      self.ScheduledItems.splice( i, 1 );
      self.ScheduledItems.splice( i - 1, 0, data );
    }
  };

  this.moveItemDown = function( data, element ) {
    var _items = self.ScheduledItems();
    var max = _items.length;

    var i = _items.indexOf( data );
    if ( i < max ) {
      self.ScheduledItems.splice( i, 1 );
      self.ScheduledItems.splice( i + 1, 0, data );
    }
  };

  this.moveItemToBottom = function( data, element ) {
    var _items = self.ScheduledItems();
    var max = _items.length;

    var i = _items.indexOf( data );
    if ( i < max ) {
      self.ScheduledItems.splice( i, 1 );
      self.ScheduledItems.push( data );
    }
  };

  // Validation
  this.validation = ko.validatedObservable({

  });
}

SchedulesEditor.prototype.toDto = function() {
  var _items = this.ScheduledItems();
  var _dtoItems = [];

  for ( var i = 0; i < _items.length; i++ ) {
    _items[i].Index = i;
    _dtoItems.push( _items[i].toDto() );
  }

  return ko.toJS({
    ProductionScheduleKey: this.ProductionScheduleKey,
    ScheduledItems: _dtoItems
  });
};

function ProductionSchedulesEditorVM( params ) {
  if ( !(this instanceof ProductionSchedulesEditorVM) ) { return new ProductionSchedulesEditorVM( params ); }

  var self = this;

  this.disposables = [];

  this.editor = ko.observable( buildEditor( ko.unwrap( params.input ) ) );

  function buildEditor( editorData ) {
    var editor = new SchedulesEditor( editorData );

    editor.dirtyFlag = (function( root, isInitiallyDirty ) {
      var result = function() {},
      _initialState = ko.observable( ko.toJSON( root ) ),
      _isInitiallyDirty = ko.observable( isInitiallyDirty );

      result.isDirty = ko.computed(function() {
        return _isInitiallyDirty() || _initialState() !== ko.toJSON(root);
      });

      result.reset = function() {
        _initialState( ko.toJSON( root ) );
        _isInitiallyDirty( false );
      };

      return result;
    })( editor.ScheduledItems );

    return editor;
  }

  this.isDirty = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.dirtyFlag.isDirty();
  });

  function resetDirtyFlag() {
    var _editor = self.editor();

    return _editor && _editor.dirtyFlag.reset();
  }

  var inputSub = params.input.subscribe(function( schedule ) {
    if ( schedule ) {
      self.editor( buildEditor( schedule ) );
    }
  });

  function toDto() {
    var _editor = self.editor();

    return _editor && _editor.toDto();
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      isDirty: this.isDirty,
      resetDirtyFlag: resetDirtyFlag,
      toDto: toDto
    });
  }

  return this;
}

ko.utils.extend(ProductionSchedulesEditorVM.prototype, {
    dispose: function() {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
        ko.utils.objectForEach(this, this.disposeOne);
    },

    // little helper that handles being given a value or prop + value
    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

module.exports = {
  viewModel: ProductionSchedulesEditorVM,
  template: require('./production-schedules-editor.html')
};
