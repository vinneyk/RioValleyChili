(function() {
  ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindings) {
      $(element).wrap('<div class="input-group"></div>');
      $(element).datepicker({
        showOn: 'button',
        buttonText: '<i class="fa fa-calendar"></i>',
        changeMonth: true,
        changeYear: true
      }).next(".ui-datepicker-trigger")
          .addClass("btn btn-default")
          .attr( 'tabindex', '-1' )
          .wrap('<span class="input-group-btn"></span>');

      ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
        //todo: cleanup wrapper element
        $(element).datepicker('destroy');
      });

      var value = valueAccessor();
      if (ko.isObservable(value)) {
        ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
      }
    }
  };
}());
