function LotAttribute( values, checkOutOfRange ) {
    if (!(this instanceof LotAttribute)) return new LotAttribute(values);

    var value = values.Value;

    this.Key = values.Key;
    this.Name = values.Name;
    this.Value = value;
    this.AttributeDate = values.AttributeDate;
    this.Defect = values.Defect;
    this.isValueComputed = values.Computed || false;

    this.outOfRange = 0;
    //this.outOfRange = typeof checkOutOfRange === 'function' && !ko.isObservable( checkOutOfRange ) ? checkOutOfRange.call( this, this.Key, this.Value ) : 0;
    if ( ko.isComputed( checkOutOfRange ) ) {
      this.outOfRange = ko.pureComputed(function() {
        var checkerFunction = checkOutOfRange();
        return typeof checkerFunction === 'function' && checkerFunction.call(this, this.Key, this.Value);
      }, this);
    } else if ( typeof checkOutOfRange === 'function' && !ko.isObservable( checkOutOfRange ) ) {
      this.outOfRange = checkOutOfRange.call( this, this.Key, this.Value );
    }

    this.formattedValue = value && value.toLocaleString();

    return this;
}

module.exports = LotAttribute;

